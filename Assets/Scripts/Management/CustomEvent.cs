using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomEvent", menuName = "ScriptableObjects/CustomEvent", order = 0)]
public class CustomEvent : ScriptableObject {
    
    public delegate void EventDelegate(System.Object eventData);
    private event EventDelegate _event;

    public void RaiseEvent(System.Object eventData = null)
    {
        _event?.Invoke(eventData);
    }

    public void Subscribe(EventDelegate handler)
    {
        _event += handler;
    }

    public void Unsubscribe(EventDelegate handler)
    {
        _event -= handler;
    }
}
