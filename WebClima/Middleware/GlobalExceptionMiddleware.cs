using System.Text.Json;
using Application.DTOs;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http.Features;

namespace WebClima.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Exceção não tratada capturada. Tipo: {ExceptionType}, Mensagem: {Message}, Path: {Path}, Method: {Method}, TraceId: {TraceId}", 
                    ex.GetType().Name, 
                    ex.Message, 
                    context.Request.Path, 
                    context.Request.Method,
                    context.TraceIdentifier);
                
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            ErrorResponse errorResponse;
            int statusCode;

            switch (exception)
            {
                case NegocioException negocioEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    errorResponse = new ErrorResponse(
                        statusCode,
                        negocioEx.Message,
                        null
                    );
                    _logger.LogWarning(
                        "Erro de negócio: {Message}, Path: {Path}, TraceId: {TraceId}", 
                        negocioEx.Message, 
                        context.Request.Path, 
                        context.TraceIdentifier);
                    break;

                case HttpRequestException httpEx:
                    statusCode = StatusCodes.Status502BadGateway;
                    errorResponse = new ErrorResponse(
                        statusCode,
                        "Erro ao comunicar com serviço externo. Tente novamente mais tarde.",
                        new List<string> { httpEx.Message }
                    );
                    _logger.LogError(httpEx, 
                        "Erro de comunicação com serviço externo: {Message}, Path: {Path}, TraceId: {TraceId}", 
                        httpEx.Message, 
                        context.Request.Path, 
                        context.TraceIdentifier);
                    break;

                case FormatException formatEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    errorResponse = new ErrorResponse(
                        statusCode,
                        "Formato de parâmetro inválido. Verifique os valores enviados.",
                        new List<string> { formatEx.Message }
                    );
                    _logger.LogWarning(
                        "Formato de parâmetro inválido: {Message}, Path: {Path}, TraceId: {TraceId}", 
                        formatEx.Message, 
                        context.Request.Path, 
                        context.TraceIdentifier);
                    break;

                case OverflowException overflowEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    errorResponse = new ErrorResponse(
                        statusCode,
                        "Valor fora do intervalo permitido. Verifique os parâmetros enviados.",
                        new List<string> { overflowEx.Message }
                    );
                    _logger.LogWarning(
                        "Valor fora do intervalo: {Message}, Path: {Path}, TraceId: {TraceId}", 
                        overflowEx.Message, 
                        context.Request.Path, 
                        context.TraceIdentifier);
                    break;

                case BadHttpRequestException badHttpEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    errorResponse = new ErrorResponse(
                        statusCode,
                        "Erro na requisição. Verifique os parâmetros enviados.",
                        new List<string> { badHttpEx.Message }
                    );
                    _logger.LogWarning(
                        "Requisição HTTP inválida: {Message}, Path: {Path}, TraceId: {TraceId}", 
                        badHttpEx.Message, 
                        context.Request.Path, 
                        context.TraceIdentifier);
                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    errorResponse = new ErrorResponse(
                        statusCode,
                        "Erro interno do servidor. Contate o administrador.",
                        new List<string> { exception.Message }
                    );
                    _logger.LogCritical(exception, 
                        "Erro interno não tratado: {Message}, StackTrace: {StackTrace}, Path: {Path}, TraceId: {TraceId}", 
                        exception.Message, 
                        exception.StackTrace, 
                        context.Request.Path, 
                        context.TraceIdentifier);
                    break;
            }

            context.Response.StatusCode = statusCode;

            JsonSerializerOptions jsonOptions = new()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string json = JsonSerializer.Serialize(errorResponse, jsonOptions);
            return context.Response.WriteAsync(json);
        }
    }
}