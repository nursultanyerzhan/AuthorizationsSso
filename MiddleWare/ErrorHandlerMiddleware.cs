using System.Net;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

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
            string pathString = context.Request.Path.Value.ToString();
            if (pathString.ToLower().Contains("getiin"))
                if (context.User.Identities.Any(r => !r.IsAuthenticated))
                    throw new SecurityTokenExpiredException();

            await _next(context);
        }
        catch (Exception ex)
        {
            // Проверяем, если исключение связано с истекшим токеном, то возвращаем 401 и сообщение
            if (ex is SecurityTokenExpiredException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { message = "Истек срок действия токена." }));
                return;
            }

            // В остальных случаях возвращаем другой код состояния и сообщение об ошибке
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonConvert.SerializeObject(new { message = "Произошла ошибка на сервере." }));
        }
    }
}
