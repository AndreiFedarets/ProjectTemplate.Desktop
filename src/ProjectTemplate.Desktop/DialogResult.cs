namespace ProjectTemplate.Desktop
{
    public sealed class DialogResult<T>
    {
        public DialogResult(bool success, T value)
        {
            Success = success;
            Value = success ? value : default(T);
        }

        public bool Success { get; private set; }

        public T Value { get; private set; }
    }
}
