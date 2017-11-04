using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pomelo.Marked;
using YuukoBlog.Filters;
using YuukoBlog.Models;

namespace YuukoBlog.Controllers
{
    public class AdminController : BaseController
    {
        [AdminRequired]
        [HttpGet]
        [Route("Admin/Index")]
        public IActionResult Index()
        {
            return this.View();
        }

        [AdminRequired]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Index")]
        public IActionResult Index(Config config)
        {
            this.Configuration["Account"] = config.Account;
            this.Configuration["Password"] = config.Password;
            this.Configuration["Site"] = config.Site;
            this.Configuration["Description"] = config.Description;
            this.Configuration["Disqus"] = config.Disqus;
            this.Configuration["AvatarUrl"] = config.AvatarUrl;
            this.Configuration["AboutUrl"] = config.AboutUrl;
            this.Configuration["BlogRoll:GitHub"] = config.GitHub;
            this.Configuration["BlogRoll:Follower"] = config.Follower.ToString();
            this.Configuration["BlogRoll:Following"] = config.Following.ToString();

            return this.RedirectToAction("Index", "Admin");
        }

        [GuestRequired]
        public IActionResult Login()
        {
            return this.View();
        }

        [GuestRequired]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            var account = this.Configuration["Account"];

            if (username == this.Configuration["Account"] && password == this.Configuration["Password"])
            {
                this.HttpContext.Session.SetString("Admin", "true");

                return this.RedirectToAction("Index", "Admin");
            }
            else
            {
                return this.View();
            }
        }

        [AdminRequired]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Post/Edit")]
        public IActionResult PostEdit(string id, string newId, string tags, bool isPage, string title, Guid? catalog, string content)
        {
            var post = this.DB.Posts
                .Include(x => x.Tags)
                .Where(x => x.Url == id)
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

            var summary = string.Empty;
            var flag = false;

            if (content != null)
            {
                var temp = content.Split('\n');

                if (temp.Count() > 16)
                {
                    for (var i = 0; i < 16; i++)
                    {
                        if (temp[i].IndexOf("```") == 0)
                        {
                            flag = !flag;
                        }

                        summary += temp[i] + '\n';
                    }

                    if (flag)
                    {
                        summary += "```\r\n";
                    }

                    summary += $"\r\n[{this.SR["Read More"]} »](/post/{newId})";
                }
                else
                {
                    summary = content;
                }
            }

            foreach (var tag in post.Tags)
            {
                this.DB.PostTags.Remove(tag);
            }

            post.Url = newId;
            post.Summary = summary;
            post.Title = title;
            post.Content = content;
            post.CatalogId = catalog;
            post.IsPage = isPage;

            if (!string.IsNullOrEmpty(tags))
            {
                foreach (var tag in tags.Split(','))
                {
                    post.Tags.Add(new PostTag { PostId = post.Id, Tag = tag.Trim(' ') });
                }
            }

            this.DB.SaveChanges();

            return this.Content(Instance.Parse(content));
        }

        [AdminRequired]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Post/Delete")]
        public IActionResult PostDelete(string id)
        {
            var post = this.DB.Posts
                .Include(x => x.Tags)
                .Where(x => x.Url == id).SingleOrDefault();

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

            foreach (var tag in post.Tags)
            {
                this.DB.PostTags.Remove(tag);
            }

            this.DB.Posts.Remove(post);
            this.DB.SaveChanges();

            return this.RedirectToAction("Index", "Home");
        }

        [AdminRequired]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Post/New")]
        public IActionResult PostNew()
        {
            var post = new Post
            {
                Id = Guid.NewGuid(),
                Url = Guid.NewGuid().ToString().Substring(0, 8),
                Title = this.SR["Untitled Post"],
                Content = string.Empty,
                Summary = string.Empty,
                CatalogId = null,
                IsPage = false,
                Time = DateTime.Now,
            };

            this.DB.Posts.Add(post);
            this.DB.SaveChanges();

            return this.RedirectToAction("Post", "Post", new { id = post.Url });
        }

        [AdminRequired]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            this.HttpContext.Session.Clear();

            return this.RedirectToAction("Index", "Home");
        }

        [AdminRequired]
        public IActionResult Catalog()
        {
            return this.View(this.DB.Catalogs.OrderByDescending(x => x.PRI).ToList());
        }

        [AdminRequired]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Catalog/Delete")]
        public IActionResult CatalogDelete(string id)
        {
            var catalog = this.DB.Catalogs.Where(x => x.Url == id).SingleOrDefault();

            if (catalog == null)
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

            this.DB.Catalogs.Remove(catalog);
            this.DB.SaveChanges();

            return this.Content("true");
        }

        [AdminRequired]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Catalog/Edit")]
        public IActionResult CatalogEdit(string id, string newId, string title, int pri)
        {
            var catalog = this.DB.Catalogs.Where(x => x.Url == id).SingleOrDefault();

            if (catalog == null)
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

            catalog.Url = newId;
            catalog.Title = title;
            catalog.PRI = pri;

            this.DB.SaveChanges();

            return this.Content("true");
        }

        [AdminRequired]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Catalog/New")]
        public IActionResult CatalogNew()
        {
            var catalog = new Catalog
            {
                Url = Guid.NewGuid().ToString().Substring(0, 8),
                PRI = 0,
                Title = this.SR["New Catalog"],
            };

            this.DB.Catalogs.Add(catalog);
            this.DB.SaveChanges();

            return this.RedirectToAction("Catalog", "Admin");
        }
    }
}
