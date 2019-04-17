using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFieldOfView : MonoBehaviour
{
    public float ViewRadius;
    [Range(0, 360)]
    public float ViewAngle;

    public LayerMask TargetMask;
    public LayerMask ObstacleMask;

    [HideInInspector]
    public Transform VisibleTarget = null;

    void Start()
    {
        StartCoroutine(FindTargetsWithDelay(.2f));
    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        var hadPreviousTarget = VisibleTarget != null;
        VisibleTarget = null;
        var pos = (Vector2)transform.position;
        // memory inefficient
        var collider = Physics2D.OverlapCircle(
            pos,
            ViewRadius,
            TargetMask
            );

        if (collider != null)
        {
            Transform target = collider.transform;
            var targetPos = (Vector2)target.position;
            var dirToTarget = (targetPos - pos).normalized;
            if (Vector2.Angle(transform.up, dirToTarget) < ViewAngle / 2)
            {
                float dstToTarget = Vector2.Distance(pos, targetPos);

                if (!Physics2D.Raycast(pos, dirToTarget, dstToTarget, ObstacleMask))
                {
                    VisibleTarget = target;
                }
            }
        }

        // inefficient
        if (hadPreviousTarget && VisibleTarget == null)
        {
            if (OnPlayerExit != null)
            {
                OnPlayerExit(this, new EventArgs());
            }
        }
        if (!hadPreviousTarget && VisibleTarget != null)
        {
            if (OnPlayerEnter != null)
            {
                OnPlayerEnter(this, new EventArgs());
            }
        }
    }

    public event EventHandler OnPlayerExit;
    public event EventHandler OnPlayerEnter;


    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees -= transform.eulerAngles.z;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }
}
