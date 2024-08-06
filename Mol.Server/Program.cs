using System.Net;
using System.Security.Cryptography.X509Certificates;
using MagicOnion;
using MagicOnion.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Parse("0.0.0.0"), 5000,
        listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });

    options.Listen(IPAddress.Parse("0.0.0.0"), 5001,
        listenOptions =>
        {
            if (args.Any(arg => arg == "--load-cert=true"))
            {
                Console.WriteLine("load certificate");
                listenOptions.UseHttps(new X509Certificate2("/home/nafell/source/learncodes/magicOnionLearn/Mol.Server/server.pfx", "password"));
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            }
        });
    
    options.Listen(IPAddress.Parse("0.0.0.0"), 5002,
        listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
});

builder.Services.AddGrpc();
builder.Services.AddMagicOnion();

var app = builder.Build();

app.MapGet("/", () => "Hello, World!");

app.MapMagicOnionService();

app.Run();