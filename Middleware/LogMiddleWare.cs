using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace WebApi.Middleware
{
    public class LogMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        ILogger<LogMiddleWare> _logger;

        public LogMiddleWare(RequestDelegate next, IOptions<AppSettings> appSettings, ILogger<LogMiddleWare> logger)
        {
            _next = next;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, DataContext dataContext)
        {
            if (_appSettings.LogRequests)
            {
                context.Request.EnableBuffering();
                var bodyStream = new StreamReader(context.Request.Body);
                var bodyText = await bodyStream.ReadToEndAsync();
                context.Request.Body.Position = 0;

                _logger.LogInformation("############ REQUST LOG ########################");
                _logger.LogInformation(context.Request.GetDisplayUrl());
                _logger.LogInformation(context.Request.Method);
                _logger.LogInformation(string.Join(Environment.NewLine, context.Request.Headers.Select(x => $"{x.Key}: {x.Value}")));
                _logger.LogInformation(bodyText);
                _logger.LogInformation("################################################");
            }

            await _next(context);
        }
        
    }
}