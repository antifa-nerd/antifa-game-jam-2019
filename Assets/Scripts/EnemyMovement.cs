using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyMovement : MonoBehaviour
{
    public GameObject[] Steps;

    private int currentStep = 0;

    private int currentWaypoint = 0;

    public float Speed = 10f;

    public float RotationSpeed = 1f;

    private Seeker Seeker;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponentInChildren<EnemySight>().OnAlertedChanged += AlertedChanged;
        Seeker = GetComponent<Seeker>();
        Seeker.pathCallback += this.OnPathComplete;
    }

    private void OnDisable()
    {
        this.GetComponentInChildren<EnemySight>().OnAlertedChanged -= AlertedChanged;
        Seeker.pathCallback -= null;
    }

    private void AlertedChanged(object source, bool alerted)
    {
        UnityEngine.Debug.Log("Alerted changed to: " + alerted.ToString());
    }

    private Path Path = null;

    private bool ComputingPath = false;

    public void OnPathComplete(Path p)
    {
        Path = p;
        ComputingPath = false;
        currentWaypoint = 0;
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
    }

    // Update is called once per frame
    void Update()
    {
        // compute start and end point of this patrolling area
        var startPosition = transform.position;
        var patrollingEndPosition = Steps[currentStep].transform.position;

        // if we need to compute to path: run it
        if (Path == null && !ComputingPath)
        {
            ComputingPath = true;
            Seeker.StartPath(startPosition, patrollingEndPosition);
        }

        // if we still don't have the path: don't move yet
        if (Path == null)
        {
            return;
        }

        // get the end position for this waypoint
        var endPosition = Path.vectorPath[currentWaypoint];

        // move along the path
        var delta = Speed * Time.deltaTime;
        var totalDistance = (endPosition - startPosition).magnitude;
        transform.position = Vector3.Lerp(startPosition, endPosition, delta / totalDistance);

        // move to the next path part if completed it
        if (delta > totalDistance)
        {
            currentWaypoint++;
            if (currentWaypoint >= Path.vectorPath.Count)
            {
                Path = null;
                currentStep = (currentStep + 1) % Steps.Length;
            }
        }

        // rotate according to the direction
        var destUp = (endPosition - transform.position).normalized;
        if ((transform.up - destUp).sqrMagnitude > 0.1f)
        {
            var newUp = Vector3.Lerp(transform.up, destUp, RotationSpeed * Time.deltaTime / (destUp - transform.up).magnitude);
            if (transform.up != newUp)
            {
                Debug.Log(string.Format("{2}: Changing up from {0} to {1}", transform.up, newUp, Time.time));
            }
            transform.up = newUp;
        }
    }
}
