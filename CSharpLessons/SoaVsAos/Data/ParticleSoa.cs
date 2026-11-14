using System;

namespace CSharpLessons.SoaVsAos.Data
{
    public sealed class ParticleSoa
    {
        public int Count { get; }
        public float[] PosX { get; }
        public float[] PosY { get; }
        public float[] VelX { get; }
        public float[] VelY { get; }
        public float[] Life { get; }
        public int[] Flags { get; }

        public ParticleSoa(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
            }

            Count = count;
            PosX = new float[count];
            PosY = new float[count];
            VelX = new float[count];
            VelY = new float[count];
            Life = new float[count];
            Flags = new int[count];
        }
    }
}
