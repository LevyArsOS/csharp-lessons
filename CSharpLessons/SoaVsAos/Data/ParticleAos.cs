namespace CSharpLessons.SoaVsAos.Data
{
    public struct ParticleAos
    {
        // Hot physics fields (20 bytes)
        public float X;
        public float Y;
        public float Vx;
        public float Vy;
        public float Life;
        // Cold rendering/metadata fields (44 bytes -> 64B struct)
        public float R;
        public float G;
        public float B;
        public float A;
        public int Flags;
        public long MetadataA;
        public long MetadataB;
        public long MetadataC;
        public int Padding;
    }
}
