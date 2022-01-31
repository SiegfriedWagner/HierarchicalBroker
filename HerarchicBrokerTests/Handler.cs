namespace HierarchiBrokerTests
{
    public static class Handlers<T>
    {
        public static void EmptyHandler(object sender, in T args) {}
        public static void EmptyCancelingHandler(object sender, in T args, ref bool canceling) {}
    }
}