using System;
using System.Collections.Generic;

namespace CSharpLessons.DataLocality.Data
{
    public static class ParticleUpdateSimulator
    {
        public static List<ParticleClass> CreateClassList(int count, bool scatterHeap = true)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
            }

            var list = new List<ParticleClass>(count);
            for (int i = 0; i < count; i++)
            {
                float f = (i % 100) * 0.1f;
                list.Add(new ParticleClass(f, f * 0.5f, 0f, 1.5f, -0.5f, 0.25f, 10f));
            }

            if (scatterHeap && list.Count > 1)
            {
                var rng = new Random(42);
                for (int i = list.Count - 1; i > 0; i--)
                {
                    int j = rng.Next(i + 1);
                    (list[i], list[j]) = (list[j], list[i]);
                }
            }

            return list;
        }

        public static ParticleStruct[] CreateStructArray(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
            }

            var array = new ParticleStruct[count];
            for (int i = 0; i < count; i++)
            {
                float f = (i % 100) * 0.1f;
                array[i] = new ParticleStruct(f, f * 0.5f, 0f, 1.5f, -0.5f, 0.25f, 10f);
            }

            return array;
        }

        public static double UpdateClassList(List<ParticleClass> particles, float dt, int frames)
        {
            ArgumentNullException.ThrowIfNull(particles);

            for (int f = 0; f < frames; f++)
            {
                for (int i = 0; i < particles.Count; i++)
                {
                    particles[i].Step(dt);
                }
            }

            double sum = 0.0;
            for (int i = 0; i < particles.Count; i++)
            {
                ParticleClass p = particles[i];
                sum += p.X + p.Y + p.Z + p.Life;
            }
            return sum;
        }

        public static double UpdateStructArray(ParticleStruct[] particles, float dt, int frames)
        {
            ArgumentNullException.ThrowIfNull(particles);

            for (int f = 0; f < frames; f++)
            {
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].Step(dt);
                }
            }

            double sum = 0.0;
            for (int i = 0; i < particles.Length; i++)
            {
                ref readonly ParticleStruct p = ref particles[i];
                sum += p.X + p.Y + p.Z + p.Life;
            }
            return sum;
        }
    }
}
