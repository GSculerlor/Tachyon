namespace Tachyon.Game.IO.Legacy
{
    public interface ILegacySerializable
    {
        void ReadFromStream(SerializationReader sr);
        void WriteToStream(SerializationWriter sw);
    }
}
