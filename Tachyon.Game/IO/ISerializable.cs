namespace Tachyon.Game.IO
{
    public interface ISerializable
    {
        void ReadFromStream(SerializationReader sr);
        void WriteToStream(SerializationWriter sw);
    }
}
