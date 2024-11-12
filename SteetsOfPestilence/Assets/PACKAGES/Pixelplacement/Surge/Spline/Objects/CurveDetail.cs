/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Holds details of a spline's curve.
/// 
/// </summary>

namespace Pixelplacement
{
    public struct CurveDetail
    {
        //Public Variables:
        public int currentCurve;
        public float currentCurvePercentage;

        //Constructor:
        public CurveDetail (int currentCurve, float currentCurvePercentage)
        {
            this.currentCurve = currentCurve;
            this.currentCurvePercentage = currentCurvePercentage;
        }
    }
}