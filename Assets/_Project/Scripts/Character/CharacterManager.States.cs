using UnityEngine;

public partial class CharacterManager 
{
    // ============
    // GROUND STATES
    // ============

    private class GroundState : State<CharacterManager>
    {
        public GroundState(CharacterManager owner, StateMachine<CharacterManager> stateMachine)
            : base(owner, stateMachine) { }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!owner.GroundCheck.HasSurfaceBelow())
            {
                stateMachine.ChangeState(owner._airFallState);
                return;
            }

            if (owner.Input.JumpTriggered && owner.Jump.CanJump())
            {
                owner.Jump.Jump();
                stateMachine.ChangeState(owner._airJumpState);
                return;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }

    private class GroundIdleState : State<CharacterManager>
    {
        public GroundIdleState(CharacterManager owner, StateMachine<CharacterManager> stateMachine,
            State<CharacterManager> superState) : base(owner, stateMachine, superState) { }

        public override void Enter()
        {
            base.Enter();
            owner.Animation.PlayIdle();
            owner.Animation.AlignIdle(owner.DirectionFacing); // Align Character parts with direction
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (stateMachine.CurrentState != this) { return; }

            if (owner.Input.Horizontal != 0)
            {
                stateMachine.ChangeState(owner._groundMoveState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (stateMachine.CurrentState != this) { return; }

            if (owner.Movement.IsMovingHorizontally())
            {
                owner.Movement.DecelerateHorizontalMovement(owner.Data.DecelerationSpeed);
            }
            else
            {
                owner.Movement.StopHorizontalMovement();
            }
        }
    }

    private class GroundMoveState : State<CharacterManager>
    {
        public GroundMoveState(CharacterManager owner, StateMachine<CharacterManager> stateMachine,
            State<CharacterManager> superState) : base(owner, stateMachine, superState) { }

        private float _moveInput;

        public override void Enter()
        {
            base.Enter();
            owner.Animation.PlayWalk();
        }
        
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (stateMachine.CurrentState != this) { return; }

            _moveInput = owner.Input.Horizontal;

            if (_moveInput == 0)
            {
                stateMachine.ChangeState(owner._groundIdleState);
                return;
            }

            owner.SetDirectionFacing((int)Mathf.Sign(_moveInput));
            owner.Animation.AlignWalk(owner.DirectionFacing); // Align Character parts with direction
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (stateMachine.CurrentState != this) { return; }

            float targetSpeed = _moveInput * owner.Data.MoveSpeedOnGround;
            owner.Movement.AccelerateHorizontalMovement(owner.Data.AccelerationSpeed, targetSpeed);
        }
    }

    // =========
    // AIR STATES
    // =========

    private class AirState : State<CharacterManager>
    {
        public AirState(CharacterManager owner, StateMachine<CharacterManager> stateMachine)
            : base(owner, stateMachine) { }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (owner.GroundCheck.HasSurfaceBelow())
            {
                // Don't snap to ground while ascending from a jump
                if (owner.RB.linearVelocity.y <= 0f)
                {
                    stateMachine.ChangeState(owner._groundIdleState);
                    return;
                }
            }

            if (owner.RB.linearVelocity.y < 0f && stateMachine.CurrentState != owner._airFallState)
            {
                stateMachine.ChangeState(owner._airFallState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }

    private class AirJumpState : State<CharacterManager>
    {
        public AirJumpState(CharacterManager owner, StateMachine<CharacterManager> stateMachine,
            State<CharacterManager> superState) : base(owner, stateMachine, superState) { }

        private float _moveInput;

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (stateMachine.CurrentState != this) { return; }

            if (!owner.Input.JumpHeld)
            {
                owner.Jump.CutJump();
            }

            _moveInput = owner.Input.Horizontal;
            owner.SetDirectionFacing((int)Mathf.Sign(_moveInput));
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (stateMachine.CurrentState != this) { return; }

            float targetSpeed = _moveInput * owner.Data.MoveSpeedOnGround;
            owner.Movement.AccelerateHorizontalMovement(owner.Data.AccelerationSpeed, targetSpeed);
        }
    }

    private class AirFallState : State<CharacterManager>
    {
        public AirFallState(CharacterManager owner, StateMachine<CharacterManager> stateMachine,
            State<CharacterManager> superState) : base(owner, stateMachine, superState) { }

        private float _moveInput;

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (stateMachine.CurrentState != this) { return; }

            _moveInput = owner.Input.Horizontal;
            owner.SetDirectionFacing((int)Mathf.Sign(_moveInput));
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (stateMachine.CurrentState != this) { return; }

            float targetSpeed = _moveInput * owner.Data.MoveSpeedInAir;
            owner.Movement.ClampFallSpeed(owner.Data.MaxFallSpeed);
            owner.Movement.AccelerateHorizontalMovement(owner.Data.AccelerationSpeed, targetSpeed);
        }
    }

    private class AirLeapState : State<CharacterManager>
    {
        public AirLeapState(CharacterManager owner, StateMachine<CharacterManager> stateMachine,
            State<CharacterManager> superState) : base(owner, stateMachine, superState) { }
    }

    // ===========
    // FOCUS STATES
    // ===========

    private class FocusState : State<CharacterManager>
    {
        public FocusState(CharacterManager owner, StateMachine<CharacterManager> stateMachine)
            : base(owner, stateMachine) { }
    }

    private class FocusGroundState : State<CharacterManager>
    {
        public FocusGroundState(CharacterManager owner, StateMachine<CharacterManager> stateMachine,
            State<CharacterManager> superState) : base(owner, stateMachine, superState) { }
    }

    private class FocusAirState : State<CharacterManager>
    {
        public FocusAirState(CharacterManager owner, StateMachine<CharacterManager> stateMachine,
            State<CharacterManager> superState) : base(owner, stateMachine, superState) { }
    }
}
