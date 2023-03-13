using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class SpiderProceduralWalk : MonoBehaviour
{
    [SerializeField] private LegData[] _legs;
    [SerializeField] private float _stepLength = 0.75f;
    // [SerializeField] private float _stressPoint = 1.5f;

    private NavMeshAgent _agent;

    private Vector3 _lastPos;

    private bool CanMove(int legIndex)
    {
        if (legIndex <= 2)
        {
            LegData l1 = _legs[3];
            LegData l2 = _legs[4];
            LegData l3 = _legs[5];

            return !l1.leg.IsMoving && !l2.leg.IsMoving && !l3.leg.IsMoving;
        }
        else
        {
            LegData l1 = _legs[0];
            LegData l2 = _legs[1];
            LegData l3 = _legs[2];

            return !l1.leg.IsMoving && !l2.leg.IsMoving && !l3.leg.IsMoving;
        }
    }

//     private bool StressPointReached(int legIndex)
//     {
//         // bool isReached = _legs[legIndex].leg.LegExtension >= _stressPoint;
// // 
//         return true;
//     }

    private void WalkProcedurally()
    {
        Vector3 vel = _agent.transform.position - _lastPos;
        _lastPos = _agent.transform.position;

        for (int index = 0; index < _legs.Length; index++)
        {
            LegData legData = _legs[index];

            float distance = Vector3.Distance(legData.leg.Position, legData.raycast.Position);

            if (!CanMove(index)) continue;
            if (!legData.leg.IsMoving && (distance <= _stepLength)) continue;

            legData.leg.MoveTo(legData.raycast.Position, vel, _stepLength, legData.raycast.Normal);
        }
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        WalkProcedurally();
    }

    [System.Serializable]
    private struct LegData
    {
        public LegTarget leg;
        public LegRaycast raycast;
    }
}
