using System;
using Assets.Scripts.InGame.Pathfinding;
using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.NetMessages.World.Models;
using UnityEngine;

[RequireComponent(typeof(HeroMotor))]
public class HeroView : MonoBehaviour, IPathRequester
{
    public GameObject graphic;
    public Hero hero;
    public HeroMotor motor;
    public bool isMoving;

    [SerializeField]
    private GameObject teleportOutEffect;

    [SerializeField]
    private GameObject teleportInEffect;

    private void Awake()
    {
        motor = GetComponent<HeroMotor>();
    }

    public void MoveToNode(Coord destination)
    {
        if (this.motor.Path != null && this.motor.Path.Length > 0)
        {
            // 1. If path exists - use it
            this.motor.ExecuteFollowPath(this.motor.Path);
        }
        else
        {
            // 2. If not -> find the path and then use it.
            Vector3 start = this.transform.position;
            Vector3 end = MapManager.Instance.graph.nodes[destination.X, destination.Y].worldPosition;
            PathRequestManager.RequestPath(start, end, this, ExecutePathImmediate);
        }
    }

    public void Blink(Coord destination)
    {
        this.motor.ExecuteBlink(destination);
    }

    public void TeleportOut()
    {
        this.motor.ExecuteTeleportOut();
    }

    public void Init(Hero hero, Vector3 worldPosition)
    {
        if (graphic != null)
        {
            this.hero = hero;
            gameObject.name = "Hero (" + hero.x + "," + hero.y + ")";
            gameObject.transform.position = worldPosition;

            InitGraphic();
        }
    }

    private void InitGraphic()
    {
        // TODO: do something with this.graphic.
        graphic.transform.localPosition = new Vector3(0, .5f, 0);
    }

    public void OnPathFound(Node[] newPath)
    {
        this.motor.Path = newPath;
    }

    private void ExecutePathImmediate(Node[] path, bool success)
    {
        if (success)
        {
            this.motor.ExecuteFollowPath(path);
        }
    }

    public void PlayTeleportEffect(TeleportType teleportType)
    {
        switch (teleportType)
        {
            case TeleportType.In:
                this.teleportInEffect.SetActive(false);
                this.teleportInEffect.SetActive(true);
                break;
            case TeleportType.Out:
                this.teleportOutEffect.SetActive(false);
                this.teleportOutEffect.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void StopTeleportEffects()
    {
        this.teleportInEffect.SetActive(false);
        this.teleportOutEffect.SetActive(false);
    }

    public enum TeleportType
    {
        In,
        Out
    }

    //TODO: add other methods related with hero visuals. Like coloring on hover
}
