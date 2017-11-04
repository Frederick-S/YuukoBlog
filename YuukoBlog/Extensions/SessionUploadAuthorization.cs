using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.AspNetCore.Extensions.BlobStorage;
using YuukoBlog.Extensions;

namespace YuukoBlog.Extensions
{
    public class SessionUploadAuthorization : IBlobUploadAuthorizationProvider
    {
        private IServiceProvider services;

        public SessionUploadAuthorization(IServiceProvider provider)
        {
            this.services = provider;
        }

        public bool IsAbleToUpload()
        {
            var val = this.services.GetRequiredService<IHttpContextAccessor>().HttpContext.Session.GetString("Admin");

            if (val == "true")
            {
                return true;
            }

            return false;
        }
    }
}