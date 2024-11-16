using UnityEngine;

public class MultiplayerGame : MonoBehaviour
{
    private static MultiplayerGame _instance;
    private ClientCardManager _cardManager;
    private ClientPlayerManager _playerManager;

    public static MultiplayerGame Instance => _instance;
    public ClientCardManager CardManager => _cardManager;
    public ClientPlayerManager PlayerManager => _playerManager;

    void Start()
    {
        if (_instance == null)
        {
            print("MultiplayerGame:: instance is initialized");
            _instance = this;
            Connection.Instance.AttemptConnect();
        }
        else if (_instance != this)
        {
            print("MultiplayerGame:: instance is already initialized, deleting another one...");
            Destroy(gameObject);
        }

        _cardManager = GetComponentInChildren<ClientCardManager>();
        _playerManager = GetComponentInChildren<ClientPlayerManager>();
    }
}
