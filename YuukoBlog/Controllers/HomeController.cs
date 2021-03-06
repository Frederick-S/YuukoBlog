﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YuukoBlog.Models;

namespace YuukoBlog.Controllers
{
    public class HomeController : BaseController
    {
        [Route("{p:int?}")]
        public IActionResult Index(int p = 1)
        {
            var posts = this.DB.Posts
                .Include(x => x.Catalog)
                .Include(x => x.Tags)
                .Where(x => !x.IsPage)
                .OrderByDescending(x => x.Time);

            return this.PagedView<PostViewModel, Post>(posts, 5, "Home");
        }

        [Route("{year:int}/{month:int}/{p:int?}")]
        public IActionResult Calendar(int year, int month, int p = 1)
        {
            var begin = new DateTime(year, month, 1);
            var end = begin.AddMonths(1);
            var posts = this.DB.Posts
                .Include(x => x.Tags)
                .Include(x => x.Catalog)
                .Where(x => !x.IsPage)
                .Where(x => x.Time >= begin && x.Time <= end)
                .OrderByDescending(x => x.Time);

            return this.PagedView<PostViewModel, Post>(posts, 5, "Home");
        }

        [Route("Catalog/{id}/{p:int?}")]
        public IActionResult Catalog(string id, int p = 1)
        {
            var catalog = this.DB.Catalogs
                .Where(x => x.Url == id)
                .SingleOrDefault();

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

            this.ViewBag.Position = catalog.Url;

            var posts = this.DB.Posts.Include(x => x.Tags)
                .Include(x => x.Catalog)
                .Where(x => !x.IsPage && x.CatalogId == catalog.Id)
                .OrderByDescending(x => x.Time);

            return this.PagedView<PostViewModel, Post>(posts, 5, "Home");
        }

        [Route("Tag/{tag}/{p:int?}")]
        public IActionResult Tag(string tag, int p = 1)
        {
            var posts = this.DB.Posts.Include(x => x.Tags)
                 .Include(x => x.Catalog)
                 .Where(x => !x.IsPage)
                 .Where(x => x.Tags.Any(y => y.Tag == tag))
                 .OrderByDescending(x => x.Time);

            return this.PagedView<PostViewModel, Post>(posts, 5, "Home");
        }

        [Route("Search/{id}/{p:int?}")]
        public IActionResult Search(string id, int p = 1)
        {
            var posts = this.DB.Posts.Include(x => x.Tags)
                    .Include(x => x.Catalog)
                    .Where(x => !x.IsPage)
                    .Where(x => x.Title.Contains(id) || id.Contains(x.Title))
                    .OrderByDescending(x => x.Time);

            return this.PagedView<PostViewModel, Post>(posts, 5, "Home");
        }

        public IActionResult Template(string folder, [FromHeader] string referer)
        {
            this.Cookies["ASPNET_TEMPLATE"] = folder;

            return this.Redirect(referer ?? "/");
        }
    }
}
