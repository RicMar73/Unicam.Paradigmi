using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Unicam.Paradigmi.Application.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _logger.LogInformation("ErrorHandlingMiddleware registered");
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                // Log dell'errore
                _logger.LogError(ex, "Errore gestito dal middleware: {Message}", ex.Message);

                // Configura la risposta HTTP per gli errori
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                // Risposta JSON per il client
                var response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Si è verificato un errore durante la tua richiesta.",
                };
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));

            }
        }
    }

}
