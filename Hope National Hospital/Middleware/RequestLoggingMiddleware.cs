namespace Hope_National_Hospital.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopewatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                await _next(context);
            }
            finally
            {
                stopewatch.Stop();
                var elapseMiddleseconds = stopewatch.ElapsedMilliseconds;


                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}.ms TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    elapseMiddleseconds,
                    context.TraceIdentifier
                );
                 
            }
        }
    }
}
