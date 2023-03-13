using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SpiderLookAtTarget : MonoBehaviour
{
    public float smoothness = 0.5f;
    [SerializeField, Range(0f, 80f)] private float _constraintAngle = 30f;
    [SerializeField] private float _targetOffset = 1.25f;

    private SpiderStateMachine _stateMachine;
    private Transform _head;

    private Quaternion _targetRot;
    private bool _isActive = false;

    public void SetLookAtActive(bool active)
    {
        _isActive = active;
    }

    private void LookAtTarget()
    {
        if(_stateMachine.currentTarget == null) return;

        Vector3 offset = _stateMachine.currentTarget.up * _targetOffset;
        Vector3 targetPos = _stateMachine.currentTarget.transform.position + offset;

        Vector3 targetDir = Vector3.Normalize(_stateMachine.currentTarget.position - _head.position);
        float angleDiff = Vector3.Dot(targetDir, _stateMachine.transform.forward);
        angleDiff = Mathf.Acos(angleDiff) * Mathf.Rad2Deg;

        // Si se encuentra dentro del rango de visión, actualizar
        if (angleDiff <= 80f && _isActive)
        {
            _targetRot = Quaternion.LookRotation(targetDir, transform.up);
        }
        else
        {
            _targetRot = Quaternion.LookRotation(transform.forward, transform.up);
        }

        Quaternion nextRot = Quaternion.Slerp(_head.rotation, _targetRot, smoothness * Time.deltaTime);
        bool isInRange = Quaternion.Angle(nextRot, transform.rotation) <= _constraintAngle;

        /* Revisar si la cabeza se encuentra fuera del rango, esto para evitar que se quede atascada 
        como se puede ver en la spitter cuando baja un muro*/
        bool isHeadOut = Quaternion.Angle(_head.rotation, transform.rotation) > _constraintAngle;

        if (isInRange || isHeadOut)
        {
            _head.rotation = nextRot;
        }
    }

    private void Awake()
    {
        _stateMachine = GetComponent<SpiderStateMachine>();
        _head = transform.Find("Body/Head");
    }

    private void LateUpdate()
    {
        if (_isActive)
            LookAtTarget();
    }
}