namespace CSharpLessons.StructVsClass.Data
{
    public static class EntityTransformSimulator
    {
        public static (Vec2Class[] Positions, Vec2Class[] Velocities) CreateClassEntities(int count)
        {
            var positions = new Vec2Class[count];
            var velocities = new Vec2Class[count];
            for (int i = 0; i < count; i++)
            {
                positions[i] = new Vec2Class(i * 1.5f, i * 0.5f);
                velocities[i] = new Vec2Class(0.1f * (i % 7), 0.2f * (i % 5));
            }

            return (positions, velocities);
        }

        public static (Vec2Struct[] Positions, Vec2Struct[] Velocities) CreateStructEntities(int count)
        {
            var positions = new Vec2Struct[count];
            var velocities = new Vec2Struct[count];
            for (int i = 0; i < count; i++)
            {
                positions[i] = new Vec2Struct(i * 1.5f, i * 0.5f);
                velocities[i] = new Vec2Struct(0.1f * (i % 7), 0.2f * (i % 5));
            }

            return (positions, velocities);
        }

        public static void IntegrateClass(Vec2Class[] positions, Vec2Class[] velocities, float dt)
        {
            int length = Math.Min(positions.Length, velocities.Length);
            for (int i = 0; i < length; i++)
            {
                positions[i] = positions[i] + (velocities[i] * dt);
            }
        }

        public static void IntegrateStruct(Vec2Struct[] positions, Vec2Struct[] velocities, float dt)
        {
            int length = Math.Min(positions.Length, velocities.Length);
            for (int i = 0; i < length; i++)
            {
                positions[i] = positions[i] + (velocities[i] * dt);
            }
        }

        public static Vec2Class SumClass(Vec2Class[] vectors)
        {
            var total = new Vec2Class(0f, 0f);
            for (int i = 0; i < vectors.Length; i++)
            {
                total = total + vectors[i];
            }

            return total;
        }

        public static Vec2Struct SumStruct(Vec2Struct[] vectors)
        {
            var total = new Vec2Struct(0f, 0f);
            for (int i = 0; i < vectors.Length; i++)
            {
                total = total + vectors[i];
            }

            return total;
        }
    }
}
