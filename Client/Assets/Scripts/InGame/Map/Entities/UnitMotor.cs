﻿using Assets.Scripts.InGame;
using Assets.Scripts.Shared.NetMessages.World.Models;
using System;
using System.Collections;
using UnityEngine;

public class UnitMotor : MonoBehaviour
{
    private Vector3[] waypoints;
    private int targetIndex;
    public float speed = 20;
    private UnitView selfView;
    public event Action<Node, UnitView> OnDestinationReached;

    public Node[] Path { get; set; }

    private void Start()
    {
        selfView = GetComponent<HeroView>();
    }

    public void ExecuteFollowPath(Node[] waypoints)
    {
        StopCoroutine(FollowPath(waypoints));
        StartCoroutine(FollowPath(waypoints));
    }

    private IEnumerator FollowPath(Node[] waypoints)
    {
        if (waypoints.Length > 0)
        {
            Vector3 currentWaypoint = waypoints[0].worldPosition;
            targetIndex = 0;
            selfView.isMoving = true;

            while (true)
            {
                if (transform.position == currentWaypoint)
                {
                    targetIndex++;
                    if (targetIndex >= waypoints.Length)
                    {
                        selfView.rawUnit.X = waypoints[targetIndex - 1].gridX;
                        selfView.rawUnit.Y = waypoints[targetIndex - 1].gridY;
                        selfView.isMoving = false;
                        this.ClearPath();
                        OnDestinationReached?.Invoke(waypoints[targetIndex - 1], selfView);
                        yield break;
                    }
                    currentWaypoint = waypoints[targetIndex].worldPosition;
                }

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                yield return null;
            }
        }
        else
        {
            selfView.isMoving = false;
            this.ClearPath();
            OnDestinationReached?.Invoke(null, selfView);
        }
    }

    public void ExecuteTeleportOut()
    {
        StopCoroutine(TeleportOutRoutine());
        StartCoroutine(TeleportOutRoutine());
    }

    private IEnumerator TeleportOutRoutine()
    {
        selfView.PlayTeleportEffect(HeroView.TeleportType.Out);
        yield return new WaitForSeconds(.4f);
        HeroesManager.Instance.Heroes.Remove(selfView.rawUnit.Id);
        Destroy(selfView.gameObject);
    }

    private void ClearPath()
    {
        this.Path = null;
    }

    public void ExecuteBlink(Coord destination)
    {
        StopCoroutine(TeleportRoutine(destination));
        StartCoroutine(TeleportRoutine(destination));
    }

    private IEnumerator TeleportRoutine(Coord destination)
    {
        // 1. play teleport out particle effect
        // selfView.TeleportEffect.SetActive(false)
        // selfView.TeleportEffect.SetActive(true)
        // 2. yield wait for the duration of the effect.
        // 3. hide the hero graphic
        // 4. update hero position
        // 5. play teleport in particle effect
        // 6. yield wait for the duration of the effect
        // 7. show the hero graphic
        // 8. update hero position in the cache.
        // 9. Smooth camera follow.

        // 1. Teleport Out
        selfView.PlayTeleportEffect(HeroView.TeleportType.Out);
        yield return new WaitForSeconds(.4f);
        selfView.graphic.SetActive(false);
        this.transform.position = MapManager.Instance.graph.nodes[destination.X, destination.Y].worldPosition;
        selfView.rawUnit.X = destination.X;
        selfView.rawUnit.Y = destination.Y;
        this.ClearPath();

        // 2. Teleport In
        MapCamera.Instance.StartFollowTarget(this.transform.position);
        selfView.PlayTeleportEffect(HeroView.TeleportType.In);
        yield return new WaitForSeconds(.4f);
        selfView.graphic.SetActive(true);

        // TODO: Smooth camera follow!


    }
}
