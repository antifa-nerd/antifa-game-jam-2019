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

    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;


    [HideInInspector]
    public Transform VisibleTarget = null;

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
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

    private void Update()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(ViewAngle * meshResolution);
        float stepAngleSize = ViewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        // ViewCastInfo oldViewCast = new ViewCastInfo ();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = -transform.eulerAngles.z - ViewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            // if (i > 0) {
            // 	bool edgeDstThresholdExceeded = Mathf.Abs (oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            // 	if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded)) {
            // 		EdgeInfo edge = FindEdge (oldViewCast, newViewCast);
            // 		if (edge.pointA != Vector3.zero) {
            // 			viewPoints.Add (edge.pointA);
            // 		}
            // 		if (edge.pointB != Vector3.zero) {
            // 			viewPoints.Add (edge.pointB);
            // 		}
            // 	}

            // }


            viewPoints.Add(newViewCast.point);
            Debug.DrawLine(transform.position, newViewCast.point, Color.red);
            // oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit2D hit;

        hit = Physics2D.Raycast(transform.position, dir, ViewRadius, ObstacleMask);
        if (hit.collider != null)
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * ViewRadius, ViewRadius, globalAngle);
        }
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}