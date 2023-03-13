public class SpiderBaseState
{
    protected SpiderStateMachine stateMachine;

    public SpiderBaseState(SpiderStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Tick() { }
    public virtual void Exit() { }
}
