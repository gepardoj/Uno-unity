using System.Linq;
using TMPro;
using UnityEngine;

public class MultiplayerGame : MonoBehaviour
{
    private static MultiplayerGame _instance;
    private ClientCardManager _cardManager;
    private ClientPlayerManager _playerManager;

    public static MultiplayerGame Instance => _instance;
    public ClientCardManager CardManager => _cardManager;
    public ClientPlayerManager PlayerManager => _playerManager;

    public TextMeshProUGUI debugText;

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

    public void OnSelectCard(Card card)
    {
        CardManager.LastTouchedCard = card;
        Connection.Instance.SendTryChooseCard(card);
    }

    public void AddPlayer(string id, int cardsNumber)
    {
        var player = PlayerManager.AddPlayer(id);
        foreach (var _ in Enumerable.Range(1, cardsNumber))
            CardManager.CreateCardAndAddToPlayer(player, CardType.suit, SuitColor.red, SuitValue._0, null, CardState.closed); // doesnt really matter what cards, it's just a facade
    }
}
