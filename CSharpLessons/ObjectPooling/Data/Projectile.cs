namespace CSharpLessons.ObjectPooling.Data
{
    public sealed class Projectile
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Vx { get; private set; }
        public float Vy { get; private set; }
        public int RemainingFrames { get; private set; }
        public bool Active { get; private set; }

        public void Activate(float x, float y, float vx, float vy, int lifetimeFrames)
        {
            X = x;
            Y = y;
            Vx = vx;
            Vy = vy;
            RemainingFrames = lifetimeFrames;
            Active = true;
        }

        public bool Step(float dt)
        {
            if (!Active)
            {
                return false;
            }

            X += Vx * dt;
            Y += Vy * dt;
            RemainingFrames--;
            if (RemainingFrames <= 0)
            {
                Active = false;
                return false;
            }

            return true;
        }

        public void Reset()
        {
            X = 0f;
            Y = 0f;
            Vx = 0f;
            Vy = 0f;
            RemainingFrames = 0;
            Active = false;
        }
    }
}
