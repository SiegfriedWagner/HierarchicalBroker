using System;

namespace HierachicalBroker.Logging
{
    [Flags]
    public enum LoggedEvents
    {
        None = 0,
        Subscribe = 1,
        Invoke = 2,
        Unsubscribe = 3,
        CancelInvoke = 4, 
        All = Subscribe | Invoke | Unsubscribe | CancelInvoke
    }
}