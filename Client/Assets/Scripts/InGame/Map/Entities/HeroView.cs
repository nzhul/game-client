using Assets.Scripts.Data.Models;
using UnityEngine;

public class HeroView : MonoBehaviour
{
    public GameObject graphic;
    public Hero hero;
    public Vector3 worldPosition;

    public void Init(Hero hero, Vector3 worldPosition)
    {
        if (graphic != null)
        {
            this.hero = hero;
            gameObject.name = "Hero (" + hero.x + "," + hero.y + ")";
            gameObject.transform.position = worldPosition;
            this.worldPosition = worldPosition;

            InitGraphic();
        }
    }

    private void InitGraphic()
    {
        // TODO: do something with this.graphic.
        graphic.transform.localPosition = new Vector3(0, .3f, 0);
    }

    //TODO: add other methods related with hero visuals. Like coloring on hover
}
