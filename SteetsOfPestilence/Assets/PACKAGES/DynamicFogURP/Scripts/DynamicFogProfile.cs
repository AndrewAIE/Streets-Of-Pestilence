using System;
using UnityEngine;

namespace DynamicFogAndMist2 {

    public delegate void OnSettingsChanged();

    [CreateAssetMenu(menuName = "Dynamic Fog \x8B& Mist/Fog Profile", fileName = "DynamicFogProfile", order = 1001)]
    public class DynamicFogProfile : ScriptableObject {

        [Header("Appearance")]
        public float baseAltitude;
        [Tooltip("If true, the 3D distance will be used to compute fog accumulation. If false, only X/Z plane distance will be used.")]
        public bool useXYZDistance = true;
        public float densityLinear = 0.55f;
        public float densityExponential = 0.94f;
        public float maxHeight = 10000;
        public float distanceMax = 100000;
        public Color tintColor = new Color32(195, 195, 200, 255);
        public bool useVerticalGradient = true;
        [GradientUsage(hdr: true, ColorSpace.Linear)] public Gradient verticalGradient;
        public bool boxProjection;
        [Tooltip("Hides fog behing opaque objects")]
        public bool depthClip;

        [Header("Noise")]
        [Range(0, 1)]
        public float noiseColorBlend = 0.1f;
        public Color noiseColor = new Color32(175, 180, 182, 255);
        [Tooltip("Horizontal scale of the noise/turbulences")]
        public float scale = 50f;
        [Tooltip("Noise shift")]
        [Range(-1, 1)]
        public float shift;
        [Tooltip("Attenuates noise with distance")]
        [Range(0,1f)]
        public float noiseDistanceAtten = 1f;

        [Header("Wind")]
        public float turbulence = 10f;
        public float speed = 0.73f;
        [Tooltip("Wind direction")]
        public Vector3 direction = new Vector3(0.02f, 0, 0);

        [Header("Directional Light")]
        [Range(0, 128)] public float lightDiffusionPower = 32;
        [Range(0, 1)] public float lightDiffusionIntensity = 0.4f;
        public float brightness = 1f;
        [Tooltip("If enabled, the fog color will be tinted with the directional light color.")]
        public bool useSunColor = true;

        [Header("Rendering")]
        [Range(0, 2)] public float dithering = 1f;
        [Tooltip("The render queue for this renderer. By default, all transparent objects use a render queue of 3000+. Use a lower value to render before all transparent objects.")]
        public int renderQueue = 3099;
        [Tooltip("Optional sorting layer Id (number) for this renderer. By default 0. Usually used to control the order with other transparent renderers, like Sprite Renderer.")]
        public int sortingLayerID;
        [Tooltip("Optional sorting order for this renderer. Used to control the order with other transparent renderers, like Sprite Renderer.")]
        public int sortingOrder;

        public event OnSettingsChanged onSettingsChanged;

        [NonSerialized]
        public Color[] gradientColors;
        public const int GRADIENT_TEXTURE_WIDTH = 256;

        private void OnEnable() {
            ValidateSettings();
        }

        private void OnValidate() {
            ValidateSettings();
            if (onSettingsChanged != null) {
                onSettingsChanged();
            }
        }

        public void ValidateSettings() {
            if (verticalGradient == null) {
                verticalGradient = new Gradient();
                GradientColorKey[] keys = new GradientColorKey[2];
                keys[0].color = tintColor;
                keys[0].time = 0;
                keys[1].color = tintColor;
                keys[1].time = 0;
                verticalGradient.colorKeys = keys;
            }
            // evaluate gradient colors
            if (gradientColors == null || gradientColors.Length != GRADIENT_TEXTURE_WIDTH) {
                gradientColors = new Color[GRADIENT_TEXTURE_WIDTH];
            }
            if (useVerticalGradient) {
                for (int k = 0; k < GRADIENT_TEXTURE_WIDTH; k++) {
                    float t = (float)k / GRADIENT_TEXTURE_WIDTH;
                    gradientColors[k] = verticalGradient.Evaluate(t);
                }
            } else {
                Color white = Color.white;
                for (int k = 0; k < GRADIENT_TEXTURE_WIDTH; k++) {
                    gradientColors[k] = white;
                }
            }

            densityLinear = Mathf.Max(0, densityLinear);
            densityExponential = Mathf.Max(0, densityExponential);
            maxHeight = Mathf.Max(0, maxHeight);
            distanceMax = Mathf.Max(0, distanceMax);
            scale = Mathf.Max(0f, scale);
            turbulence = Mathf.Max(0f, turbulence);
            speed = Mathf.Max(0f, speed);
            lightDiffusionPower = Mathf.Max(0.1f, lightDiffusionPower);
            lightDiffusionIntensity = Mathf.Max(0, lightDiffusionIntensity);
            brightness = Mathf.Max(0f, brightness);
        }

        public void Lerp(DynamicFogProfile p1, DynamicFogProfile p2, float t) {
            float t0 = 1f - t;
            baseAltitude = p1.baseAltitude * t0 + p2.baseAltitude * t;
            densityLinear = p1.densityLinear * t0 + p2.densityLinear * t;
            densityExponential = p1.densityExponential * t0 + p2.densityExponential * t;
            maxHeight = p1.maxHeight * t0 + p2.maxHeight * t;
            distanceMax = p1.distanceMax * t0 + p2.distanceMax * t;
            tintColor = p1.tintColor * t0 + p2.tintColor * t;
            verticalGradient = p1.verticalGradient;
            // blend gradient colors
            if (p1.gradientColors != null && p2.gradientColors != null && p1.gradientColors.Length == GRADIENT_TEXTURE_WIDTH && p2.gradientColors.Length == GRADIENT_TEXTURE_WIDTH) {
                for (int k=0;k<GRADIENT_TEXTURE_WIDTH;k++) {
                    gradientColors[k] = p1.gradientColors[k] * t0 + p2.gradientColors[k] * t;
                }
            }
            boxProjection = t < 0.5f ? p1.boxProjection : p2.boxProjection;
            noiseColorBlend = p1.noiseColorBlend * t0 + p2.noiseColorBlend * t;
            noiseColor = p1.noiseColor * t0 + p2.noiseColor * t;
            scale = p1.scale * t0 + p2.scale * t;
            shift = p1.shift * t0 + p2.shift * t;
            turbulence = p1.turbulence * t0 + p2.turbulence * t;
            speed = p1.speed * t0 + p2.speed * t;
            direction = Vector3.Lerp(p1.direction, p2.direction, t);
            lightDiffusionPower = p1.lightDiffusionPower * t0 + p2.lightDiffusionPower * t;
            lightDiffusionIntensity = p1.lightDiffusionIntensity * t0 + p2.lightDiffusionIntensity * t;
            brightness = p1.brightness * t0 + p2.brightness * t;
            dithering = p1.dithering * t0 + p2.dithering * t;
            renderQueue = t < 0.5f ? p1.renderQueue : p2.renderQueue;
            sortingLayerID = t < 0.5f ? p1.sortingLayerID : p2.sortingLayerID;
            sortingOrder = t < 0.5f ? p1.sortingOrder : p2.sortingOrder;
    }

    }
}