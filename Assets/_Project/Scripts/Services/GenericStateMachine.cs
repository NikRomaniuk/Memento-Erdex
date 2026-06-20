// =========================
// HIERARCHICAL STATE MACHINE
// =========================

using System;
using UnityEngine;

public abstract class State<T> where T : class
{
    protected T owner;
    protected StateMachine<T> stateMachine;
    protected State<T> superState;

    public State(T owner, StateMachine<T> stateMachine, State<T> superState = null)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;
        this.superState = superState;
    }

    public string Name => GetType().Name;

    public virtual void Enter() { superState?.Enter(); }
    public virtual void LogicUpdate() { superState?.LogicUpdate(); }
    public virtual void PhysicsUpdate() { superState?.PhysicsUpdate(); }
    public virtual void Exit() { superState?.Exit(); }
}

public class StateMachine<T> where T : class
{
    public State<T> CurrentState { get; private set; }

    public void Initialize(State<T> startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(State<T> newState)
    {
        if (CurrentState == newState) return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}