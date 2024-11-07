using UnityEngine;

public class GameMaster : MonoBehaviour
{
    private static GameMaster _instance;
    private CardManager _cardManager;
    private PlayerManager _playerManager;

    public static GameMaster Instance => _instance;
    public CardManager CardManager => _cardManager;
    public PlayerManager PlayerManager => _playerManager;

    void Start()
    {
        if (_instance == null) _instance = this;
        else { if (_instance != this) Destroy(gameObject); }

        _cardManager = GetComponentInChildren<CardManager>();
        _playerManager = GetComponentInChildren<PlayerManager>();

        _cardManager.ManualInit();

        _playerManager.ManualInit(new PlayerManager.GetCardsCB[]{
            holder => _cardManager.GiveCardsToPlayer(CardManager.START_CARDS_N, CardState.opened, holder),
            holder => _cardManager.GiveCardsToPlayer(CardManager.START_CARDS_N, CardState.closed, holder),
            holder => _cardManager.GiveCardsToPlayer(CardManager.START_CARDS_N, CardState.closed, holder),
            holder => _cardManager.GiveCardsToPlayer(CardManager.START_CARDS_N, CardState.closed, holder)
        });
        _playerManager.NextTurn();
    }

    public void PerformActionByPlayer(GameObject cardsHolder, Card card)
    {
        var player = _playerManager.GetPlayerByHolder(cardsHolder);
        if (_cardManager.TryMoveCardToDrop(player.Cards, card))
        {
            _playerManager.OnEndTurn();
            _playerManager.NextTurn();
        };
    }

    public void UseCardByAI(PlayerData player, Card card)
    {
        _cardManager.MoveCardToDrop(player.Cards, card);
    }
}
