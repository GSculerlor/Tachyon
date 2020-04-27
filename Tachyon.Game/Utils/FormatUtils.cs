namespace Tachyon.Game.Utils
{
    public static class FormatUtils
    {
        /// <summary>
        /// Turns the provided accuracy into a percentage with 2 decimal places.
        /// </summary>
        /// <param name="accuracy">The accuracy to be formatted</param>
        /// <returns>formatted accuracy in percentage</returns>
        public static string FormatAccuracy(this double accuracy) => $"{accuracy:0.00%}";

        /// <summary>
        /// Turns the provided accuracy into a percentage with 2 decimal places.
        /// </summary>
        /// <param name="accuracy">The accuracy to be formatted</param>
        /// <returns>formatted accuracy in percentage</returns>
        public static string FormatAccuracy(this decimal accuracy) => $"{accuracy:0.00}%";
    }
}
