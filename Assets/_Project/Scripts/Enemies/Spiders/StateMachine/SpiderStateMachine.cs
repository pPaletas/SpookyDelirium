using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class SpiderStateMachine : MonoBehaviour
{
    public enum SpiderType { Default, Mohammed, Spitter }

    #region StatesVariables
    // Variables de estados
    public SpiderType spiderType = SpiderType.Default;

    [Header("Player Interaction")]
    public float checkRadius = 20f;
    public float attackDistance = 2f;
    public float attackAngle = 30f;
    public float attackDamage = 10f;
    public LayerMask groundLayer;
    [Header("Movement")]
    public float explosionModeSpeedMultiplier = 2f;
    [Header("Explosive Spider")]
    public float explosionDamage = 100f;
    public float explosionRadius = 5f;
    [Header("Spitter spider")]
    public GameObject spitProjectile;
    public float spitMaxDistance = 20f;
    public float spitLookAngle = 5f;
    public float spitSpreadRadius = 4f;
    [Range(5f, 80f)] public float spitAngle = 60f;
    public float spitGravity = 18f;

    // Referencias externas
    [HideInInspector] public ActorsManager actorsManager;
    [HideInInspector] public Timer attackCooldown;
    [HideInInspector] public Timer spitCooldown;
    [HideInInspector] public Timer loadExplosion;
    [HideInInspector] public Timer explosiveRush;
    [HideInInspector] public Transform currentTarget;

    // Referencias hijas
    [HideInInspector] public Animator animator;
    [HideInInspector] public AnimationEventsListener animationListener;
    [HideInInspector] public SpiderLookAtTarget lookAtTarget;
    [HideInInspector] public Transform mouth;
    [HideInInspector] public GameObject hitbox;

    // Referencias propias
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Health health;

    // Explosive rush
    [HideInInspector] public GameObject explosionParticle;
    [HideInInspector] public bool isInRushMode = false;

    // Bezier climb
    [HideInInspector] public float cachedBezierTime;
    [HideInInspector] public bool isBezierClimbPaused = false;
    public Endpoint relativeStart;
    public Endpoint relativeEnd;

    // Variables de animación
    [HideInInspector] public int animStandUpHash;
    [HideInInspector] public int animAttackHash;
    [HideInInspector] public int animLoadingExplosion;

    private const string ANIM_STAND_UP = "StandUp";
    private const string ANIM_ATTACK = "Attack";
    private const string ANIM_LOADING_EXPLOSION = "LoadingExplosion";
    #endregion

    private SpiderBaseState _currentState;
    private LegTarget[] _legs;
    private float _currentCheckRadius;
    private float _defaultSpeed;

    public void SetState(SpiderBaseState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void MultiplySpeed(float speedMultiplier)
    {
        agent.speed *= speedMultiplier;
        agent.acceleration *= speedMultiplier;
        lookAtTarget.smoothness *= speedMultiplier;

        foreach (LegTarget leg in _legs)
        {
            leg.stepSpeed *= speedMultiplier;
        }
    }

    // Solo para la araña explosiva
    public void Detonate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, ~groundLayer);

        explosionParticle.transform.SetParent(null);
        explosionParticle.SetActive(true);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<Damageable>(out Damageable damageable))
            {
                damageable.InflictDamage(explosionDamage, true, gameObject);
            }
        }

        SetState(new SpiderKilledState(this));
    }

    private void SetClosestPlayer()
    {
        _currentCheckRadius = checkRadius;

        for (int i = 0; i < actorsManager.Players.Count; i++)
        {
            // Revisa la distancia que hay entre la araña, y alguno de los jugadores
            Vector3 targetPos = actorsManager.Players[i].transform.position;
            float dist = Vector3.Distance(targetPos, transform.position);

            // Si la distancia es lo suficientemente corta, y el transform encontrado, no es el mismo al establecido
            if (dist <= checkRadius && currentTarget != actorsManager.Players[i].transform)
            {
                _currentCheckRadius = dist;
                Debug.Log(actorsManager.Players[i].transform);
                currentTarget = actorsManager.Players[i].transform;
            }
        }
    }

    // Metodo usado para las variables de animación
    private void ConvertStringToHash()
    {
        animStandUpHash = Animator.StringToHash(ANIM_STAND_UP);
        animAttackHash = Animator.StringToHash(ANIM_ATTACK);
        animLoadingExplosion = Animator.StringToHash(ANIM_LOADING_EXPLOSION);
    }

    private void OnDied()
    {
        if (spiderType == SpiderType.Mohammed)
        {
            SetState(new LoadingExplosionState(this));
        }
        else
        {
            SetState(new SpiderKilledState(this));
        }
    }

    private void Awake()
    {
        // Variables externas
        actorsManager = FindObjectOfType<ActorsManager>();
        attackCooldown = transform.parent.Find("Timers/AttackCooldown").GetComponent<Timer>();

        // Estado explosivo
        if (spiderType == SpiderType.Mohammed)
        {
            explosionParticle = transform.Find("Explosion").gameObject;
            loadExplosion = transform.parent.Find("Timers/LoadingExplosion").GetComponent<Timer>();
            explosiveRush = transform.parent.Find("Timers/ExplosiveRush").GetComponent<Timer>();
            explosiveRush.onTimerEnded += Detonate;

            _legs = transform.parent.GetComponentsInChildren<LegTarget>();
        }
        else if (spiderType == SpiderType.Spitter)
        {
            spitCooldown = transform.parent.Find("Timers/SpitCooldown").GetComponent<Timer>();
        }

        // Variables hijas
        animator = GetComponentInChildren<Animator>();
        animationListener = GetComponentInChildren<AnimationEventsListener>();
        lookAtTarget = GetComponent<SpiderLookAtTarget>();
        hitbox = transform.Find("Hitbox").gameObject;
        if (spiderType == SpiderType.Spitter) mouth = transform.Find("Body/Head/Mouth");

        // Variables propias
        health = GetComponent<Health>();
        health.OnDie += OnDied;

        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;

        // Privadas
        _defaultSpeed = agent.speed;

        ConvertStringToHash();

        SetState(new SpiderIdleState(this));
    }

    private void Update()
    {
        SetClosestPlayer();
        _currentState.Tick();
    }

    private void OnDisable()
    {
        health.OnDie -= OnDied;

        if (spiderType == SpiderType.Mohammed)
            explosiveRush.onTimerEnded -= Detonate;
    }

    private void OnDrawGizmos()
    {
#if (UNITY_EDITOR)
        if (!EditorApplication.isPlaying)
        {
            Ray ray1 = new Ray(transform.position, Quaternion.AngleAxis(attackAngle, Vector3.up) * transform.forward * attackDistance);
            Ray ray2 = new Ray(transform.position, Quaternion.AngleAxis(-attackAngle, Vector3.up) * transform.forward * attackDistance);

            Gizmos.DrawLine(transform.position, transform.position + (ray1.direction * attackDistance));
            Gizmos.DrawLine(transform.position, transform.position + (ray2.direction * attackDistance));
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
#endif
    }
}