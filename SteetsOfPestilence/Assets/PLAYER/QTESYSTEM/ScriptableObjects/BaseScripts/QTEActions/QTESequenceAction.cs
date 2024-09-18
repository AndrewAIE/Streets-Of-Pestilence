using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QTESystem
{
    [CreateAssetMenu(fileName = "NewPressAction", menuName = "Quick Time Event/New Quick Time Action/Press Action")]
    public class QTESequenceAction : QTEAction
    {
        public override void CheckInput(InputAction.CallbackContext _context)
        {
            throw new System.NotImplementedException();
        }

        public override void DisplayUpdate()
        {
            throw new System.NotImplementedException();
        }       

        public override void SetTargetInputs(QTEInputs _qteInputControl)
        {
            throw new System.NotImplementedException();
        }

        protected override ActionState onUpdate()
        {
            throw new System.NotImplementedException();
        }
    }
}

