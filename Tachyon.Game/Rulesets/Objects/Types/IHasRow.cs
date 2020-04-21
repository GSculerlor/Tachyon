namespace Tachyon.Game.Rulesets.Objects.Types
{
    /// <summary>
    /// A type of hit object which lies in one of a number of predetermined rows.
    /// </summary>
    public interface IHasRow
    {
        /// <summary>
        /// The row which the hit object lies in.
        /// </summary>
        int Row { get; }
    }
}
