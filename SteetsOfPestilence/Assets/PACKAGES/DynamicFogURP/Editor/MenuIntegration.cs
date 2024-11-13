using UnityEngine;
using UnityEditor;

namespace DynamicFogAndMist2 {

    public class DynamicFog2EditorIntegration : MonoBehaviour {

        [MenuItem("GameObject/Effects/Dynamic Fog 2/Manager", false, 100)]
        public static void CreateManager(MenuCommand menuCommand) {
            DynamicFogManager fog2 = Tools.CheckMainManager();
            Selection.activeObject = fog2.gameObject;
        }


        [MenuItem("GameObject/Effects/Dynamic Fog 2/Fog Volume", false, 120)]
        public static void CreateFogVolume(MenuCommand menuCommand) {
            GameObject go = DynamicFogManager.CreateFogVolume("Dynamic Fog Volume");
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            go.transform.position = Vector3.zero;
            go.transform.localScale = new Vector3(5000, 5000, 5000);
            Undo.RegisterCreatedObjectUndo(go, "Create Dynamic Fog Volume");
            Selection.activeObject = go;
        }

        [MenuItem("GameObject/Effects/Dynamic Fog 2/Fog Sub-Volume", false, 121)]
        public static void CreateFogSubVolume(MenuCommand menuCommand) {
            GameObject go = DynamicFogManager.CreateFogSubVolume("Dynamic Fog Sub-Volume");
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            go.transform.position = Vector3.zero;
            go.transform.localScale = new Vector3(20, 10, 20);
            Undo.RegisterCreatedObjectUndo(go, "Create Dynamic Fog Sub-Volume");
            Selection.activeObject = go;
        }


    }

}