namespace TTGJ.Serialize {
     public interface ISerializable<T> where T : ISerializable<T>
    {
        string Serialize(T data);
        void Deserialize(string date);
    }
}