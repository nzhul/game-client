using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.InGame.Console;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Network.Services.TCP.Interfaces;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class WaypointSelector : MonoBehaviour
{
    [SerializeField]
    public RectTransform _waypointsPanel;

    [SerializeField]
    public RectTransform _waypointBtnsContainer;

    [SerializeField]
    public Button _waypointBtnPrefab;

    private IWorldService _worldService;

    private void Start()
    {
        _worldService = new WorldService();
        var waypoints = DataManager.Instance.Waypoints;
        RefreshWaypointButtons(waypoints);
    }

    private void RefreshWaypointButtons(IList<Dwelling> waypoints)
    {
        if (waypoints != null && waypoints.Count() > 0)
        {
            Common.Empty(_waypointBtnsContainer.transform);

            foreach (var waypoint in waypoints)
            {
                Button waypointBtn = GameObject.Instantiate<Button>(_waypointBtnPrefab, _waypointBtnsContainer);
                waypointBtn.name = "X:" + waypoint.X + "-Y:" + waypoint.Y + "-" + waypoint.Id;
                waypointBtn.onClick.AddListener(delegate { OnWaypointButtonPressed(waypointBtn, waypoint); });

                Text regionNametext = waypointBtn.transform.Find("RegionName").GetComponent<Text>();
                regionNametext.text = "WP-" + waypoint.Id;

                Text regionLevelText = waypointBtn.transform.Find("Level").GetComponentInChildren<Text>();
                regionLevelText.text = "RegionLevel";
            }

        }
        else
        {
            // no waypoints were found
            // show default empty UI
            // "You don't have any active waypoints. Explore the map to find some."

        }
    }

    private void OnWaypointButtonPressed(Button target, Dwelling waypoint)
    {
        _worldService.TeleportRequest(DataManager.Instance.ActiveArmyId, waypoint.GameId, waypoint.Id);
        this.CloseModal();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !DeveloperConsole.ConsoleIsOpen)
        {
            _waypointsPanel.gameObject.SetActive(!_waypointsPanel.gameObject.activeInHierarchy);
        }
    }

    private void CloseModal()
    {
        _waypointsPanel.gameObject.SetActive(false);
    }
}
