using Microsoft.AspNetCore.Http;
using ProjectManagement.BLL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectManagement.WEB.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case NotExistingException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;

                    case AlreadyExistingException e:
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        break;

                    case System.UnauthorizedAccessException:
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        break;

                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(new { message = error?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
