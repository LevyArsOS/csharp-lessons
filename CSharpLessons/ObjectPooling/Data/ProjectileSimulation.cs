using System.Collections.Generic;

namespace CSharpLessons.ObjectPooling.Data
{
    public static class ProjectileSimulation
    {
        public static float RunNaive(int batchSize, int batches, int lifetimeFrames, float dt)
        {
            var active = new List<Projectile>(batchSize);
            float checksum = 0f;

            for (int b = 0; b < batches; b++)
            {
                for (int i = 0; i < batchSize; i++)
                {
                    var p = new Projectile();
                    p.Activate(i * 0.5f, 0f, 10f, 5f, lifetimeFrames);
                    active.Add(p);
                }

                for (int frame = 0; frame < lifetimeFrames; frame++)
                {
                    for (int i = active.Count - 1; i >= 0; i--)
                    {
                        var p = active[i];
                        if (!p.Step(dt))
                        {
                            checksum += p.X + p.Y;
                            int last = active.Count - 1;
                            active[i] = active[last];
                            active.RemoveAt(last);
                        }
                    }
                }
            }

            return checksum;
        }

        public static float RunPooled(int batchSize, int batches, int lifetimeFrames, float dt, IObjectPool<Projectile> pool, List<Projectile> activeScratch)
        {
            activeScratch.Clear();
            float checksum = 0f;

            for (int b = 0; b < batches; b++)
            {
                for (int i = 0; i < batchSize; i++)
                {
                    var p = pool.Rent();
                    p.Activate(i * 0.5f, 0f, 10f, 5f, lifetimeFrames);
                    activeScratch.Add(p);
                }

                for (int frame = 0; frame < lifetimeFrames; frame++)
                {
                    for (int i = activeScratch.Count - 1; i >= 0; i--)
                    {
                        var p = activeScratch[i];
                        if (!p.Step(dt))
                        {
                            checksum += p.X + p.Y;
                            int last = activeScratch.Count - 1;
                            activeScratch[i] = activeScratch[last];
                            activeScratch.RemoveAt(last);
                            pool.Return(p);
                        }
                    }
                }
            }

            return checksum;
        }
    }
}
