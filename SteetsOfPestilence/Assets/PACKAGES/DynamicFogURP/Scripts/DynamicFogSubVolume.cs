using System.Collections.Generic;
using UnityEngine;

namespace DynamicFogAndMist2 {

    public class DynamicFogSubVolume : MonoBehaviour {

        public DynamicFogProfile profile;
        public float fadeDistance = 1f;

        public static readonly List<DynamicFogSubVolume> subVolumes = new List<DynamicFogSubVolume>();

        void OnDrawGizmos() {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }

        void OnEnable() {
            if (!subVolumes.Contains(this)) {
                subVolumes.Add(this);
            }
        }

        void OnDisable() {
            if (subVolumes.Contains(this)) {
                subVolumes.Remove(this);
            }
        }

    }

}