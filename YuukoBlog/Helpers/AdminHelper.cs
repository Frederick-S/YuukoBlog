using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class AdminHelper
    {
        public static bool IsAdmin(this IHtmlHelper self)
        {
            return self.ViewContext.HttpContext.Session.GetString("Admin") == "true";
        }
    }
}
