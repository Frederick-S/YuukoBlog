using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YuukoBlog.Models;

namespace YuukoBlog.Controllers
{
    public class BaseController : BaseController<BlogContext>
    {
        public override void Prepare()
        {
            base.Prepare();

            // Building Constants
            this.ViewBag.Position = "home";
            this.ViewBag.IsPost = false;
            this.ViewBag.Description = this.Configuration["Description"];
            this.ViewBag.Title = this.Configuration["Site"];
            this.ViewBag.Site = this.Configuration["Site"];
            this.ViewBag.AboutUrl = this.Configuration["AboutUrl"];
            this.ViewBag.AvatarUrl = this.Configuration["AvatarUrl"];
            this.ViewBag.Disqus = this.Configuration["Disqus"];
            this.ViewBag.Account = this.Configuration["Account"];
            this.ViewBag.DefaultTemplate = this.Configuration["DefaultTemplate"];
            this.ViewBag.GitHub = this.Configuration["BlogRoll:GitHub"];
            this.ViewBag.Following = Convert.ToBoolean(this.Configuration["BlogRoll:Following"]);
            this.ViewBag.Follower = Convert.ToBoolean(this.Configuration["BlogRoll:Follower"]);

            // Building Tags
            this.ViewBag.Tags = this.DB.PostTags
                .OrderBy(x => x.Tag)
                .GroupBy(x => x.Tag)
                .Select(x => new TagViewModel
                {
                    Title = x.Key,
                    Count = x.Count(),
                })
                .ToList();

            // Building Calendar
            this.ViewBag.Calendars = this.DB.Posts
                .Where(x => !x.IsPage)
                .OrderByDescending(x => x.Time)
                .GroupBy(x => new { Year = x.Time.Year, Month = x.Time.Month })
                .Select(x => new CalendarViewModel
                {
                    Year = x.Key.Year,
                    Month = x.Key.Month,
                    Count = x.Count(),
                })
                .ToList();

            // Building Catalogs
            this.ViewBag.Catalogs = this.DB.Catalogs
                .Include(x => x.Posts)
                .OrderByDescending(x => x.PRI)
                .ToList()
                .Select(x => new CatalogViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Count = x.Posts.Count(),
                    PRI = x.PRI,
                    Url = x.Url,
                })
                .ToList();

            // Building Blog Rolls
            var rolls = this.DB.BlogRolls
                .Where(x => !string.IsNullOrEmpty(x.URL) && x.AvatarId.HasValue)
                .OrderByDescending(x => x.Type)
                .Select(x => new BlogRollViewModel
                {
                    AvatarId = x.AvatarId.Value,
                    Name = x.NickName,
                    URL = x.URL,
                })
                .ToList();

            rolls.Reverse();

            this.ViewBag.Rolls = rolls;
        }
    }
}
