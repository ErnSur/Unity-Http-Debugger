namespace QuickEye.RequestWatcher
{
    internal class ConsoleRequestData : RequestData
    {
        public long timestamp;
        public string stackTrace;

        public new static ConsoleRequestData Create() => Create<ConsoleRequestData>();
    }
}