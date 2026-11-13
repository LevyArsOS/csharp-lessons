namespace CSharpLessons.HotPathAllocations.Data
{
    public sealed class Entity
    {
        public int Id { get; }
        public float Energy { get; set; }
        public float PositionX { get; set; }
        public bool Active { get; set; }

        public Entity(int id, float energy, float positionX, bool active)
        {
            Id = id;
            Energy = energy;
            PositionX = positionX;
            Active = active;
        }
    }
}
