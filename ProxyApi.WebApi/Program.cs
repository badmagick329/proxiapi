using System.Diagnostics;

class Program
{
    public static void Main(string[] args)
    {
        Run(args);
    }

    static void Run(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddHttpClient();
        var app = builder.Build();
        var allowedIps = builder.Configuration["AllowedIps"]?.Split(",");
        var listenOn = builder.Configuration["ListenOn"];
        Debug.Assert(allowedIps is not null);
        Debug.Assert(!string.IsNullOrEmpty(listenOn));

        app.Use(
            async (context, next) =>
            {
                var remoteIp = context.Connection.RemoteIpAddress?.ToString();
                if (!allowedIps.Contains(remoteIp))
                {
                    Console.WriteLine($"Forbidden request from {remoteIp}");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden");
                    return;
                }
                await next();
            }
        );

        app.MapControllers();
        app.Run();
    }
}
