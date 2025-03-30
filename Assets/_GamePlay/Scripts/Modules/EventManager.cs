using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class EventManager
{
    public static Dictionary<string, Action<object>> _events = new();

    public static void Resgister(string eventID, Action<object> action)
    {
        if(!_events.ContainsKey(eventID) )
        {
            _events.Add(eventID, action);
        }
        else
        {
            _events[eventID] += action;
        }
    }

    public static void UnResgister(string eventID, Action<object> action)
    {
        if(_events.ContainsKey(eventID) )
        {
            _events[eventID] -= action;
        }
    }

    public static void Emit(string eventID, object obj = null)
    {
        if(_events.ContainsKey(eventID))
        {
            _events[eventID]?.Invoke(obj);
        }
    }
}
