using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public LayerMask movementMask;
    Camera cam;
    MapGrid _grid;
    // public HeroMotor activeHeroMotor; // current active hero of the player
    // public Interactable focus; // current focus of the player (Node)

    private void Start()
    {
        _grid = FindObjectOfType<MapGrid>();
        cam = Camera.main;
        // activeHeroMotor = GetActiveHeroMotor()...
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, movementMask))
            {
                //Debug.Log(hit.point);
                Node node = _grid.NodeFromWorldPoint(hit.point);
                Debug.Log(string.Format("{0}:{1}", node.gridX, node.gridY));
                // SetFocus
            }
        }
    }
}