namespace Assets.Scripts.Shared.DataModels
{
    public class Room
    {
        public int id;
        public string tilesString; // parse
        public string edgeTilesString; // parse
        public int roomSize;
        public bool isMainRoom;
        public bool isAccessibleFromMainRoom;
    }
}
