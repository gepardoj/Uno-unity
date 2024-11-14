using NativeWebSocket;
using UnityEngine;

public class Connection : MonoBehaviour
{
    private static Connection _instance;
    private API _api;
    public static Connection Instance => _instance;

    WebSocket websocket;

    void Start()
    {
        if (_instance == null) _instance = this;
        else { if (_instance != this) Destroy(gameObject); }

        _api = GetComponent<API>();
    }


    public async void Init()
    {
        if (websocket == null)
        {
            print("init websocket");
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

            websocket.OnMessage += _api.OnMessage;

            InvokeRepeating(nameof(SendWebSocketMessage), 0.0f, 0.3f);
        }
        else
        {
            print("websocket already initialized");
        }
        await websocket.Connect();
    }

    public void Close()
    {
        websocket.Close();
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
