using System;

namespace CSharpLessons.HotPathAllocations.Data
{
    public sealed class EntityWorld
    {
        public Entity[] Entities { get; }

        public EntityWorld(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
            }

            Entities = new Entity[count];
            for (int i = 0; i < count; i++)
            {
                bool active = (i % 5) != 0; // 80% active
                float energy = 10f + (i % 25);
                Entities[i] = new Entity(i, energy, i * 0.1f, active);
            }
        }
    }
}
