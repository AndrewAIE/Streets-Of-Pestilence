using UnityEngine;
using DynamicFogAndMist2;

namespace DynamicFogAndMist2_Demos {

    public class FogOfWarPainter : MonoBehaviour {

        public float clearRadius = 5f;
        public float clearDuration = 0f;
        public float restoreDelay = 10f;
        public float restoreDuration = 2f;
        [Range(0,1)]
        public float borderSmoothness = 0.2f;

        DynamicFog fog;

        void Start() {
            fog = GetComponent<DynamicFog>();
        }

        void Update() {
            if (Input.GetMouseButton(0)) {
                // Raycast to terrain and clear fog around hit position
                Vector3 mousePos = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                RaycastHit terrainHit;
                if (Physics.Raycast(ray, out terrainHit)) {
                    fog.SetFogOfWarAlpha(terrainHit.point, clearRadius, 0, false, clearDuration, borderSmoothness, restoreDelay, restoreDuration);
                }
            }
        }

        public void RestoreFog() {
            fog.ResetFogOfWar();
        }
    }

}