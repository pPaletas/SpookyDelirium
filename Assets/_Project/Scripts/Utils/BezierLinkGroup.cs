using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BezierLinkGroup : MonoBehaviour
{
    [SerializeField] private int _width = 5;
    [SerializeField] private int _resolution = 1;

    private void SpawnLinksAlongWidth()
    {
        Vector3 currentPos = Vector3.forward * -_width * 0.5f;
        Transform original = transform.Find("BezierLink");

        for (int i = 0; i < _width * _resolution; i++)
        {
            Transform bLink = i == 0 ? original : Instantiate(original, transform);
            bLink.localPosition = currentPos;

            currentPos.z += 1 / _resolution;
        }
    }

    private void Start()
    {
        SpawnLinksAlongWidth();
    }

    private void OnDrawGizmos()
    {
#if (UNITY_EDITOR)
        if (!EditorApplication.isPlaying)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.2f, 0.2f, _width));
        }
#endif
    }
}