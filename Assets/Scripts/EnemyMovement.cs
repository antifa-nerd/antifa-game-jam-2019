using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyMovement : MonoBehaviour
{
    public GameObject[] Steps;

    private int currentStep = 0;

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
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
    }

    // Update is called once per frame
    void Update()
    {
        // get the current path we're following
        var startPosition = transform.position;
        var endPosition = Steps[currentStep].transform.position;

        if (Path == null && !ComputingPath)
        {
            ComputingPath = true;
            Seeker.StartPath(startPosition, endPosition);
        }

        // move along the path
        var delta = Speed * Time.deltaTime;
        var totalDistance = (endPosition - startPosition).magnitude;
        transform.position = Vector3.Lerp(startPosition, endPosition, delta / totalDistance);

        // move to the next path part if completed it
        if (delta > totalDistance)
        {
            Path = null;
            currentStep = (currentStep + 1) % Steps.Length;
        }

        // rotate according to the direction
        var destUp = endPosition - transform.position;
        if (transform.up != destUp)
        {
            transform.up = Vector3.Lerp(transform.up, destUp, RotationSpeed / (destUp - transform.up).magnitude);
        }
    }
}
