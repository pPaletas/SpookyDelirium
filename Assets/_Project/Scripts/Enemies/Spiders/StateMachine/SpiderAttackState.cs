using Unity.FPS.Game;
using UnityEngine;

public class SpiderAttackState : SpiderBaseState
{
    public SpiderAttackState(SpiderStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.animationListener.attack += OnAttack;
        stateMachine.animationListener.attackEnd += OnFinish;
        stateMachine.animator.SetTrigger(stateMachine.animAttackHash);
        stateMachine.lookAtTarget.SetLookAtActive(false);
    }

    public override void Exit()
    {
        stateMachine.animationListener.attack -= OnAttack;
        stateMachine.animationListener.attackEnd -= OnFinish;
        stateMachine.lookAtTarget.SetLookAtActive(true);
    }

    private void OnAttack()
    {
        for (int i = 0; i < stateMachine.actorsManager.Players.Count; i++)
        {
            Vector3 plrPos = stateMachine.actorsManager.Players[i].transform.position;
            Vector3 plrDir = (plrPos - stateMachine.transform.position).normalized;

            float dist = Vector3.Distance(plrPos, stateMachine.transform.position);
            float angle = Mathf.Acos(Vector3.Dot(plrDir, stateMachine.transform.forward)) * Mathf.Rad2Deg;

            if (dist <= stateMachine.attackDistance && angle <= stateMachine.attackAngle)
            {
                float dmg = stateMachine.attackDamage;
                stateMachine.actorsManager.Players[i].GetComponent<Health>().TakeDamage(dmg, stateMachine.gameObject);
            }
        }
    }

    private void OnFinish()
    {
        if (!stateMachine.isBezierClimbPaused)
        {
            stateMachine.SetState(new SpiderFollowState(stateMachine));
        }
        else
        {
            stateMachine.SetState(new SpiderStartClimbState(stateMachine));
        }
    }
}