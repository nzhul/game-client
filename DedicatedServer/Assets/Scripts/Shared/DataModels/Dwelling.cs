using Assets.Scripts.Network.Shared.DataModels;

namespace Assets.Scripts.Shared.DataModels
{
    public class Dwelling
    {
        public int id;
        public int? ownerId;
        public DwellingType type; // enum
        public int x;
        public int y;
        public string occupiedTilesString; // parse
        public string name;
    }
}
