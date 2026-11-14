namespace CSharpLessons.StructVsClass.Data
{
    public sealed class Vec2Class
    {
        public float X { get; }
        public float Y { get; }

        public Vec2Class(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vec2Class Add(Vec2Class other) => new Vec2Class(X + other.X, Y + other.Y);

        public Vec2Class Scale(float factor) => new Vec2Class(X * factor, Y * factor);

        public float LengthSquared() => (X * X) + (Y * Y);

        public static Vec2Class operator +(Vec2Class a, Vec2Class b) => a.Add(b);

        public static Vec2Class operator *(Vec2Class a, float factor) => a.Scale(factor);
    }
}
