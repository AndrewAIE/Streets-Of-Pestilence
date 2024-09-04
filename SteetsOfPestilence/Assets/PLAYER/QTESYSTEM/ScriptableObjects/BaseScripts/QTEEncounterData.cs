using System.Collections.Generic;
using UnityEngine;

namespace QTESystem
{
    [CreateAssetMenu(fileName = "QuickTimeStream", menuName = "Quick Time Event/Quick Time Encounter Data", order = 2)]
    public class QTEEncounterData : ScriptableObject
    {
        // Start is called before the first frame update
        public List<QTEStreamData> NeutralStreamData;
        public List<QTEStreamData> OffensiveStreamData;
        public List<QTEStreamData> DefensiveStreamData;
    }
}
