using System;

namespace HierarchicBroker.Logging
{
    [Flags]
    public enum LoggedEvents
    {
        None = 0,
        Subscribe = 1,
        Invoke = 2,
        Unsubscribe = 4,
        CancelInvoke = 8, 
        All = Subscribe | Invoke | Unsubscribe | CancelInvoke
    }
}