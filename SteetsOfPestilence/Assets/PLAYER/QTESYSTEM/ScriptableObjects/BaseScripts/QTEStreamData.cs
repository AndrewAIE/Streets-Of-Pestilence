using System.Collections.Generic;
using UnityEngine;

namespace QTESystem
{
    [CreateAssetMenu(fileName = "QuickTimeStream", menuName = "Quick Time Event/Quick Time Stream", order = 1)]
    public class QTEStreamData : ScriptableObject
    {
        public List<QTEAction> Actions;
        public float ActionTimer;
        public float BetweenActionTimer;
        public float BeginningOfStreamPause;
        public float EndOfStreamPause;        
    }
}
