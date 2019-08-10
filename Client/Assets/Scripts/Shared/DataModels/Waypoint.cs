namespace Assets.Scripts.Network.Shared.DataModels
{
    public class Waypoint
    {
        public int Id { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public string Name { get; set; }

        public int RegionId { get; set; }

        public string RegionName { get; set; }

        public int RegionLevel { get; set; }
    }
}