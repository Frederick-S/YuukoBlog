using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace YuukoBlog.Controllers
{
    public class PostController : BaseController
    {
        [Route("Post/{id}")]
        public IActionResult Post(string id)
        {
            var post = this.DB.Posts
                .Include(x => x.Catalog)
                .Include(x => x.Tags)
                .Where(x => x.Url == id && !x.IsPage)
                .SingleOrDefault();

            if (post == null)
            {
                return this.Prompt(x =>
                {
                    x.StatusCode = 404;
                    x.Title = this.SR["Not Found"];
                    x.Details = this.SR["The resources have not been found, please check your request."];
                    x.RedirectUrl = this.Url.Link("default", new { controller = "Home", action = "Index" });
                    x.RedirectText = this.SR["Back to home"];
                });
            }

            this.ViewBag.Title = post.Title;
            this.ViewBag.Position = post.CatalogId != null ? post.Catalog.Url : "home";

            return this.View(post);
        }

        [Route("{id}")]
        public IActionResult Page(string id)
        {
            var post = this.DB.Posts
                .Where(x => x.Url == id && x.IsPage)
                .SingleOrDefault();

            if (post == null)
            {
                return this.Prompt(x =>
                {
                    x.StatusCode = 404;
                    x.Title = this.SR["Not Found"];
                    x.Details = this.SR["The resources have not been found, please check your request."];
                    x.RedirectUrl = this.Url.Link("default", new { controller = "Home", action = "Index" });
                    x.RedirectText = this.SR["Back to home"];
                });
            }

            this.ViewBag.Title = post.Title;
            this.ViewBag.Position = post.CatalogId.HasValue ? post.Catalog.Url : "home";

            return this.View("Post", post);
        }
    }
}
