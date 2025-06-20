using Microsoft.Net.Http.Headers;

namespace BlogDoFt.DeveloperToolbox.Api.Extensions;

public static class MediaTypeHeaderExtensions
{
    public static bool Includes(this IList<MediaTypeHeaderValue> headers, string mediaType)
    {
        if (string.IsNullOrEmpty(mediaType))
        {
            return false;
        }

        return headers.Any(h => string.Equals(h.MediaType.Value, mediaType, StringComparison.OrdinalIgnoreCase));
    }
}
