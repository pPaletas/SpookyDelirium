using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LegTarget : MonoBehaviour
{
    public float stepSpeed = 5f;
    [SerializeField] private AnimationCurve _stepCurve;

    private NavMeshAgent _agent;

    private Vector3 _position;
    private Movement? _movement;

    public Vector3 Position => _position;
    public bool IsMoving => _movement != null;
    public float LegExtension => Vector3.Distance(_agent.transform.position, transform.position);

    public void MoveTo(Vector3 targetPos, Vector3 vel, float stepLength, Vector3 groundNormal)
    {
        Vector3 projectedOffset = Vector3.ProjectOnPlane(vel, groundNormal).normalized * (stepLength * 0.8f);

        if (_movement == null)
        {
            _movement = new Movement
            {
                progress = 0f,
                fromPosition = _position,
                toPosition = targetPos + projectedOffset
            };
        }
        else
        {
            _movement = new Movement
            {
                progress = _movement.Value.progress,
                fromPosition = _movement.Value.fromPosition,
                toPosition = targetPos + projectedOffset
            };
        }
    }

    private void Awake()
    {
        _position = transform.position;
        _agent = transform.root.GetComponentInChildren<NavMeshAgent>();
    }

    private void Update()
    {
        if (_movement != null)
        {
            Movement m = _movement.Value;

            m.progress = Mathf.Clamp01(m.progress + Time.deltaTime * stepSpeed);
            _position = m.Evaluate(_agent.transform.up, _stepCurve);
            _movement = m.progress < 1 ? m : null;
        }

        transform.position = _position;
    }

    private struct Movement
    {
        public float progress;
        public Vector3 fromPosition;
        public Vector3 toPosition;

        public Vector3 Evaluate(in Vector3 up, AnimationCurve stepCurve)
        {
            return Vector3.Lerp(fromPosition, toPosition, progress) + up * stepCurve.Evaluate(progress);
        }
    }
}
