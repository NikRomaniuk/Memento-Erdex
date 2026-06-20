using UnityEngine;

public partial class SpectatorManager
{
    // =========
    // IDLE STATE
    // =========

    private class IdleState : State<SpectatorManager>
    {
        public IdleState(SpectatorManager owner, StateMachine<SpectatorManager> stateMachine)
            : base(owner, stateMachine) { }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (stateMachine.CurrentState != this) { return; }

            if (owner.Input.MoveDirection != Vector2.zero)
            {
                stateMachine.ChangeState(owner._moveState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (stateMachine.CurrentState != this) { return; }

            if (owner.Movement.IsMoving())
            {
                owner.Movement.DecelerateMovement(owner.Data.DecelerationSpeed);
            }
            else
            {
                owner.Movement.StopMovement();
            }
        }
    }

    // =========
    // MOVE STATE
    // =========

    private class MoveState : State<SpectatorManager>
    {
        public MoveState(SpectatorManager owner, StateMachine<SpectatorManager> stateMachine)
            : base(owner, stateMachine) { }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (stateMachine.CurrentState != this) { return; }

            if (owner.Input.MoveDirection == Vector2.zero)
            {
                stateMachine.ChangeState(owner._idleState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (stateMachine.CurrentState != this) { return; }

            Vector2 cameraMult = owner.GetCameraOffsetMultiplier(owner.Input.MoveDirection);
            float zoomMult = owner.GetZoomSpeedMultiplier();
            Vector2 targetVelocity = owner.Input.MoveDirection * owner.Data.MoveSpeed;
            targetVelocity.Scale(cameraMult * zoomMult);
            owner.Movement.AccelerateMovement(owner.Data.AccelerationSpeed, targetVelocity);
        }
    }
}