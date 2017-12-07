namespace YeggQuest.NS_Spline
{
    public enum SplineMovementType
    {
        Forward, Backward, PingPong
    }

    public enum SplineMovementSmoothing
    {
        Off, WholeSpline, BetweenNodes
    }

    public enum SplineInputBehavior
    {
        ControlSpeed, ControlDirection, ControlDirectionClamped
    }
}