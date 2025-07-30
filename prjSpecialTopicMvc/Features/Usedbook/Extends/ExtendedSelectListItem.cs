using Microsoft.AspNetCore.Mvc.Rendering;

namespace prjSpecialTopicMvc.Features.Usedbook.Extends
{
    public class ExtendedSelectListItem : SelectListItem
    {
        public string? Description { get; set; }
    }

    public static class HttpRequestExtensions
    {
        public static string GetBaseUrl(this HttpRequest request)
            => $"{request.Scheme}://{request.Host}{request.PathBase}";
    }
}
