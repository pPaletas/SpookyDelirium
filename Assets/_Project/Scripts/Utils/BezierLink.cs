using UnityEngine;
using UnityEngine.AI;

public struct Endpoint
{
    public Transform point;
    public Transform tangentPoint;
}

public class BezierLink : MonoBehaviour
{
    public bool editBezier = true;

    [HideInInspector] public OffMeshLink offMeshLink;
    public Transform startTangent;
    public Transform endTangent;

    public Vector3 GetPositionAtTime(LinkData data, float time)
    {
        Vector3 p0 = data.startPoint;
        Vector3 p1 = data.startTangentPoint;
        Vector3 p2 = data.endTangentPoint;
        Vector3 p3 = data.endPoint;

        Vector3 q0 = Vector3.Lerp(p0, p1, time);
        Vector3 q1 = Vector3.Lerp(p1, p2, time);
        Vector3 q2 = Vector3.Lerp(p2, p3, time);

        Vector3 r0 = Vector3.Lerp(q0, q1, time);
        Vector3 r1 = Vector3.Lerp(q1, q2, time);

        Vector3 s = Vector3.Lerp(r0, r1, time);

        return s;
    }

    public void GetStartAndEndLink(Vector3 currentPos, ref Endpoint start, ref Endpoint end)
    {
        float worldStartDistance = (offMeshLink.startTransform.position - currentPos).magnitude;
        float worldEndDistance = (offMeshLink.endTransform.position - currentPos).magnitude;

        if (worldEndDistance > worldStartDistance)
        {
            start.point = offMeshLink.startTransform;
            start.tangentPoint = startTangent;
            end.point = offMeshLink.endTransform;
            end.tangentPoint = endTangent;
        }
        else
        {
            end.point = offMeshLink.startTransform;
            end.tangentPoint = startTangent;
            start.point = offMeshLink.endTransform;
            start.tangentPoint = endTangent;
        }
    }

    public float GetLength(Vector3 initialPos, int iterations = 12)
    {
        float step = 1f / iterations;
        float t = 0;
        Vector3 prevPoint = Evaluate(initialPos, 0);
        float length = 0f;
        for (int i = 1; i <= iterations; i++)
        {
            t = Mathf.Lerp(0, 1, i * step);
            Vector3 point = Evaluate(initialPos, t);
            length += Vector3.Distance(prevPoint, point);
            prevPoint = point;
        }
        return length;
    }

    // Evaluates a Bezier curve at parameter t using De Casteljau's algorithm.
    private Vector3 Evaluate(Vector3 initialPos, float t)
    {
        Vector3 p0 = initialPos;
        Vector3 p1 = startTangent.position;
        Vector3 p2 = endTangent.position;
        Vector3 p3 = offMeshLink.endTransform.position;

        int degree = 3;
        Vector3[] temp = { p0, p1, p2, p3 };
        for (int i = 1; i <= degree; i++)
        {
            for (int j = 0; j <= degree - i; j++)
            {
                temp[j] = (1f - t) * temp[j] + t * temp[j + 1];
            }
        }
        return temp[0];
    }

    private void Awake()
    {
        offMeshLink = GetComponent<OffMeshLink>();
    }
}