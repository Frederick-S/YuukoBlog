namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class RawHelper
    {
        public static bool IsRaw(this IHtmlHelper self)
        {
            return self.ViewContext.HttpContext.Request.Query["raw"] == "true";
        }
    }
}
