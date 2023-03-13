using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[CustomEditor(typeof(BezierLink))]
public class DrawBezier : Editor
{
    BezierLink bLink;

    private void OnSceneViewGUI(SceneView sv)
    {
        if (bLink.editBezier && bLink.offMeshLink != null && bLink.startTangent != null && bLink.endTangent != null)
        {
            Vector3 startPos = bLink.offMeshLink.startTransform.position;
            Vector3 endPos = bLink.offMeshLink.endTransform.position;

            bLink.startTangent.position = Handles.PositionHandle(bLink.startTangent.position, Quaternion.identity);
            bLink.endTangent.position = Handles.PositionHandle(bLink.endTangent.position, Quaternion.identity);

            Handles.DrawBezier(startPos, endPos, bLink.startTangent.position, bLink.endTangent.position, Color.red, null, 2f);
        }
    }

    private void Awake()
    {
        bLink = (BezierLink)target;
        bLink.offMeshLink = bLink.GetComponent<OffMeshLink>();
        bLink.startTangent = bLink.transform.Find("StartTangent");
        bLink.endTangent = bLink.transform.Find("EndTangent");

        SceneView.duringSceneGui += OnSceneViewGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneViewGUI;
    }
}