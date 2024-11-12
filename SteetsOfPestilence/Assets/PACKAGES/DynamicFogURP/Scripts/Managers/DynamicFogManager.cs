using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace DynamicFogAndMist2 {

    [ExecuteInEditMode]
    public class DynamicFogManager : MonoBehaviour, IDynamicFogManager {

        public string managerName {
            get {
                return "Dynamic Fog Manager";
            }
        }

        static DynamicFogManager _instance;

        public Camera mainCamera;
        public Light sun;
        [Tooltip("Flip depth texture. Use only as a workaround to a bug in URP if the depth shows inverted in GameView. Alternatively you can enable MSAA or HDR instead of using this option.")]
        public bool flipDepthTexture;

        const string SKW_FLIP_DEPTH_TEXTURE = "DF2_FLIP_DEPTH_TEXTURE";

        public static DynamicFogManager instance {
            get {
                if (_instance == null) {
                    _instance = Tools.CheckMainManager();
                }
                return _instance;
            }
        }

        void OnEnable() {
            SetupCamera();
            SetupLights();
            SetupDepthPrePass();
        }

        void OnValidate() {
            SetupDepthPrePass();
        }

        void SetupCamera() {
            Tools.CheckCamera(ref mainCamera);
            if (mainCamera != null) {
                mainCamera.depthTextureMode |= DepthTextureMode.Depth;
            }
        }

        void SetupLights() {
            Light[] lights = FindObjectsOfType<Light>();
            for (int k = 0; k < lights.Length; k++) {
                Light l = lights[k];
                if (l.type == LightType.Directional) {
                    if (sun == null) {
                        sun = l;
                    }
                    return;
                }
            }
        }

        void SetupDepthPrePass() {
            Shader.SetGlobalInt(SKW_FLIP_DEPTH_TEXTURE, flipDepthTexture ? 1 : 0);
        }

        /// <summary>
        /// Creates a new fog volume
        /// </summary>
        public static GameObject CreateFogVolume(string name) {
            GameObject go = Resources.Load<GameObject>("DynamicFog/Prefabs/FogVolume2D");
            go = Instantiate(go);
            go.name = name;
            return go;
        }


        /// <summary>
        /// Creates a new fog sub-volume
        /// </summary>
        public static GameObject CreateFogSubVolume(string name) {
            GameObject go = Resources.Load<GameObject>("DynamicFog/Prefabs/FogSubVolume");
            go = Instantiate(go);
            go.name = name;
            return go;
        }

    }
}