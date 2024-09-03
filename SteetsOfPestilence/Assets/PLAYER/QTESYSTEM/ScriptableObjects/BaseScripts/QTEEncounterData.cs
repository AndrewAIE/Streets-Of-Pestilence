using UnityEngine;

namespace QTESystem
{
    [CreateAssetMenu(fileName = "QuickTimeStream", menuName = "Quick Time Event/Quick Time Encounter Data", order = 2)]
    public class QTEEncounterData : ScriptableObject
    {
        // Start is called before the first frame update
        public QTEStreamData NeutralStreamData;
        public QTEStreamData OffensiveStreamData;
        public QTEStreamData DefensiveStreamData;
    }
}
