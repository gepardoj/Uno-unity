using NativeWebSocket;

public class Connection : API
{
    private static Connection _instance;
    public static Connection Instance => _instance;

    void Start()
    {
        if (_instance == null)
        {
            print("Connection:: instance is initialized");
            _instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            print("Connection:: instance is already initialized, deleting another one...");
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        if (websocket == null)
        {
            print("init websocket");
            websocket = new WebSocket("ws://192.168.1.248:3000");
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
            websocket.OnMessage += OnMessage;
        }
        else
        {
            print("websocket already initialized");
        }
    }

    public void AttemptConnect()
    {
        if (websocket != null && websocket.State != WebSocketState.Open)
            websocket.Connect();
    }

    public void Close()
    {
        websocket?.Close();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    void OnApplicationQuit()
    {
        Close();
    }
}
