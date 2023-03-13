public class LoadingExplosionState : SpiderBaseState
{
    public LoadingExplosionState(SpiderStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.MultiplySpeed(stateMachine.explosionModeSpeedMultiplier);
        stateMachine.agent.isStopped = true;
        stateMachine.animator.SetBool(stateMachine.animLoadingExplosion, true);
        stateMachine.loadExplosion.StartTimer();
        stateMachine.lookAtTarget.SetLookAtActive(false);
    }

    public override void Tick()
    {
        if (stateMachine.loadExplosion.IsStopped)
        {
            OnLoadingEnded();
        }
    }

    public override void Exit()
    {
        stateMachine.lookAtTarget.SetLookAtActive(true);
    }

    private void OnLoadingEnded()
    {
        stateMachine.animator.SetBool(stateMachine.animLoadingExplosion, false);
        stateMachine.isInRushMode = true;
        stateMachine.explosiveRush.StartTimer();

        if (stateMachine.isBezierClimbPaused)
        {
            stateMachine.SetState(new SpiderStartClimbState(stateMachine));
        }
        else
        {
            stateMachine.SetState(new SpiderFollowState(stateMachine));
        }
    }
}