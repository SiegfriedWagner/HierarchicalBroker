namespace HierarchicalMediators
{
    public struct CancellableEventArgs<T> : ICancellable, IEventArgs
    {                                           
        public bool Cancel { get; set; }
        public T EventArg { get; }

        public CancellableEventArgs(ref T args)
        {
            EventArg = args;
            Cancel = false;
        }
    }
}