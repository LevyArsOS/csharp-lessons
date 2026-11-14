using System;
using System.Numerics;

namespace CSharpLessons.SoaVsAos.Data
{
    public static class ParticleIntegrator
    {
        public static ParticleAos[] CreateAos(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
            }

            var particles = new ParticleAos[count];
            for (int i = 0; i < count; i++)
            {
                float f = (i % 200) * 0.1f;
                particles[i] = new ParticleAos
                {
                    X = f,
                    Y = f * 0.5f,
                    Vx = 1.5f,
                    Vy = -0.5f,
                    Life = 10f,
                    R = 1f,
                    G = 0.8f,
                    B = 0.2f,
                    A = 1f,
                    Flags = 1
                };
            }
            return particles;
        }

        public static ParticleSoa CreateSoa(int count)
        {
            var soa = new ParticleSoa(count);
            for (int i = 0; i < count; i++)
            {
                float f = (i % 200) * 0.1f;
                soa.PosX[i] = f;
                soa.PosY[i] = f * 0.5f;
                soa.VelX[i] = 1.5f;
                soa.VelY[i] = -0.5f;
                soa.Life[i] = 10f;
                soa.Flags[i] = 1;
            }
            return soa;
        }

        public static double IntegrateAos(ParticleAos[] particles, float dt, int frames)
        {
            ArgumentNullException.ThrowIfNull(particles);

            for (int f = 0; f < frames; f++)
            {
                for (int i = 0; i < particles.Length; i++)
                {
                    ref ParticleAos p = ref particles[i];
                    p.X += p.Vx * dt;
                    p.Y += p.Vy * dt;
                    p.Life -= dt;
                }
            }

            double sum = 0.0;
            for (int i = 0; i < particles.Length; i++)
            {
                sum += particles[i].X + particles[i].Y + particles[i].Life;
            }
            return sum;
        }

        public static double IntegrateSoaScalar(ParticleSoa soa, float dt, int frames)
        {
            ArgumentNullException.ThrowIfNull(soa);
            int n = soa.Count;
            float[] px = soa.PosX;
            float[] py = soa.PosY;
            float[] vx = soa.VelX;
            float[] vy = soa.VelY;
            float[] life = soa.Life;

            for (int f = 0; f < frames; f++)
            {
                for (int i = 0; i < n; i++)
                {
                    px[i] += vx[i] * dt;
                    py[i] += vy[i] * dt;
                    life[i] -= dt;
                }
            }

            double sum = 0.0;
            for (int i = 0; i < n; i++)
            {
                sum += px[i] + py[i] + life[i];
            }
            return sum;
        }

        public static double IntegrateSoaSimd(ParticleSoa soa, float dt, int frames)
        {
            ArgumentNullException.ThrowIfNull(soa);
            int n = soa.Count;
            float[] px = soa.PosX;
            float[] py = soa.PosY;
            float[] vx = soa.VelX;
            float[] vy = soa.VelY;
            float[] life = soa.Life;

            int width = Vector<float>.Count;
            int simdLimit = n - (n % width);
            var dtVec = new Vector<float>(dt);

            for (int f = 0; f < frames; f++)
            {
                int i = 0;
                for (; i < simdLimit; i += width)
                {
                    var xVec = new Vector<float>(px, i) + new Vector<float>(vx, i) * dtVec;
                    var yVec = new Vector<float>(py, i) + new Vector<float>(vy, i) * dtVec;
                    var lVec = new Vector<float>(life, i) - dtVec;
                    xVec.CopyTo(px, i);
                    yVec.CopyTo(py, i);
                    lVec.CopyTo(life, i);
                }
                for (; i < n; i++)
                {
                    px[i] += vx[i] * dt;
                    py[i] += vy[i] * dt;
                    life[i] -= dt;
                }
            }

            double sum = 0.0;
            for (int i = 0; i < n; i++)
            {
                sum += px[i] + py[i] + life[i];
            }
            return sum;
        }
    }
}
