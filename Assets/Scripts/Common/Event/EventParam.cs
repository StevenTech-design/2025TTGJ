namespace TTGJ.Common
{
    public class EventParam
    {
        public EventType eventType;
    }
    public class EventParam<T> : EventParam
    {
        public T param;
    }
}
