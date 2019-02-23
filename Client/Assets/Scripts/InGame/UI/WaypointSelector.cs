using UnityEngine;

public class WaypointSelector : MonoBehaviour
{
    [SerializeField]
    private RectTransform SelectorPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {

            SelectorPanel.gameObject.SetActive(!SelectorPanel.gameObject.activeSelf);
        }
    }
}
