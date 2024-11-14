using NativeWebSocket;
using UnityEngine;

public class Connection : MonoBehaviour
{
    WebSocket websocket;

    async void Start()
    {
        websocket = new WebSocket("ws://localhost:3000");

        websocket.OnOpen += () =>
        {
            print($"connection open");
        };

        websocket.OnError += (e) =>
        {
            print($"error! {e}");
        };

        websocket.OnClose += (e) =>
        {
            print("connection closed");
        };

        websocket.OnMessage += (bytes) =>
        {
            print($"On message {bytes}");
        };

        InvokeRepeating(nameof(SendWebSocketMessage), 0.0f, 0.3f);

        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    async void SendWebSocketMessage()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.Send(new byte[] { 10, 20, 30 });
            await websocket.SendText("plain text message");
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}
