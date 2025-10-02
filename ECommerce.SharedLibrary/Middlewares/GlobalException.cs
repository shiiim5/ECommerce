using System.Text.Json;
using ECommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.SharedLibrary.Middlewares
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string message = "Sorry, Internal Server Error Occured";
            int statusCode = (int)StatusCodes.Status500InternalServerError;
            string title = "Error";

            try
            {
                await next(context);

                //Too many requests
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    message = "Too many requests";
                    statusCode = StatusCodes.Status429TooManyRequests;
                    title = "Warning";

                    await ModifyHeader(context, title, statusCode, message);
                }

                //Not Authorized
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    message = "You are not authorized to access";
                    statusCode = StatusCodes.Status401Unauthorized;
                    title = "Alert";

                    await ModifyHeader(context, title, statusCode, message);
                }
                //Forbidden
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    message = "You are not allowed to access";
                    statusCode = StatusCodes.Status403Forbidden;
                    title = "Not Allowed";

                    await ModifyHeader(context, title, statusCode, message);
                }


            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    message = "Request time out! Try again";
                    title = "Time Out";
                    statusCode = StatusCodes.Status408RequestTimeout;

                }

                    await ModifyHeader(context, title, statusCode, message);


            }
        }

        private async static Task ModifyHeader(HttpContext context, string title, int statusCode, string message) {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = title

            }),CancellationToken.None);
            return;
        
            
        }
        
    }
}