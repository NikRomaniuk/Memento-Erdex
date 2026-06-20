using System;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

/// <summary>
/// Polymorphic base for GameEvent -> UnityEvent response bindings
/// </summary>
[Serializable]
public abstract class BaseBinding
{
    public abstract void Register();
    public abstract void Unregister();
}

// ====================
// PARAMETERLESS BINDING
// ====================

[Serializable]
public class Binding_Void : BaseBinding, IGameEventListener_Void
{
    [TabGroup("Event")]
    [HideLabel]
    [SerializeField] private GameEvent _event;

    [TabGroup("Response")]
    [HideLabel]
    [SerializeField] private UnityEvent _response;

    public override void Register()
    {
        if (_event == null) { return; }
        _event.RegisterListener(this);
    }

    public override void Unregister()
    {
        if (_event == null) { return; }
        _event.UnregisterListener(this);
    }

    public void OnEventInvoked()
    {
        _response?.Invoke();
    }
}

// =====================
// PARAMETERIZED BINDINGS
// =====================

// --- Int Binding ---
[Serializable]
public class Binding_Int : BaseBinding, IGameEventListener<int>
{
    [TabGroup("Event")]
    [HideLabel]
    [SerializeField] private GameEvent_Int _event;

    [TabGroup("Response")]
    [HideLabel]
    [SerializeField] private UnityEvent<int> _response;

    public override void Register()
    {
        if (_event == null) { return; }
        _event.RegisterListener(this);
    }

    public override void Unregister()
    {
        if (_event == null) { return; }
        _event.UnregisterListener(this);
    }

    public void OnEventInvoked(int value)
    {
        _response?.Invoke(value);
    }
}

// --- Float Binding ---
[Serializable]
public class Binding_Float : BaseBinding, IGameEventListener<float>
{
    [TabGroup("Event")]
    [HideLabel]
    [SerializeField] private GameEvent_Float _event;

    [TabGroup("Response")]
    [HideLabel]
    [SerializeField] private UnityEvent<float> _response;

    public override void Register()
    {
        if (_event == null) { return; }
        _event.RegisterListener(this);
    }

    public override void Unregister()
    {
        if (_event == null) { return; }
        _event.UnregisterListener(this);
    }

    public void OnEventInvoked(float value)
    {
        _response?.Invoke(value);
    }
}

// --- String Binding ---
[Serializable]
public class Binding_String : BaseBinding, IGameEventListener<string>
{
    [TabGroup("Event")]
    [HideLabel]
    [SerializeField] private GameEvent_String _event;

    [TabGroup("Response")]
    [HideLabel]
    [SerializeField] private UnityEvent<string> _response;

    public override void Register()
    {
        if (_event == null) { return; }
        _event.RegisterListener(this);
    }

    public override void Unregister()
    {
        if (_event == null) { return; }
        _event.UnregisterListener(this);
    }

    public void OnEventInvoked(string value)
    {
        _response?.Invoke(value);
    }
}

// --- GameObject Binding ---
[Serializable]
public class Binding_GameObject : BaseBinding, IGameEventListener<GameObject>
{
    [TabGroup("Event")]
    [HideLabel]
    [SerializeField] private GameEvent_GameObject _event;

    [TabGroup("Response")]
    [HideLabel]
    [SerializeField] private UnityEvent<GameObject> _response;

    public override void Register()
    {
        if (_event == null) { return; }
        _event.RegisterListener(this);
    }

    public override void Unregister()
    {
        if (_event == null) { return; }
        _event.UnregisterListener(this);
    }

    public void OnEventInvoked(GameObject value)
    {
        _response?.Invoke(value);
    }
}

// --- Collider2D Binding ---
[Serializable]
public class Binding_Collider2D : BaseBinding, IGameEventListener<Collider2D>
{
    [TabGroup("Event")]
    [HideLabel]
    [SerializeField] private GameEvent_Collider2D _event;

    [TabGroup("Response")]
    [HideLabel]
    [SerializeField] private UnityEvent<Collider2D> _response;

    public override void Register()
    {
        if (_event == null) { return; }
        _event.RegisterListener(this);
    }

    public override void Unregister()
    {
        if (_event == null) { return; }
        _event.UnregisterListener(this);
    }

    public void OnEventInvoked(Collider2D value)
    {
        _response?.Invoke(value);
    }
}
