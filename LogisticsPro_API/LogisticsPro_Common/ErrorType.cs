using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace LogisticsPro_Common.Common
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ErrorType
    {
        INTERNAL,
        BAD_REQUEST,
        NOT_FOUND,
        FORBIDDEN,
        UNAUTHORIZED
    }
}
