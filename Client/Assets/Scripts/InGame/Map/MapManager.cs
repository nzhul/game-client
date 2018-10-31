using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Models;
using Assets.Scripts.Network;
using Assets.Scripts.Utilities;
using BestHTTP;
using Newtonsoft.Json;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private DataManager _dm;

    void Start()
    {
        _dm = DataManager.Instance;
        _dm.Load();

        if (_dm.Avatar != null && _dm.Avatar.heroes != null && _dm.Avatar.heroes.Count >= 1)
        {
            int[] regionsForLoading = _dm.Avatar.heroes.Select(h => h.regionId).ToArray();

            string endpoint = "realms/{0}/regions";
            string[] @params = new string[] { DataManager.Instance.CurrentRealmId.ToString() };
            IDictionary<string, string> queryParams = new Dictionary<string, string>();

            for (int i = 0; i < regionsForLoading.Length; i++)
            {
                queryParams.Add("regionIds", regionsForLoading[i].ToString());
            }

            RequestManager.Instance.Get(endpoint, @params, queryParams, OnGetGetRegionsRequestFinished);
        }
    }

    private void OnGetGetRegionsRequestFinished(HTTPRequest request, HTTPResponse response)
    {
        // TODO: Implement this
        // store the region information in a file (maybe a different file than the main one)
        // use region data to render the map!

        if (Common.RequestIsSuccessful(request, response))
        {
            string json = response.DataAsText;
            IList<Region> regions = JsonConvert.DeserializeObject<IList<Region>>(json);

            if (regions != null && regions.Count >= 1)
            {
                _dm.Regions = regions;
                _dm.Save();

                this.RenderMap();
            }
        }
    }

    private void RenderMap()
    {
        Region activeRegion = _dm.Regions.FirstOrDefault(r => r.heroes.Any(h => h.id == _dm.ActiveHeroId));
        if (activeRegion == null)
        {
            Debug.LogWarning("Cannot find region for the current active hero with ID: " + _dm.ActiveHeroId);
            return;
        }

        Debug.Log(activeRegion.matrixString);

        // TODO: Implement Graph, GraphView, Node, NodeView PathFinder, PriorityQueue
        // See pathfinding-in-unity repo for reference.
    }

    // Update is called once per frame
    void Update()
    {

    }
}
