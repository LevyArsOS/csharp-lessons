namespace CSharpLessons.DataLocality.Data
{
    public struct ParticleStruct
    {
        public float X;
        public float Y;
        public float Z;
        public float Vx;
        public float Vy;
        public float Vz;
        public float Life;

        public ParticleStruct(float x, float y, float z, float vx, float vy, float vz, float life)
        {
            X = x;
            Y = y;
            Z = z;
            Vx = vx;
            Vy = vy;
            Vz = vz;
            Life = life;
        }

        public void Step(float dt)
        {
            X += Vx * dt;
            Y += Vy * dt;
            Z += Vz * dt;
            Life -= dt;
        }
    }
}
