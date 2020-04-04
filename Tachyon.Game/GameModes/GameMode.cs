namespace Tachyon.Game.GameModes
{
    public abstract class GameMode
    {
        public GameModeInfo GameModeInfo { get; internal set; }
        
        public abstract string ShortName { get; }
        public abstract string Description { get; }
        
        protected GameMode()
        {
            GameModeInfo = new GameModeInfo
            {
                Name = Description,
                ShortName = ShortName,
                ID = 0,
                InstantiationInfo = GetType().AssemblyQualifiedName,
            };
        }
    }
}