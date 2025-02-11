using System.Text.Json;
using MySqlX.XDevAPI.Common;
using ServicesLibrary.Exceptions;

namespace WebApi.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            try
            {
                await _next(context);

            }
            catch (TournamentException ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    message = ex.Message,
                    statusCode = ex.StatusCode
                });


                await response.WriteAsync(result);

            }
        }
    }
}
