using System.Collections.Generic;
using YuukoBlog.Models;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class TagHelper
    {
        public static string TagSerialize(this IHtmlHelper self, IEnumerable<PostTag> tags)
        {
            return string.Join(", ", tags).TrimEnd(' ');
        }
    }
}
