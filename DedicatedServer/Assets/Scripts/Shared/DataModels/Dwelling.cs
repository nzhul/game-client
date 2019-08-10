using Assets.Scripts.Network.Shared.DataModels;

namespace Assets.Scripts.Shared.DataModels
{
    public class Dwelling
    {
        public int Id { get; set; }

        public int? OwnerId { get; set; }

        public DwellingType Type { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public string OccupiedTilesString { get; set; }

        public string Name { get; set; }
    }
}
