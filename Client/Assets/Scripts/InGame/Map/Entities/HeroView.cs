using Assets.Scripts.Data.Models;
using UnityEngine;

public class HeroView : MonoBehaviour
{
    public GameObject graphic;
    Hero _hero;

    MapGrid _grid;

    private void Start()
    {
        MapGrid _grid = FindObjectOfType<MapGrid>();
    }

    public void Init(Hero hero)
    {
        if (graphic != null)
        {
            gameObject.name = "Hero (" + hero.x + "," + hero.y + ")";
            gameObject.transform.position = _grid.grid[hero.x, hero.y].worldPosition;
        }
    }

    //TODO: add other methods related with hero visuals. Like coloring on hover
}
