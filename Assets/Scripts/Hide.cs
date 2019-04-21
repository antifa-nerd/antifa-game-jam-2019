using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var myCollider = gameObject.GetComponent<BoxCollider2D>();
        var delta1 = new Vector2(myCollider.size.x / 2, myCollider.size.y / 2);
        var delta2 = new Vector2(myCollider.size.x / 2, -myCollider.size.y / 2);
        colliderPoints = new Vector2[] {
            myCollider.offset + delta1,
            myCollider.offset - delta1,
            myCollider.offset + delta2,
            myCollider.offset - delta2,
        };
    }

    private Vector2[] colliderPoints;

    public LayerMask HidingPlaceLayers;

    public GameObject Shadow;

    [HideInInspector]
    public bool Hidden;

    private void OnTriggerStay2D(Collider2D other)
    {
        // only collides with our layer mask
        if ((HidingPlaceLayers & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }
        // check if all our points are inside the collider
        var otherCollider = other.gameObject.GetComponent<Collider2D>();
        bool fullyHidden = true;
        foreach (var point in colliderPoints)
        {
            if (!otherCollider.OverlapPoint(point + (Vector2)gameObject.transform.position))
            {
                fullyHidden = false;
                break;
            }
        }

        if (Shadow.activeSelf != fullyHidden)
        {
            Shadow.SetActive(fullyHidden);
        }
        Hidden = fullyHidden;
    }
}
