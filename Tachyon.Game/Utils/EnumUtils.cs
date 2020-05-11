using System;

namespace Tachyon.Game.Utils
{
    public static class EnumUtils
    {
        public static T GetRandom<T>() where T : struct, IConvertible
        {
            Random random = new Random();
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            var a = Enum.GetValues(enumType);

            return (T)a.GetValue(random.Next(a.Length));
        }
    }
}
