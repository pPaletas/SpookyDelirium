using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;

public class PoisonousProjectile : MonoBehaviour
{
    [SerializeField] private float _initialDamage = 10f;
    [SerializeField] private float _damageRadius = 4f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _poisonDamage = 4f;
    [SerializeField] private float _posionTime = 5f;

    private GameObject _explosion;
    private Rigidbody _rb;

    private SpiderStateMachine _stateMachine;

    public void Init(SpiderStateMachine stateMachine, Vector3 initialVel, Rigidbody rb)
    {
        _stateMachine = stateMachine;
        _rb = rb;

        _rb.velocity = initialVel;
    }

    private void PoisonSurrounding()
    {
        float radius = _stateMachine.spitSpreadRadius;
        LayerMask groundMask = _stateMachine.groundLayer;

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, ~groundMask);

        foreach (Collider col in colliders)
        {
            bool isPlayer = col.TryGetComponent<PlayerCharacterController>(out PlayerCharacterController fpc);

            if (isPlayer && col.TryGetComponent<Health>(out Health health))
            {
                health.TakeDamage(_damage, gameObject);
                health.Poison(_poisonDamage, _posionTime);
            }
        }
    }

    private void Awake()
    {
        _explosion = transform.Find("Explosion").gameObject;
    }

    private void Update()
    {
        if (_stateMachine != null)
        {
            Vector3 vel = _rb.velocity;
            vel.y -= _stateMachine.spitGravity * Time.deltaTime;
            _rb.velocity = vel;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        _explosion.transform.SetParent(null);
        _explosion.SetActive(true);
        PoisonSurrounding();

        gameObject.SetActive(false);
        GameObject.Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _damageRadius);
    }
}