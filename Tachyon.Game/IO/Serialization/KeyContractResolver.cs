using Humanizer;
using Newtonsoft.Json.Serialization;

namespace Tachyon.Game.IO.Serialization
{
    public class KeyContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.Underscore();
        }
    }
}
