﻿using EventManagement.Application.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace EventManagement.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (BadRequestException ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (NotFoundException ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (TokenExpiredException ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (InternalServerErrorException ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error from the custom middleware.",
                Detailed = exception.Message 
            };

            _logger.LogError($"Exception Caught: {exception.Message}");

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
    }