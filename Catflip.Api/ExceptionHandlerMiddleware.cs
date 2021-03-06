﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Catflip.Api
{

    //public record ProblemDetails(string Title, string Type, int Status, string TraceId);


    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        JsonSerializerSettings settings;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
            settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                var logger = context.RequestServices.GetService<ILogger<ExceptionHandlerMiddleware>>();
                if (logger != null)
                {
                    logger.LogError(ex, ex.Message);
                }
                var requestId = Activity.Current?.Id ?? context.TraceIdentifier;
                var result = JsonConvert.SerializeObject(new ProblemDetails {
                    Title = ex.Message, 
                    Type = ex.GetType().Name, 
                    Status = (int)HttpStatusCode.BadRequest, 
                    Detail = requestId 
                }, settings);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(result);
            }
        }
    }
}
