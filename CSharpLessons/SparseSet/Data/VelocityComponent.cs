namespace CSharpLessons.SparseSet.Data;

/// <summary>
/// Per-entity velocity component used to compare Dictionary vs. SparseSet iteration and churn.
/// </summary>
public struct VelocityComponent
{
    public float X;
    public float Y;
    public float Z;
    public float Speed;

    public VelocityComponent(float x, float y, float z, float speed)
    {
        X = x;
        Y = y;
        Z = z;
        Speed = speed;
    }
}
