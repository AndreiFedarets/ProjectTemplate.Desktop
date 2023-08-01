namespace ProjectTemplate
{
    public interface IBatchOperation
    {
        int TotalCount { get; }

        int CompletedCount { get; }

        bool IsCompleted { get; }

        void Cancel();
    }

    public interface IBatchOperation<T> : IBatchOperation
    {
        IEnumerable<T> Result { get; }
    }
}
