// See https://aka.ms/new-console-template for more information
using Cocona;
using System.Net;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks.Dataflow;

var builder = CoconaApp.CreateBuilder();

builder.Services.AddHttpClient();
var app = builder.Build();
List<WebSocket> clients = new List<WebSocket>();

app.AddCommand("Connect-client", async () => {
    try
    {

        var newClient = new ClientWebSocket();

        string clientName = "";
        while (true)
        {
            Console.Write("Please insert your name : ");
            clientName = Console.ReadLine();
            if (!string.IsNullOrEmpty(clientName))
            {
                break;
            }

        }

        await newClient.ConnectAsync(new Uri($"ws://localhost:5000/ws/?name={clientName}"), CancellationToken.None);

        Console.WriteLine("Client connected");

        var TaskSend = Task.Run(async () => await SendMessage(newClient,clientName));

        var TaskReceived = Task.Run(async () => await receivedMessage(newClient,clientName));

        Task.WaitAny(TaskSend, TaskReceived);

        if (newClient.State != WebSocketState.Closed)
        {
            await newClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }

        Task.WaitAll(TaskSend, TaskReceived);

    }
    catch (Exception e)
    {

        Console.WriteLine(e.Message.ToString());
    };

});

app.AddSubCommand("broadcast-server", x => {

    x.AddCommand("start", async () => {

        var httpListener = new HttpListener();
        httpListener.Prefixes.Add("http://localhost:5000/ws/");
        httpListener.Start();
        Console.WriteLine("WebSocket server started at ws://localhost:5000/ws/");

        while (true)
        {

            var httpContext = await httpListener.GetContextAsync();

            if (httpContext.Request.IsWebSocketRequest)
            {

                var clientName = httpContext.Request.QueryString["name"];

                if (clientName == null)
                {
                    clientName = "random" + DateTime.UtcNow.Millisecond;
                }

                var ws = await httpContext.AcceptWebSocketAsync(null);
                clients.Add(ws.WebSocket);
                Console.WriteLine($"Welcome client : {clientName}");
                await BoardCast($"Welcome client : {clientName}");
                Console.WriteLine($"Current client Count : {clients.Count}");

                

                Task.Run(() => GetMessage(ws.WebSocket, async (msg, buffer) => {

                    if (msg.MessageType == WebSocketMessageType.Text)
                    {

                        var encodedMsg = Encoding.UTF8.GetString(buffer, 0, msg.Count);

                        await BoardCast($"{clientName}:" + encodedMsg);


                    }
                    else if (msg.MessageType == WebSocketMessageType.Close || ws.WebSocket.State == WebSocketState.Aborted)
                    {
                        clients.Remove(ws.WebSocket);
                    
                        await BoardCast($"Connection closed by Client :  {clientName}");
                        await ws.WebSocket.CloseAsync(msg.CloseStatus.Value, "", CancellationToken.None);

                    }

                }));

            }
            else
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                httpContext.Response.StatusDescription = "We only accept Websocket Request";
            }
        }

    });

});

async Task BoardCast(string message)
{
    var msgByte = Encoding.UTF8.GetBytes(message);

    foreach (var socket in clients)
    {
        if (socket.State == WebSocketState.Open)
        {
            await socket.SendAsync(msgByte, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}

// Action is a delegate that does not return a value.
// The generic types inside Action represent the parameters it accepts.
async Task GetMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
{
    var buffer = new byte[1024];

    while (socket.State == WebSocketState.Open)
    {
        var receivedMsg = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        handleMessage(receivedMsg, buffer);
    }

}

async Task receivedMessage(WebSocket socket,string clientName)
{
   

    byte[] buffer = new byte[1024];

    while (true)
    {

        var msg = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        if (msg.MessageType == WebSocketMessageType.Close)
        {
            break;
        }
        string decodedMessage = Encoding.UTF8.GetString(buffer,0,msg.Count);


        if (decodedMessage.Split(':')[0].ToString() != clientName)
        {
            Console.WriteLine(decodedMessage);
        }
      
       
    }

}

async Task SendMessage(WebSocket socket,string clientName)
{

    while (true)
    {

        var msg = Console.ReadLine();

        if (msg == "exit")
        {
            break;
        }

        var msgConverted = Encoding.UTF8.GetBytes(msg);

        await socket.SendAsync(new ArraySegment<byte>(msgConverted), WebSocketMessageType.Text, true, CancellationToken.None); ;

    }

}

app.Run();