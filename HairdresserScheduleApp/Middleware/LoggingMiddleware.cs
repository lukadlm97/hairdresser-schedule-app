using System.Net;
using System.Text.Json;
using HairdresserScheduleApp.BusinessLogic.Models.Logging;
using HairdresserScheduleApp.BusinessLogic.Utilities;

namespace WebApi.Middleware
{
    public class LoggingMiddleware : IMiddleware
    {
        private readonly ILogger<LoggingMiddleware> logger;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IMemoryStreamPool memoryStreamPool;
        private readonly HairdresserScheduleApp.BusinessLogic.Services.IJwtService jWTService;
        private LogRequest logRequest;

        public LoggingMiddleware(IWebHostEnvironment hostingEnvironment,
                                 ILogger<LoggingMiddleware> logger,
                                 IMemoryStreamPool memoryStreamPool,
                                  LogRequest logRequest,
                                 HairdresserScheduleApp.BusinessLogic.Services.IJwtService jWTService)
        {
            this.jWTService = jWTService;
            this.logger = logger;
            this.hostingEnvironment = hostingEnvironment;
            this.memoryStreamPool = memoryStreamPool;
            this.logRequest = logRequest;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                GenerateGuid();
                MakeAutorizationLog(context);
                await LogRequest(context);
                await next.Invoke(context);
                LogRequestTrace();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occurred while executing request.");
                await HandleExceptionAsync(context, e, HttpStatusCode.InternalServerError);
            }
        }

        private void MakeAutorizationLog(HttpContext context)
        {
            var result = context.Request.Headers["Authorization"].ToString();

            if (result != null && result.Length > 7)
            {
                var token = result.ToString().Substring(7);
                (DateTime tokenExpireDate, string nameId, string role) = jWTService.GetAccountDetails(token);

                logRequest.AutorizationData = nameId + " - " + role + "     token expire:" + tokenExpireDate.ToString();
            }
        }

        private void LogRequestTrace()
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace($"RequestID:{logRequest.RequestId} " +
                                $"\nAutorization: {(string.IsNullOrWhiteSpace(logRequest.AutorizationData) ? "not autorized request" : logRequest.AutorizationData)}  " +
                                $"\nMessage:{logRequest.Message}");
            }
        }

        private void GenerateGuid()
        {
            logRequest.RequestId = Guid.NewGuid().ToString();
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode httpStatusCode)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)httpStatusCode;
            return context.Response.WriteAsync(
                (hostingEnvironment.IsDevelopment())
                    ? JsonSerializer.Serialize(new { error = exception.Message })
                    : string.Empty);
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();

            await using var requestStream = memoryStreamPool.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace($"Http Request Information:{Environment.NewLine}" +
                                $"Schema:{context.Request.Scheme} " +
                                $"Host: {context.Request.Host} " +
                                $"Path: {context.Request.Path} " +
                                $"QueryString: {context.Request.QueryString} " +
                                $"Request Body: {ReadStreamInChunks(requestStream)}");
            }
            context.Request.Body.Position = 0;
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;

            stream.Seek(0, SeekOrigin.Begin);

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);

            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;

            do
            {
                readChunkLength = reader.ReadBlock(readChunk,
                                                   0,
                                                   readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }
    }
}
