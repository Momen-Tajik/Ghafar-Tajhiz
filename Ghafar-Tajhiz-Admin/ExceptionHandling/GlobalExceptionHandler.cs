using Microsoft.AspNetCore.Diagnostics;

namespace Ghafar_Tajhiz_Admin.ExceptionHandling
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var traceId =
                httpContext.TraceIdentifier;

            _logger.LogError(
                exception,
                "Unhandled exception. Request: {Method} {Path}. TraceId: {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                traceId);

            if (!httpContext.Response.HasStarted)
            {
                httpContext.Response.Redirect(
                    $"/Home/Error?traceId={Uri.EscapeDataString(traceId)}");
            }

            return ValueTask.FromResult(true);
        }
    }
}