using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EnemyFieldOfView))]
public class EnemyFieldOfViewEditor : Editor
{

    void OnSceneGUI()
    {
        var fow = (EnemyFieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.forward, Vector3.left, 360, fow.ViewRadius);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.ViewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.ViewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.ViewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.ViewRadius);

        Handles.color = Color.red;
        if (fow.VisibleTarget != null)
        {
            Handles.DrawLine(fow.transform.position, fow.VisibleTarget.position);
        }
    }

}