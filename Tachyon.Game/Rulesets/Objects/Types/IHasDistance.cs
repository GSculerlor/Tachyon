namespace Tachyon.Game.Rulesets.Objects.Types
{
    /// <summary>
    /// A HitObject that has a positional length.
    /// </summary>
    public interface IHasDistance : IHasEndTime
    {
        /// <summary>
        /// The positional length of the HitObject.
        /// </summary>
        double Distance { get; }
    }
}
