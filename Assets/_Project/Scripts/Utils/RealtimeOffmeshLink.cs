using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEditor;
using UnityEngine;

// Poner el parent paralelo a la esquina, que el up vector quede apuntando hacia la dirección del movimiento

public class RealtimeOffmeshLink : MonoBehaviour
{
    [SerializeField] private float _width;

    private BezierLink _bLink;
    private Transform _target;
    private ActorsManager _actorsManager;

    private Vector3 _direction;
    private Vector3 _initialPos;

    private Vector3 _lastTargetPos;
    private float _distanceFromProjectedCenter;

    private const float TARGET_VELOCITY_THRESHOLD = 0.01f;
    private const float OFFSET_VALUE = 0.07f;

    private void SetTarget()
    {
        float lastDist = 1000f;

        for (int i = 0; i < _actorsManager.Players.Count; i++)
        {
            float dist = Vector3.Distance(_actorsManager.Players[i].transform.position, transform.position);

            if (dist < lastDist)
            {
                _target = _actorsManager.Players[i].transform;
            }
        }
    }

    private void FollowTarget()
    {
        Vector3 projectedWorld = Vector3.Project(_target.position, transform.forward);
        Vector3 clampedDirection = Vector3.ClampMagnitude(projectedWorld - transform.forward * _distanceFromProjectedCenter, _width * 0.5f);
        Vector3 projectedLocal = _initialPos + clampedDirection;

        transform.position = projectedLocal;
    }

    private void ApplyOffsetToEveryPoint()
    {
        _bLink.offMeshLink.startTransform.position += _bLink.offMeshLink.startTransform.up * OFFSET_VALUE;
        _bLink.offMeshLink.endTransform.position += _bLink.offMeshLink.endTransform.up * OFFSET_VALUE;
        _bLink.startTangent.position += _bLink.offMeshLink.startTransform.up * OFFSET_VALUE;
        _bLink.endTangent.position += _bLink.offMeshLink.endTransform.up * OFFSET_VALUE;
    }

    private void Awake()
    {
        _actorsManager = FindObjectOfType<ActorsManager>();

        _direction = transform.forward;
        _initialPos = transform.position;
        _lastTargetPos = Vector3.zero;

        float sign = Vector3.Dot(Vector3.Project(_initialPos, _direction).normalized, transform.forward);
        _distanceFromProjectedCenter = Vector3.Project(_initialPos, _direction).magnitude * sign;

        _bLink = GetComponent<BezierLink>();
        ApplyOffsetToEveryPoint();
    }

    private void Update()
    {
        SetTarget();
    }

    private void LateUpdate()
    {
        bool targetIsMoving = (_target.position - _lastTargetPos).magnitude >= TARGET_VELOCITY_THRESHOLD;
        _lastTargetPos = _target.position;

        if (targetIsMoving)
        {
            FollowTarget();
        }
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