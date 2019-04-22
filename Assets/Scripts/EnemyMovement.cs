using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;
using System.Linq;
using UnityEditor;

public class EnemyMovement : MonoBehaviour
{
    public GameObject[] Steps;

    private int currentStepIndex = 0;

    private int currentWaypointIndex = 0;

    public float Speed = 10f;

    public float RotationSpeed = 1f;

    private Seeker Seeker;

    private EnemySight EnemySight;

    private GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        EnemySight = this.GetComponentInChildren<EnemySight>();
        Seeker = GetComponent<Seeker>();
        Player = GameObject.FindGameObjectWithTag("Player");

        EnemySight.OnAlertedChanged += AlertedChanged;
        Seeker.pathCallback += this.OnPathComplete;
    }

    private void OnDisable()
    {
        EnemySight.OnAlertedChanged -= AlertedChanged;
        Seeker.pathCallback -= null;
    }

    private Coroutine FollowingPlayer = null;

    private void AlertedChanged(object source, bool alerted)
    {
        UnityEngine.Debug.Log("Alerted changed to: " + alerted.ToString());
        if (alerted)
        {
            // start following the player
            FollowingPlayer = StartCoroutine(FollowPlayer());
        }
        else
        {
            // stop following the player
            if (FollowingPlayer != null)
            {
                StopCoroutine(FollowingPlayer);
                FollowingPlayer = null;
            }
            // recompute where to start patrol
            // TODO: run N pathfinding to the N patrol points, compute total path distances, get the shortest one
            int minStepIndex = -1;
            var minSqrDist = float.MaxValue;
            for (var i = 0; i < Steps.Length; i++)
            {
                var step = Steps[i];
                var sqrDist = (step.transform.position - transform.position).sqrMagnitude;
                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    minStepIndex = i;
                }
            }
            currentStepIndex = minStepIndex;
            Path = null;
            ComputingPath = true;
            Seeker.StartPath(transform.position, Steps[currentStepIndex].transform.position);
        }
    }

    private IEnumerator FollowPlayer()
    {
        while (EnemySight.Alerted)
        {
            Path = null;
            ComputingPath = true;
            Seeker.StartPath(transform.position, Player.transform.position);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private Path Path = null;

    private bool ComputingPath = false;

    public void OnPathComplete(Path p)
    {
        if (p.error)
        {
            throw new Exception(p.errorLog);
        }
        Path = p;
        ComputingPath = false;
        currentWaypointIndex = 0;
    }

    private void OnDrawGizmosSelected()
    {
        if ( Steps != null )
        {
            Gizmos.color = Color.blue;
            for ( var i = 0; i < Steps.Length; i++ )
            {
                if ( Steps[i] != null )
                {
                    var pos1 = Steps[i].transform.position;
                    var pos2 = Steps[( i + 1 ) % Steps.Length].transform.position;
                    Gizmos.DrawLine( pos1, pos2 );
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // compute start and end point of this patrolling area
        var startPosition = transform.position;

        // if we need to compute to path: run it
        if (Path == null && !ComputingPath)
        {
            var patrollingEndPosition = Steps[currentStepIndex].transform.position;
            ComputingPath = true;
            Seeker.StartPath(startPosition, patrollingEndPosition);
        }

        // if we still don't have the path: don't move yet
        if (Path == null)
        {
            return;
        }

        // get the end position for this waypoint
        var endPosition = Path.vectorPath[currentWaypointIndex];

        // move along the path
        var delta = Speed * Time.deltaTime;
        var totalDistance = (endPosition - startPosition).magnitude;
        transform.position = Vector3.Lerp(startPosition, endPosition, delta / totalDistance);

        // move to the next path part if completed it
        if (delta > totalDistance)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= Path.vectorPath.Count)
            {
                Path = null;
                currentStepIndex = (currentStepIndex + 1) % Steps.Length;
            }
        }

        // rotate according to the direction
        var destUp = (endPosition - transform.position).normalized;
        if (transform.up != destUp)
        {
            transform.up = Vector3.Lerp(transform.up, destUp, RotationSpeed * Time.deltaTime / (destUp - transform.up).magnitude);
        }
    }
}
