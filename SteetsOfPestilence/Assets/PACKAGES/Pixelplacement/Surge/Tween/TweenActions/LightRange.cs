﻿/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// </summary>

using UnityEngine;
using System;

namespace Pixelplacement.TweenSystem
{
    class LightRange : TweenBase
    {
        //Public Properties:
        public float EndValue {get; private set;}

        //Private Variables:
        Light _target;
        float _start;

        //Constructor:
        public LightRange (Light target, float endValue, float duration, float delay, bool obeyTimescale, AnimationCurve curve, Tween.LoopType loop, Action startCallback, Action completeCallback)
        {
            //set essential properties:
            SetEssentials (Tween.TweenType.LightRange, target.GetInstanceID (), duration, delay, obeyTimescale, curve, loop, startCallback, completeCallback);

            //catalog custom properties:
            _target = target;
            EndValue = endValue;
        }

        //Processes:
        protected override bool SetStartValue ()
        {
            if (_target == null) return false;
            _start = _target.range;
            return true;
        }

        protected override void Operation (float percentage)
        {
            float calculatedValue = TweenUtilities.LinearInterpolate (_start, EndValue, percentage);
            _target.range = calculatedValue;
        }

        //Loops:
        public override void Loop ()
        {
            ResetStartTime ();
            _target.range = _start;
        }

        public override void PingPong ()
        {
            ResetStartTime ();
            _target.range = EndValue;
            EndValue = _start;
            _start = _target.range;
        }
    }
}