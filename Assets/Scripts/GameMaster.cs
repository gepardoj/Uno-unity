using UnityEngine;

public class GameMaster : MonoBehaviour
{
    private CardManager _cardManager;
    private PlayerManager _playerManager;

    void Start()
    {
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
}
