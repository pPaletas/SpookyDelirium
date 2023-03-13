using UnityEngine;

public class SpiderSpitState : SpiderBaseState
{
    public SpiderSpitState(SpiderStateMachine stateMachine) : base(stateMachine) { }

    private Rigidbody _projectile;
    private Quaternion _targetRotation;
    private float _angle;

    private bool _isAttacking = false;

    public override void Enter()
    {
        stateMachine.animationListener.attack += OnAttack;
        stateMachine.animationListener.attackEnd += OnFinish;

        _angle = stateMachine.spitAngle * Mathf.Deg2Rad;
    }

    public override void Tick()
    {
        Spit();
        if (!_isAttacking) CheckMovementTransitions();
    }

    public override void Exit()
    {
        stateMachine.animationListener.attack -= OnAttack;
        stateMachine.animationListener.attackEnd -= OnFinish;
    }

    private Vector3 GetInitialVelocity()
    {
        // Repito los valores para evitar retrasos en la información
        Vector3 mouthPos = stateMachine.mouth.position;

        Vector3 unit = Vector3.Normalize(stateMachine.currentTarget.position - mouthPos);
        Vector3 offset = -unit * 2f;
        Vector3 targetPosition = stateMachine.currentTarget.position + offset;

        Vector3 posDifference = targetPosition - mouthPos;

        Vector3 difOnPlane = Vector3.ProjectOnPlane(posDifference, Vector3.up);
        unit = difOnPlane.normalized;

        // v0 = sqrt((g * x^2) / (2 * (cos(theta))^2 * (x * tan(theta) - y)))
        float yDif = posDifference.y;
        // Quitamos y para calcular solo la diferencia en el plano XZ
        posDifference.y = 0f;
        float xDif = posDifference.magnitude;

        float a = stateMachine.spitGravity * Mathf.Pow(xDif, 2f);
        float b = 2 * Mathf.Pow(Mathf.Cos(_angle), 2f) * Mathf.Abs(xDif * Mathf.Tan(_angle) - yDif);
        float initialiVel = Mathf.Sqrt(a / b);

        Vector3 dir = Quaternion.AngleAxis(_angle * Mathf.Rad2Deg, Vector3.Cross(unit, Vector3.up)) * unit;

        return dir.normalized * initialiVel;
    }

    private Vector3 GetUnitProjectedOnUp()
    {
        Vector3 targetPos = stateMachine.currentTarget.position;

        Vector3 mouthPos = stateMachine.mouth.position;
        Vector3 posDif = targetPos - mouthPos;

        Vector3 difOnPlane = Vector3.ProjectOnPlane(posDif, stateMachine.transform.up);
        Vector3 unit = difOnPlane.normalized;

        return unit;
    }

    private bool IsInAngle()
    {
        Vector3 mouthPos = stateMachine.mouth.position;
        Vector3 posDifference = stateMachine.currentTarget.position - mouthPos;

        // Me da el vector normalizado hacia el jugador, proyectado en el propio Y
        Vector3 difOnPlane = Vector3.ProjectOnPlane(posDifference, stateMachine.transform.up);
        Vector3 unit = difOnPlane.normalized;

        // Me da el forward projectado en el propio Y
        Vector3 fwdOnPlane = Vector3.ProjectOnPlane(stateMachine.mouth.parent.forward, stateMachine.transform.up);

        // Me da el angulo de ambos vectores proyectados
        float angleDif = Mathf.Acos(Vector3.Dot(unit, fwdOnPlane)) * Mathf.Rad2Deg;

        bool isInAngle = angleDif <= stateMachine.spitAngle;

        return isInAngle;
    }

    private void CheckMovementTransitions()
    {
        Vector3 posDifference = stateMachine.currentTarget.position - stateMachine.mouth.position;

        bool max = posDifference.magnitude <= stateMachine.spitMaxDistance;
        bool min = posDifference.magnitude > stateMachine.attackDistance;
        bool isInAngle = IsInAngle();

        if (max && min && isInAngle) return;

        if (!stateMachine.isBezierClimbPaused)
        {
            stateMachine.SetState(new SpiderFollowState(stateMachine));
        }
        else
        {
            stateMachine.SetState(new SpiderStartClimbState(stateMachine));
        }
    }

    private void Spit()
    {
        bool isCooldownStopped = stateMachine.spitCooldown.IsStopped;

        if (isCooldownStopped && !_isAttacking)
        {
            Vector3 unit = GetUnitProjectedOnUp();
            Vector3 fwdOnPlane = Vector3.ProjectOnPlane(stateMachine.mouth.parent.forward, stateMachine.transform.up);

            float angleDif = Mathf.Acos(Vector3.Dot(unit, fwdOnPlane)) * Mathf.Rad2Deg;

            stateMachine.lookAtTarget.SetLookAtActive(false);
            stateMachine.animator.SetTrigger(stateMachine.animAttackHash);
            stateMachine.spitCooldown.StartTimer();
            _isAttacking = true;
        }
    }

    private void OnAttack()
    {
        Vector3 initialVel = GetInitialVelocity();

        Quaternion id = Quaternion.identity;
        GameObject _projectileClone = GameObject.Instantiate(stateMachine.spitProjectile, stateMachine.mouth.position, id);
        _projectile = _projectileClone.GetComponent<Rigidbody>();
        _projectile.GetComponent<PoisonousProjectile>().Init(stateMachine, initialVel, _projectile);
    }

    private void OnFinish()
    {
        stateMachine.spitCooldown.StartTimer();
        stateMachine.lookAtTarget.SetLookAtActive(true);
        _isAttacking = false;
    }
}