
# Websocket Message Server

The goal of this project is to help me understand how to work with websockets and implement real-time communication between clients and servers.

I really hoped that this was project was valuable for my backend journy and will also be as foundations when i will learn SignalR in Asp.net
## Run Locally

Clone the project

```bash
  git clone https://github.com/JikokuAice/WebSocket-Message-Broadcast.git
```

Go to the project directory

```bash
  cd WebSocket-Message-Broadcast
```


 1 open **Command Prompt** (cmd) in project location

2 Run Websocket server

```bash
 dotnet run broadcast-server start
```


1 open **Command Prompt** another cmd in same project

2  Run the client to connect to the WebSocket

```bash
dotnet run connect-client
```
Exit from Websocket message room

**As the client, type  exit in the console and press Enter to leave the WebSocket message room.**



## Documentation

[on progress](https://linktodocumentation)


## Features

- Uses Cocona library for creating console app in Asp.net core
- Create Websocket Server
- Server listen for message and broadcast to all clients
- Server eligently handle client abort or close state  
- Client can Send message 
- Client can receive message
- Client can exit websocket Server
- Server show who exited our Message Server in console

## Screenshots

**Starting Server**

![App Screenshot](https://lqevkoivmmqwzumdfgef.supabase.co/storage/v1/object/public/profile_pic/server%20started.png)

**Connecting first Client**

![App Screenshot](https://lqevkoivmmqwzumdfgef.supabase.co/storage/v1/object/public/profile_pic/client-connected.png)

**Connecting second Client**

![App Screenshot](https://lqevkoivmmqwzumdfgef.supabase.co/storage/v1/object/public/profile_pic/client-connected2.png)


**Message exchange**

![App Screenshot](https://lqevkoivmmqwzumdfgef.supabase.co/storage/v1/object/public/profile_pic/msg-exchange.png)

**client exits**

![App Screenshot](https://lqevkoivmmqwzumdfgef.supabase.co/storage/v1/object/public/profile_pic/connection-close.png)
