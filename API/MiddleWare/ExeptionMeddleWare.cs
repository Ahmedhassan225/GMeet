using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.MiddleWare
{
    public class ExeptionMeddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExeptionMeddleWare> _logger;
        private readonly IHostEnvironment _env;
        public ExeptionMeddleWare(RequestDelegate next, ILogger<ExeptionMeddleWare> logger, IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context){
            try{
                await _next(context);
            }
            catch(Exception ex){

                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                var responce = _env.IsDevelopment()
                    ? new ApiExeptions(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiExeptions(context.Response.StatusCode, ex.Message, "Internal Server Error");
                

                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
                var json = JsonSerializer.Serialize(responce, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}