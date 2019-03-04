using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.InGame.Console;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Network.Shared.DataModels;
using Assets.Scripts.Utilities;
using Assets.Scripts.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class WaypointSelector : MonoBehaviour
{
    [SerializeField]
    private RectTransform _waypointsPanel;

    [SerializeField]
    private RectTransform _waypointBtnsContainer;

    [SerializeField]
    private Button _waypointBtnPrefab;

    private IWorldService _worldService;

    private void Start()
    {
        _worldService = new WorldService();
        var waypoints = DataManager.Instance.Avatar.waypoints;
        RefreshWaypointButtons(waypoints);
    }

    private void RefreshWaypointButtons(IList<Waypoint> waypoints)
    {
        if (waypoints != null && waypoints.Count() > 0)
        {
            Common.Empty(_waypointBtnsContainer.transform);

            foreach (var waypoint in waypoints)
            {
                Button waypointBtn = GameObject.Instantiate<Button>(_waypointBtnPrefab, _waypointBtnsContainer);
                waypointBtn.name = "X:" + waypoint.x + "-Y:" + waypoint.y + "-" + waypoint.regionName;
                waypointBtn.onClick.AddListener(delegate { OnWaypointButtonPressed(waypointBtn, waypoint); });

                Text regionNametext = waypointBtn.transform.Find("RegionName").GetComponent<Text>();
                regionNametext.text = waypoint.regionName.Length > 24 ? waypoint.regionName.Truncate(24) + "..." : waypoint.regionName;

                Text regionLevelText = waypointBtn.transform.Find("Level").GetComponentInChildren<Text>();
                regionLevelText.text = waypoint.regionLevel.ToString();
            }

        }
        else
        {
            // no waypoints were found
            // show default empty UI
            // "You don't have any active waypoints. Explore the map to find some."

        }
    }

    private void OnWaypointButtonPressed(Button target, Waypoint waypoint)
    {
        Debug.Log("Teleporting to " + waypoint.regionName);
        _worldService.SendTeleportRequest(DataManager.Instance.ActiveHeroId, waypoint.regionId, waypoint.id);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !DeveloperConsole.ConsoleIsOpen)
        {
            _waypointsPanel.gameObject.SetActive(!_waypointsPanel.gameObject.activeInHierarchy);
        }
    }
}
