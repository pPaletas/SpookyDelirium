using System.Collections.Generic;
using UnityEngine;

public class SpiderIdleState : SpiderBaseState
{
    private bool _detectedPlayer = false;

    public SpiderIdleState(SpiderStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        stateMachine.animationListener.standUpEnd += OnStandUpAnimationEnd;
    }

    public override void Tick()
    {
        base.Tick();
        CheckTransitions();
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.hitbox.SetActive(true);
        stateMachine.animationListener.standUpEnd -= OnStandUpAnimationEnd;
    }

    protected virtual void CheckTransitions()
    {
        if (/*!_detectedPlayer && CheckForAnyTarget() &&*/ stateMachine.canGoForPlayer)
        {
            stateMachine.animator.SetTrigger(stateMachine.animStandUpHash);
        }
    }

    private bool CheckForAnyTarget()
    {
        List<GameObject> players = stateMachine.actorsManager.Players;

        for (int i = 0; i < players.Count; i++)
        {
            // Revisa la distancia que hay entre la araña, y alguno de los jugadores
            float dist = Vector3.Distance(players[i].transform.position, stateMachine.transform.position);

            if (dist <= stateMachine.checkRadius)
            {
                _detectedPlayer = true;
                return true;
            }
        }

        return false;
    }

    private void OnStandUpAnimationEnd()
    {
        stateMachine.SetState(new SpiderFollowState(stateMachine));
    }
}