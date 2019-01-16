using Assets.Scripts.Shared.DataModels;
using UnityEngine;

[RequireComponent(typeof(HeroMotor))]
public class HeroView : MonoBehaviour
{
    public GameObject graphic;
    public Hero hero;
    public Vector3 worldPosition;
    public HeroMotor motor;
    public bool isMoving;

    private void Awake()
    {
        motor = GetComponent<HeroMotor>();
    }

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
        graphic.transform.localPosition = new Vector3(0, .5f, 0);
    }

    //TODO: add other methods related with hero visuals. Like coloring on hover
}
