//------------------------------------------------------------------------------------------------------------------
// Dynamic Fog & Mist 2
// Created by Kronnect
//------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace DynamicFogAndMist2 {


    [ExecuteInEditMode]
    public partial class DynamicFog : MonoBehaviour {

        public DynamicFogProfile profile;

        [Tooltip("Fades in/out fog effect or blends with sub-volumes when reference object enters the fog volume.")]
        public bool enableFade;
        public float fadeDistance = 1;
        [Tooltip("The controller (player or camera) to check if enters the fog volume.")]
        public Transform fadeController;
        [Tooltip("Enable sub-volume blending.")]
        public bool enableSubVolumes;
        [Tooltip("Allowed subVolumes. If no subvolume is specified, any subvolume entered by this controller will affect this fog volume.")]
        public List<DynamicFogSubVolume> subVolumes;
        [Tooltip("Shows the fog volume boundary in Game View")]
        public bool showBoundary;
        [Tooltip("Allows rotation of the fog volume")]
        public bool allowRotation;

        static class ShaderParams {
            public static int MainTex = Shader.PropertyToID("_MainTex");
            public static int SunDir = Shader.PropertyToID("_SunDir");
            public static int LightColor = Shader.PropertyToID("_LightColor");
            public static int GradientTex = Shader.PropertyToID("_GradientTex");
            public static int Dithering = Shader.PropertyToID("_DitherStrength");

            public static int WindDirection = Shader.PropertyToID("_WindDirection");
            public static int Density = Shader.PropertyToID("_Density");
            public static int LightDiffusionPower = Shader.PropertyToID("_LightDiffusionPower");
            public static int LightDiffusionIntensity = Shader.PropertyToID("_LightDiffusionIntensity");

            public static int Color = Shader.PropertyToID("_Color");
            public static int SecondColor = Shader.PropertyToID("_SecondColor");

            public static int BoundsCenter = Shader.PropertyToID("_BoundsCenter");
            public static int BoundsExtents = Shader.PropertyToID("_BoundsExtents");
            public static int Geom = Shader.PropertyToID("_Geom");

            public static int NoiseData = Shader.PropertyToID("_NoiseData");
            public static int FogOfWarTex = Shader.PropertyToID("_FogOfWarTex");
            public static int FogOfWarCenter = Shader.PropertyToID("_FogOfWarCenter");
            public static int FogOfWarCenterAdjusted = Shader.PropertyToID("_FogOfWarCenterAdjusted");

            public static int FogOfWarSize = Shader.PropertyToID("_FogOfWarSize");
                
            public static int TurbulenceAmount = Shader.PropertyToID("_TurbulenceAmount");

            public const string SKW_FOW = "DF2_FOW";
            public const string SKW_BOX_PROJECTION = "DF2_BOX_PROJECTION";
            public const string SKW_DEPTH_CLIP = "DF2_DEPTH_CLIP";
            public const string SKW_3D_DISTANCE = "DF2_3D_DISTANCE";
        }

        Renderer r;
        Material fogMat, turbulenceMat;
        Material fogMat2D, turbulenceMat2D;
        RenderTexture rtTurbulence;
        float turbAcum;
        float windAcum;
        Vector3 sunDir;
        float dayLight;
        static Texture2D noiseTex;
        Texture2D gradientTex;
        Mesh debugMesh;
        Material fogDebugMat;
        DynamicFogProfile activeProfile, lerpProfile;
        Vector3 lastControllerPosition;
        float alphaMultiplier = 1f;

        public static bool requireSubvolumeUpdate;


        void OnEnable() {
            if (noiseTex == null) {
                noiseTex = Resources.Load<Texture2D>("DynamicFog/Textures/NoiseTex256");
            }
            FogOfWarInit();
            UpdateMaterialProperties();
        }

        private void OnDisable() {
            if (profile != null) {
                profile.onSettingsChanged -= UpdateMaterialProperties;
            }
        }

        private void OnValidate() {
            UpdateMaterialProperties();
        }

        private void OnDestroy() {
            if (rtTurbulence != null) {
                rtTurbulence.Release();
            }
            if (fogMat != null) {
                DestroyImmediate(fogMat);
                fogMat = null;
            }
            FogOfWarDestroy();
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = new Color(1, 1, 0, 0.75F);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        void LateUpdate() {

            if (!allowRotation) {
                transform.rotation = Quaternion.identity;
            }

            if (fogMat == null || r == null || profile == null) return;

            ComputeActiveProfile();

            DynamicFogManager globalManager = DynamicFogManager.instance;
            Light sun = globalManager.sun;
            if (sun != null) {
                sunDir = -sun.transform.forward;
                fogMat.SetVector(ShaderParams.SunDir, sunDir);
                dayLight = 1f + sunDir.y * 2f;
                if (dayLight < 0) dayLight = 0; else if (dayLight > 1f) dayLight = 1f;
                if (activeProfile.useSunColor) {
                    Color lightColor = sun.color * (sun.intensity * dayLight * activeProfile.brightness);
                    fogMat.SetVector(ShaderParams.LightColor, lightColor);
                } else {
                    fogMat.SetVector(ShaderParams.LightColor, Color.white);
                }
            }

            windAcum += Time.deltaTime * activeProfile.speed;
            windAcum %= 10000;
            fogMat.SetVector(ShaderParams.WindDirection, activeProfile.direction * windAcum);

            Bounds bounds = r.bounds;
            fogMat.SetVector(ShaderParams.BoundsCenter, bounds.center);
            fogMat.SetVector(ShaderParams.BoundsExtents, bounds.extents);

            UpdateNoise();

            if (enableFade || enableSubVolumes) {
                ApplyProfileSettings();
            }

            if (enableFogOfWar) {
                UpdateFogOfWar();
            }

            if (showBoundary) {
                if (fogDebugMat == null) {
                    fogDebugMat = new Material(Shader.Find("Hidden/DynamicFog2/DynamicFogDebug"));
                }
                if (debugMesh == null) {
                    MeshFilter mf = GetComponent<MeshFilter>();
                    if (mf != null) {
                        debugMesh = mf.sharedMesh;
                    }
                }
                Matrix4x4 m = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Graphics.DrawMesh(debugMesh, m, fogDebugMat, 0);
            }
        }


        void UpdateNoise() {
            if (activeProfile == null || noiseTex == null) return;

            if (rtTurbulence == null || rtTurbulence.width != noiseTex.width) {
                RenderTextureDescriptor desc = new RenderTextureDescriptor(noiseTex.width, noiseTex.height, RenderTextureFormat.ARGB32, 0);
                rtTurbulence = new RenderTexture(desc);
                rtTurbulence.wrapMode = TextureWrapMode.Repeat;
            }
            turbAcum += Time.deltaTime * activeProfile.speed;
            turbAcum %= 10000;
            turbulenceMat.SetFloat(ShaderParams.TurbulenceAmount, turbAcum);
            Graphics.Blit(noiseTex, rtTurbulence, turbulenceMat);

            fogMat.SetTexture(ShaderParams.MainTex, rtTurbulence);
        }

        public void UpdateMaterialProperties() {

            if (this == null || gameObject == null || !gameObject.activeInHierarchy) return;

            fadeDistance = Mathf.Max(0.1f, fadeDistance);

            if (r == null) {
                r = GetComponent<Renderer>();
            }

            if (profile == null) {
                if (fogMat == null && r != null) {
                    fogMat = new Material(Shader.Find("DynamicFog2/Empty"));
                    fogMat.hideFlags = HideFlags.DontSave;
                    r.sharedMaterial = fogMat;
                }
                return;
            }
            profile.onSettingsChanged -= UpdateMaterialProperties;
            profile.onSettingsChanged += UpdateMaterialProperties;

            // Subscribe to sub-volume profile changes
            if (subVolumes != null) {
                foreach (DynamicFogSubVolume subVol in subVolumes) {
                    if (subVol != null && subVol.profile != null) {
                        subVol.profile.onSettingsChanged -= UpdateMaterialProperties;
                        subVol.profile.onSettingsChanged += UpdateMaterialProperties;
                    }
                }
            }

            if (fogMat2D == null) {
                fogMat2D = new Material(Shader.Find("DynamicFog2/DynamicFog2DURP"));
                fogMat2D.hideFlags = HideFlags.DontSave;
            }
            fogMat = fogMat2D;
            if (turbulenceMat2D == null) {
                turbulenceMat2D = new Material(Shader.Find("DynamicFog2/Turbulence2D"));
            }
            turbulenceMat = turbulenceMat2D;

            if (r != null) {
                r.sharedMaterial = fogMat;
            }

            if (fogMat == null || profile == null) return;

            profile.ValidateSettings();

            lastControllerPosition.x = float.MaxValue;
            activeProfile = profile;

            ComputeActiveProfile();
            ApplyProfileSettings();
        }

        void ComputeActiveProfile() {

            if (maskEditorEnabled) alphaMultiplier = 0.85f;
            if (Application.isPlaying) {
                if (enableFade || enableSubVolumes) {
                    if (fadeController == null) {
                        Camera cam = Camera.main;
                        if (cam != null) {
                            fadeController = Camera.main.transform;
                        }
                    }
                    if (fadeController != null && (requireSubvolumeUpdate || lastControllerPosition != fadeController.position)) {

                        requireSubvolumeUpdate = false;
                        lastControllerPosition = fadeController.position;
                        activeProfile = profile;
                        alphaMultiplier = 1f;

                        // Self volume
                        if (enableFade) {
                            float t = ComputeVolumeFade(transform, fadeDistance);
                            alphaMultiplier *= t;
                        }

                        // Check sub-volumes
                        if (enableSubVolumes) {
                            int subVolumeCount = DynamicFogSubVolume.subVolumes.Count;
                            int allowedSubVolumesCount = subVolumes != null ? subVolumes.Count : 0;
                            for (int k = 0; k < subVolumeCount; k++) {
                                DynamicFogSubVolume subVolume = DynamicFogSubVolume.subVolumes[k];
                                if (subVolume == null || subVolume.profile == null) continue;
                                if (allowedSubVolumesCount > 0 && !subVolumes.Contains(subVolume)) continue;
                                float t = ComputeVolumeFade(subVolume.transform, subVolume.fadeDistance);
                                if (t > 0) {
                                    if (lerpProfile == null) {
                                        lerpProfile = ScriptableObject.CreateInstance<DynamicFogProfile>();
                                    }
                                    lerpProfile.Lerp(activeProfile, subVolume.profile, t);
                                    activeProfile = lerpProfile;
                                }
                            }
                        }
                    }
                }
            }

            if (activeProfile == null) {
                activeProfile = profile;
            }

            Color fogAlbedo = activeProfile.tintColor;
            Color secondColor = activeProfile.noiseColor;
            fogAlbedo.a *= alphaMultiplier;
            secondColor.a *= alphaMultiplier;

            fogMat.SetColor(ShaderParams.Color, fogAlbedo);
            fogMat.SetColor(ShaderParams.SecondColor, secondColor);
        }

        float ComputeVolumeFade(Transform transform, float fadeDistance) {
            Vector3 diff = transform.position - fadeController.position;
            diff.x = diff.x < 0 ? -diff.x : diff.x;
            diff.y = diff.y < 0 ? -diff.y : diff.y;
            diff.z = diff.z < 0 ? -diff.z : diff.z;
            Vector3 extents = transform.lossyScale * 0.5f;
            Vector3 gap = extents - diff;
            float minDiff = gap.x < gap.y ? gap.x : gap.y;
            minDiff = minDiff < gap.z ? minDiff : gap.z;
            fadeDistance += 0.0001f;
            float t = Mathf.Clamp01(minDiff / fadeDistance);
            return t;
        }


        void ApplyProfileSettings() {

            if (activeProfile == null) return;

            r.sortingLayerID = activeProfile.sortingLayerID;
            r.sortingOrder = activeProfile.sortingOrder;

            // update gradient texture
            int w = DynamicFogProfile.GRADIENT_TEXTURE_WIDTH;
            if (gradientTex == null) {
                gradientTex = new Texture2D(w, 1, SystemInfo.SupportsTextureFormat(TextureFormat.RGBAHalf) ? TextureFormat.RGBAHalf : TextureFormat.RGBA32, false);
                gradientTex.wrapMode = TextureWrapMode.Clamp;
            }
            if (activeProfile.gradientColors == null || activeProfile.gradientColors.Length != w) {
                activeProfile.ValidateSettings();
            }
            if (activeProfile.gradientColors.Length == w) {
                gradientTex.SetPixels(activeProfile.gradientColors);
            }
            gradientTex.Apply();
            fogMat.SetTexture(ShaderParams.GradientTex, gradientTex);
            fogMat.renderQueue = activeProfile.renderQueue;
            fogMat.SetVector(ShaderParams.Geom, new Vector4(activeProfile.baseAltitude, 0.001f + activeProfile.maxHeight, activeProfile.distanceMax, activeProfile.noiseDistanceAtten));
            fogMat.SetVector(ShaderParams.NoiseData, new Vector4(1f / (1f + activeProfile.scale * 200f), activeProfile.turbulence, activeProfile.shift, activeProfile.noiseColorBlend));
            fogMat.SetFloat(ShaderParams.LightDiffusionPower, activeProfile.lightDiffusionPower);
            fogMat.SetFloat(ShaderParams.LightDiffusionIntensity, activeProfile.lightDiffusionIntensity);
            fogMat.SetVector(ShaderParams.Density, new Vector3(1.001f - activeProfile.densityExponential, activeProfile.densityLinear));
            fogMat.SetFloat(ShaderParams.Dithering, activeProfile.dithering * 0.01f);

            if (activeProfile.boxProjection) {
                fogMat.EnableKeyword(ShaderParams.SKW_BOX_PROJECTION);
            } else {
                fogMat.DisableKeyword(ShaderParams.SKW_BOX_PROJECTION);
            }
            if (activeProfile.depthClip) {
                fogMat.EnableKeyword(ShaderParams.SKW_DEPTH_CLIP);
            } else {
                fogMat.DisableKeyword(ShaderParams.SKW_DEPTH_CLIP);
            }
            if (activeProfile.useXYZDistance) {
                fogMat.EnableKeyword(ShaderParams.SKW_3D_DISTANCE);
            } else {
                fogMat.DisableKeyword(ShaderParams.SKW_3D_DISTANCE);
            }
            if (enableFogOfWar) {
                fogMat.SetTexture(ShaderParams.FogOfWarTex, fogOfWarTexture);
                fogMat.SetVector(ShaderParams.FogOfWarCenter, fogOfWarCenter);
                fogMat.SetVector(ShaderParams.FogOfWarSize, fogOfWarSize);
                Vector3 ca = fogOfWarCenter - 0.5f * fogOfWarSize;
                fogMat.SetVector(ShaderParams.FogOfWarCenterAdjusted, new Vector3(ca.x / (fogOfWarSize.x + 0.0001f), 1f, ca.z / (fogOfWarSize.z + 0.0001f)));
                fogMat.EnableKeyword(ShaderParams.SKW_FOW);
            } else {
                fogMat.DisableKeyword(ShaderParams.SKW_FOW);
            }
        }

    }


}
