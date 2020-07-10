using System;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Data;
using Assets.Scripts.InGame.Console;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Shared.Models;
using BestHTTP;
using Newtonsoft.Json;
using UnityEngine;

[RequireComponent(typeof(Graph))]
[RequireComponent(typeof(GraphView))]
public class MapManager : MonoBehaviour
{
    #region Singleton
    private static MapManager _instance;

    public static MapManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        graph = GetComponent<Graph>();
        graphView = GetComponent<GraphView>();
    }
    #endregion

    public Graph graph;
    public GraphView graphView;

    public event Action OnInitComplete;

    private void Start()
    {

        //TODO: Wrap this Initialization logic into progress tacker and hook it somehow to the GameManager Loading Screen

        var game = RequestManagerHttp.GameService.GetGame(DataManager.Instance.ActiveGameId);
        DataManager.Instance.ActiveGame = game;
        DataManager.Instance.Avatar = game.Avatars.FirstOrDefault(a => a.UserId == DataManager.Instance.Id);
        DataManager.Instance.ActiveArmyId = game.Armies.FirstOrDefault(x => x.Team == DataManager.Instance.Avatar.Team).Id;
        DataManager.Instance.UnitConfigurations = RequestManagerHttp.GameService.GetUnitConfigurations();


        //BUG: I need to rotate the whole matrix 90 degrees. so it becomes 170x70 and not 70x170

        RenderMap(DataManager.Instance.ActiveGame);
        PlayerController.Instance.EnableInputs();
        OnInitComplete?.Invoke();

        //TODO: Wrap this Initialization logic into progress tacker and hook it somehow to the GameManager Loading Screen
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && !DeveloperConsole.ConsoleIsOpen)
        {
            graphView.labelsCanvas.gameObject.SetActive(!graphView.labelsCanvas.gameObject.activeInHierarchy);
        }
    }

    private void RenderMap(Game game)
    {
        if (game == null)
        {
            Debug.LogWarning("Cannot find region for the current active hero with ID: " + DataManager.Instance.ActiveArmyId);
            return;
        }



        if (graph != null && graphView != null)
        {
            graph.Init(game.MatrixString);
            graphView.Init(graph);
            // graphView.AddNPCArmies(game.Armies);
            graphView.AddArmies(game.Armies);
            PlayerController.Instance.SetActiveEntity(DataManager.Instance.ActiveArmyId);
            //graphView.Dwellings();
        }
    }

    //public Vector3 GetNodeWorldPosition(int x, int y)
    //{
    //    return graph.nodes[x, y].worldPosition;
    //}

    public Node GetNode(int x, int y)
    {
        return graph.nodes[x, y];
    }

    //public class HeroLoader
    //{
    //    public int HeroId { get; set; }
    //    public Coord Destination { get; set; }

    //    public void Load(int heroId, Coord destination)
    //    {
    //        this.HeroId = heroId;
    //        this.Destination = destination;

    //        string endpoint = "realms/heroes/{0}";
    //        string[] @params = new string[] { this.HeroId.ToString() };
    //        RequestManager.Instance.Get(endpoint, @params, DataManager.Instance.Token, OnHeroLoadComplete);
    //    }

    //    private void OnHeroLoadComplete(HTTPRequest request, HTTPResponse response)
    //    {
    //        if (NetworkCommon.RequestIsSuccessful(request, response, out string errorMessage))
    //        {
    //            string json = response.DataAsText;
    //            var newHero = JsonConvert.DeserializeObject<Hero>(json);

    //            MapManager.Instance.graphView.AddHero(newHero, new Coord(newHero.X, newHero.Y), true);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Error fetching hero data from the API on EnemyIn teleport request!");
    //        }
    //    }
    //}
}
