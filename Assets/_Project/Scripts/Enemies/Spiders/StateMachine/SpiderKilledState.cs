using UnityEngine;

public class SpiderKilledState : SpiderBaseState
{
    public SpiderKilledState(SpiderStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.transform.parent.gameObject.SetActive(false);
        GameObject.Destroy(stateMachine.transform.parent.gameObject);
    }
}