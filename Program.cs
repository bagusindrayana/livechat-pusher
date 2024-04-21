using Microsoft.OpenApi.Models;
using PusherServer;
using LiveChat.Models;
using System.Diagnostics;
using LiveChat;

var builder = WebApplication.CreateBuilder(args);

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
DotEnv.Load(dotenv);

const string CorsPolicyName = "_myCorsPolicy";
builder.Services.AddCors(options 
  => options.AddPolicy(name: CorsPolicyName, builder => builder
  .AllowAnyOrigin()
  .AllowAnyHeader()
  .AllowAnyMethod()));

var app = builder.Build();
app.UseCors(CorsPolicyName);
app.MapGet("/", () => "Hello World!");

async Task<String> sendMessage(SendMessage data) {
    var options = new PusherOptions
    {
      Cluster = "ap1",
      Encrypted = true
    };

    var pusher = new Pusher(
      Environment.GetEnvironmentVariable("APP_ID"),
      Environment.GetEnvironmentVariable("APP_KEY"),
      Environment.GetEnvironmentVariable("APP_SECRET"),
      options);

    var result = await pusher.TriggerAsync(
      data.channel,
      "message",
      new { message = data.message } );
    return "okoc";
}





app.MapPost("/send-message",async (HttpRequest request) =>{
    var data = await request.ReadFromJsonAsync<SendMessage>();
    if (data != null){
        Debug.Print(data.channel);
        var result = await sendMessage(data);
        return result;    
    } else {
        return "error";
    }
});
var PORT = Environment.GetEnvironmentVariable("PORT") ?? "5179";
app.Run("http://*:"+PORT);
