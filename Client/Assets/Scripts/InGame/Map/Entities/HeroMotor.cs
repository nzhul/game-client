using System;
using System.Collections;
using UnityEngine;

public class HeroMotor : MonoBehaviour
{
    Vector3[] waypoints;
    int targetIndex;
    public float speed = 20;
    HeroView selfView;
    public event Action<Node> OnDestinationReached;

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
                    selfView.worldPosition = transform.position;
                    selfView.hero.x = waypoints[targetIndex - 1].gridX;
                    selfView.hero.y = waypoints[targetIndex - 1].gridY;
                    selfView.isMoving = false;
                    if (OnDestinationReached != null)
                    {
                        OnDestinationReached(waypoints[targetIndex - 1]);
                    }
                    yield break;
                }
                currentWaypoint = waypoints[targetIndex].worldPosition;
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }
}
