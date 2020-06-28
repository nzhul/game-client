using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.InGame.Pathfinding;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.Shared.Models.Units;
using UnityEngine;

namespace Assets.Scripts.InGame.Map.Entities
{
    /// <summary>
    /// Alive content can move and interact with other contents.
    /// Such content is Army or Unit.
    /// </summary>
    public abstract class AliveEntityView : NodeContent, IPathRequester
    {
        public GameObject graphic;

        [NonSerialized]
        public UnitMotor motor;

        [NonSerialized]
        public GridEntity rawEntity; // N: I should make base class for Unit and Army. Ex: IMovable or Mover that have X and Y

        public bool isMoving;

        private MeshRenderer mr;

        public NodeView targetNode; // the node that is clicked
        public Node destinationNode; // the node at the end of the path

        public Material friendlyEntityMat;
        public Material enemyEntityMat;

        public GameObject teleportOutEffect;
        public GameObject teleportInEffect;

        public List<NodeView> AvailibleDestinations { get; set; }

        public abstract bool IsFriendly { get; }

        private void Awake()
        {
            mr = graphic.GetComponent<MeshRenderer>();
            motor = GetComponent<UnitMotor>();
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

        //public void TeleportOut()
        //{
        //    this.motor.ExecuteTeleportOut();
        //}

        public void Init(GridEntity entity, Coord spawnCoordinates, Vector3 worldPosition)
        {
            if (graphic != null)
            {
                this.rawEntity = entity;
                gameObject.name = "AliveEntity";
                gameObject.transform.position = worldPosition;

                InitGraphic();
            }
        }

        private void InitGraphic()
        {
            // TODO: do something with this.graphic.
            graphic.transform.localPosition = new Vector3(0, .6f, 0);

            if (this.IsFriendly)
            {
                this.mr.sharedMaterial = friendlyEntityMat;
            }
            else
            {
                this.mr.sharedMaterial = enemyEntityMat;
            }
        }

        public void OnPathFound(Node[] newPath)
        {
            this.motor.Path = newPath;

            if (this.targetNode != null)
            {
                if (this.targetNode.node.nodeType == NodeType.ContactPoint)
                {
                    this.targetNode.node.walkable = false;
                    if (newPath != null && newPath.Length > 1)
                    {
                        // If we are targeting contact point and the path is longer than 1, remove the last node
                        this.motor.Path = newPath.Take(newPath.Length - 1).ToArray();
                    }
                    else if (newPath != null && newPath.Length == 1)
                    {
                        // if we are targeting contact point and the path is exactly one 
                        // - we are setting the destination to be equal to the position of the hero.
                        this.motor.Path = null;
                        this.destinationNode = MapManager.Instance.GetNode(this.rawEntity.X, this.rawEntity.Y);
                    }
                    else
                    {
                        this.motor.Path = null;
                    }
                }
            }

            if (this.motor.Path != null && this.motor.Path.Length > 0)
            {
                this.destinationNode = this.motor.Path.Last();
            }
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
    }
}
