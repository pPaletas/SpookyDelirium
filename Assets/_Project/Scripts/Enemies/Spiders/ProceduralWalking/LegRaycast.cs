using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegRaycast : MonoBehaviour
{
    [SerializeField] private float _rayDistance = 2f;
    [SerializeField] private LayerMask _groundLayer;

    private RaycastHit _hit;

    public Vector3 Position => _hit.point;
    public Vector3 Normal => _hit.normal;

    private void Update()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit currentHit;

        if (Physics.Raycast(ray, out currentHit, _rayDistance, _groundLayer))
        {
            _hit = currentHit;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, -transform.up * _rayDistance);
    }
}