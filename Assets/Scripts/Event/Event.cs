using System;
using System.Collections.Generic;
using UnityEngine;

static internal class Event
{

    //Disable the unused variable warning
#pragma warning disable 0414
    //Ensures that the MessengerHelper will be created automatically upon start of the game.
    //	static private MessengerHelper mMessengerHelper = ( new GameObject("MessengerHelper") ).AddComponent< MessengerHelper >();
#pragma warning restore 0414

    static public Dictionary<EGameEvent, Delegate> mEventTable = new Dictionary<EGameEvent, Delegate>();

    //Message handlers that should never be removed, regardless of calling Cleanup
    static public List<EGameEvent> mPermanentMessages = new List<EGameEvent>();


    //Marks a certain event as permanent.
    static public void MarkAsPermanent(EGameEvent eventType)
    {
        mPermanentMessages.Add(eventType);
    }


    static public void Cleanup()
    {
        List<EGameEvent> messagesToRemove = new List<EGameEvent>();

        foreach (KeyValuePair<EGameEvent, Delegate> pair in mEventTable)
        {
            bool wasFound = false;

            foreach (EGameEvent message in mPermanentMessages)
            {
                if (pair.Key == message)
                {
                    wasFound = true;
                    break;
                }
            }

            if (!wasFound)
                messagesToRemove.Add(pair.Key);
        }

        foreach (EGameEvent message in messagesToRemove)
        {
            mEventTable.Remove(message);
        }
    }

    static public void PrEGameEventEventTable()
    {
        Debug.Log("\t\t\t=== MESSENGER PrEGameEventEventTable ===");

        foreach (KeyValuePair<EGameEvent, Delegate> pair in mEventTable)
        {
            Debug.Log("\t\t\t" + pair.Key + "\t\t" + pair.Value);
        }

        Debug.Log("\n");
    }

    static public void OnListenerAdding(EGameEvent eventType, Delegate listenerBeingAdded)
    {
        if (!mEventTable.ContainsKey(eventType))
        {
            mEventTable.Add(eventType, null);
        }

        Delegate d = mEventTable[eventType];
        if (d != null && d.GetType() != listenerBeingAdded.GetType())
        {
            throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
        }
    }

    static public void OnListenerRemoving(EGameEvent eventType, Delegate listenerBeingRemoved)
    {
        if (mEventTable.ContainsKey(eventType))
        {
            Delegate d = mEventTable[eventType];

            if (d == null)
            {
                throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
            }
            else if (d.GetType() != listenerBeingRemoved.GetType())
            {
                throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
            }
        }
        else
        {
            throw new ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
        }
    }

    static public void OnListenerRemoved(EGameEvent eventType)
    {
        if (mEventTable[eventType] == null)
        {
            mEventTable.Remove(eventType);
        }
    }

    static public void OnBroadcasting(EGameEvent eventType)
    {
#if REQUIRE_LISTENER
        if (!mEventTable.ContainsKey(eventType)) {
        }
#endif
    }

    static public BroadcastException CreateBroadcastSignatureException(EGameEvent eventType)
    {
        return new BroadcastException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
    }

    public class BroadcastException : Exception
    {
        public BroadcastException(string msg)
            : base(msg)
        {
        }
    }

    public class ListenerException : Exception
    {
        public ListenerException(string msg)
            : base(msg)
        {
        }
    }

    //No parameters
    static public void AddListener(EGameEvent eventType, Callback handler)
    {
        OnListenerAdding(eventType, handler);
        mEventTable[eventType] = (Callback)mEventTable[eventType] + handler;
    }

    //Single parameter
    static public void AddListener<T>(EGameEvent eventType, Callback<T> handler)
    {
        OnListenerAdding(eventType, handler);
        mEventTable[eventType] = (Callback<T>)mEventTable[eventType] + handler;
    }

    //Two parameters
    static public void AddListener<T, U>(EGameEvent eventType, Callback<T, U> handler)
    {
        OnListenerAdding(eventType, handler);
        mEventTable[eventType] = (Callback<T, U>)mEventTable[eventType] + handler;
    }

    //Three parameters
    static public void AddListener<T, U, V>(EGameEvent eventType, Callback<T, U, V> handler)
    {
        OnListenerAdding(eventType, handler);
        mEventTable[eventType] = (Callback<T, U, V>)mEventTable[eventType] + handler;
    }

    //Four parameters
    static public void AddListener<T, U, V, X>(EGameEvent eventType, Callback<T, U, V, X> handler)
    {
        OnListenerAdding(eventType, handler);
        mEventTable[eventType] = (Callback<T, U, V, X>)mEventTable[eventType] + handler;
    }

    //No parameters
    static public void RemoveListener(EGameEvent eventType, Callback handler)
    {
        OnListenerRemoving(eventType, handler);
        mEventTable[eventType] = (Callback)mEventTable[eventType] - handler;
        OnListenerRemoved(eventType);
    }

    //Single parameter
    static public void RemoveListener<T>(EGameEvent eventType, Callback<T> handler)
    {
        OnListenerRemoving(eventType, handler);
        mEventTable[eventType] = (Callback<T>)mEventTable[eventType] - handler;
        OnListenerRemoved(eventType);
    }

    //Two parameters
    static public void RemoveListener<T, U>(EGameEvent eventType, Callback<T, U> handler)
    {
        OnListenerRemoving(eventType, handler);
        mEventTable[eventType] = (Callback<T, U>)mEventTable[eventType] - handler;
        OnListenerRemoved(eventType);
    }

    //Three parameters
    static public void RemoveListener<T, U, V>(EGameEvent eventType, Callback<T, U, V> handler)
    {
        OnListenerRemoving(eventType, handler);
        mEventTable[eventType] = (Callback<T, U, V>)mEventTable[eventType] - handler;
        OnListenerRemoved(eventType);
    }

    //Four parameters
    static public void RemoveListener<T, U, V, X>(EGameEvent eventType, Callback<T, U, V, X> handler)
    {
        OnListenerRemoving(eventType, handler);
        mEventTable[eventType] = (Callback<T, U, V, X>)mEventTable[eventType] - handler;
        OnListenerRemoved(eventType);
    }

    //No parameters
    static public void Broadcast(EGameEvent eventType)
    {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);

        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            Callback callback = d as Callback;

            if (callback != null)
            {
                callback();
            }
            else
            {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }

    static public void SendEvent(CEvent evt)
    {
        Broadcast<CEvent>(evt.GetEventId(), evt);
    }

    //Single parameter
    static public void Broadcast<T>(EGameEvent eventType, T arg1)
    {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);

        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            Callback<T> callback = d as Callback<T>;

            if (callback != null)
            {
                callback(arg1);
            }
            else
            {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }

    //Two parameters
    static public void Broadcast<T, U>(EGameEvent eventType, T arg1, U arg2)
    {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);

        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            Callback<T, U> callback = d as Callback<T, U>;

            if (callback != null)
            {
                callback(arg1, arg2);
            }
            else
            {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }

    //Three parameters
    static public void Broadcast<T, U, V>(EGameEvent eventType, T arg1, U arg2, V arg3)
    {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);

        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            Callback<T, U, V> callback = d as Callback<T, U, V>;

            if (callback != null)
            {
                callback(arg1, arg2, arg3);
            }
            else
            {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }

    //Four parameters
    static public void Broadcast<T, U, V, X>(EGameEvent eventType, T arg1, U arg2, V arg3, X arg4)
    {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);

        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            Callback<T, U, V, X> callback = d as Callback<T, U, V, X>;

            if (callback != null)
            {
                callback(arg1, arg2, arg3, arg4);
            }
            else
            {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }
}
