using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpLessons.HotPathAllocations.Data
{
    public static class HotPathQueries
    {
        public static List<int> QueryActiveNaive(Entity[] entities, float minEnergy)
        {
            ArgumentNullException.ThrowIfNull(entities);
            return entities
                .Where(e => e.Active && e.Energy >= minEnergy)
                .Select(e => e.Id)
                .ToList();
        }

        public static void QueryActiveNonAlloc(Entity[] entities, float minEnergy, List<int> results)
        {
            ArgumentNullException.ThrowIfNull(entities);
            ArgumentNullException.ThrowIfNull(results);

            results.Clear();
            for (int i = 0; i < entities.Length; i++)
            {
                Entity e = entities[i];
                if (e.Active && e.Energy >= minEnergy)
                {
                    results.Add(e.Id);
                }
            }
        }

        public static float RunNaiveLinqFrameLoop(EntityWorld world, int frames, float minEnergy)
        {
            ArgumentNullException.ThrowIfNull(world);
            float checksum = 0f;

            for (int f = 0; f < frames; f++)
            {
                List<int> activeIds = QueryActiveNaive(world.Entities, minEnergy);
                for (int i = 0; i < activeIds.Count; i++)
                {
                    Entity e = world.Entities[activeIds[i]];
                    checksum += e.PositionX + e.Energy * 0.01f;
                }
            }

            return checksum;
        }

        public static float RunZeroAllocFrameLoop(EntityWorld world, int frames, float minEnergy, List<int> scratchIds)
        {
            ArgumentNullException.ThrowIfNull(world);
            ArgumentNullException.ThrowIfNull(scratchIds);

            float checksum = 0f;
            Entity[] entities = world.Entities;

            for (int f = 0; f < frames; f++)
            {
                QueryActiveNonAlloc(entities, minEnergy, scratchIds);
                int count = scratchIds.Count;
                for (int i = 0; i < count; i++)
                {
                    Entity e = entities[scratchIds[i]];
                    checksum += e.PositionX + e.Energy * 0.01f;
                }
            }

            return checksum;
        }
    }
}
