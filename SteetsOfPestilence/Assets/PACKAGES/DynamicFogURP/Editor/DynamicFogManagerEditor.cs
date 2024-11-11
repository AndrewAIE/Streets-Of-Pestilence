using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

namespace DynamicFogAndMist2 {

    [CustomEditor(typeof(DynamicFogManager))]
    public class DynamicFogManagerEditor : Editor {

        SerializedProperty mainCamera, sun, flipDepthTexture;

        private void OnEnable() {
            mainCamera = serializedObject.FindProperty("mainCamera");
            sun = serializedObject.FindProperty("sun");
            flipDepthTexture = serializedObject.FindProperty("flipDepthTexture");
        }


        public override void OnInspectorGUI() {

            EditorGUILayout.Separator();

            UniversalRenderPipelineAsset pipe = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
            if (pipe == null) {
                EditorGUILayout.HelpBox("Please assign the Universal Rendering Pipeline asset (go to Project Settings -> Graphics). You can use the UniversalRenderPipelineAsset included in the demo folder or create a new pipeline asset (check documentation for step by step setup).", MessageType.Error);
                return;
            }

            if (QualitySettings.renderPipeline != null) {
                pipe = QualitySettings.renderPipeline as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
            }

            if (!pipe.supportsCameraDepthTexture) {
                EditorGUILayout.HelpBox("Depth Texture option is required in Universal Rendering Pipeline asset!", MessageType.Error);
                if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                    Selection.activeObject = pipe;
                }
                EditorGUILayout.Separator();
                GUI.enabled = false;
            }

            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(mainCamera);
            EditorGUILayout.PropertyField(sun);
            EditorGUILayout.PropertyField(flipDepthTexture);

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

        }
    }

}