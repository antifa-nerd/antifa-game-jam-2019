using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public GameObject[] Steps;

    private int currentStep = 0;
    private float distance = 0;

    public float Speed = 10f;

    public float RotationSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // get the current path we're following
        var startStep = Steps[currentStep];
        var endStep = Steps[(currentStep + 1) % Steps.Length];
        var startPosition = startStep.transform.position;
        var endPosition = endStep.transform.position;

        // move along the path
        distance += Speed * Time.deltaTime;
        var totalDistance = (endPosition - startPosition).magnitude;
        transform.position = Vector3.Lerp(startPosition, endPosition, distance / totalDistance);

        // rotate according to the direction
        var destUp = endPosition - transform.position;
        if (transform.up != destUp)
        {
            transform.up = Vector3.Lerp(transform.up, destUp, RotationSpeed / (destUp - transform.up).magnitude);
        }

        // move to the next path part if completed it
        if (distance > totalDistance)
        {
            distance = totalDistance - distance;
            currentStep = (currentStep + 1) % Steps.Length;
        }
    }
}
