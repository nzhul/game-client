namespace Assets.Scripts.InGame.Pathfinding
{
    public interface IPathRequester
    {
        void OnPathFound(Node[] newPath);
    }
}