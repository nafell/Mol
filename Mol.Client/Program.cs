using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using MagicOnion.Client;
using Mol.Server;

//Create a custom HttpClientHandler
var handler = new HttpClientHandler();
var certificate = new X509Certificate2("/home/nafell/source/learncodes/magicOnionLearn/Mol.Server/server.crt");
handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
{
    if (cert.GetCertHashString() == certificate.GetCertHashString())
        return true;
    return false;
};


// Create HttpClient with the custom handler
var httpClient = new HttpClient(handler);

// Create GrpcChannelOptions with the HttpClient
var channelOptions = new GrpcChannelOptions
{
    HttpClient = httpClient
};

// Create the channel with the custom options
// var channel = GrpcChannel.ForAddress("http://localhost:5000");
var channel = GrpcChannel.ForAddress("https://localhost:5001", channelOptions);

var client = MagicOnionClient.Create<IMyFirstService>(channel);
var result = await client.AddAsync(1, 2);
Console.WriteLine($"Result: {result}");