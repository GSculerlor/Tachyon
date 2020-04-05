namespace Tachyon.Game.Database
{
    public interface ISoftDelete
    {
        bool DeletePending { get; set; }
    }
}
