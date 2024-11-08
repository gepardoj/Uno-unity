
/// <summary>
/// Controlled by player
/// </summary>
class PlayerController : IPlayerLogic
{
    private PlayerData _player;

    public PlayerData Player => _player;

    public PlayerController(PlayerData player)
    {
        _player = player;
    }

    public void GetTurn()
    {
        var cardsHolder = _player.CardsHolder.GetComponent<MyCardsHolder>();
        cardsHolder.CanClick = true;
        GameMaster.Instance.CardManager.CardsPull.CanClick = true;
        // MonoBehaviour.print($"get turn {cardsHolder.name} {cardsHolder.enabled}");
    }

    public void OnEndTurn()
    {
        _player.CardsHolder.GetComponent<MyCardsHolder>().CanClick = false;
        GameMaster.Instance.CardManager.CardsPull.CanClick = false;
    }

    public void UseCard(Card card)
    {
        if (GameMaster.Instance.CardManager.TryMoveCardToDrop(Player.Cards, card))
            if (card.Type == CardType.suit)
            {
                GameMaster.Instance.PlayerManager.CheckChangingDirection(card);
                GameMaster.Instance.PlayerManager.FinishTurn();
            }
            else if (card.Type == CardType.other)
            {
                WaitForColorToChoose();
            }
    }

    void WaitForColorToChoose()
    {
        GameMaster.Instance.CardManager.ColorPicker.SetActive(true);
        GameMaster.Instance.CardManager.CardsPull.CanClick = false;
        Player.CardsHolder.GetComponent<MyCardsHolder>().CanClick = false;
    }

    public void OnChooseColor(SuitColor color)
    {
        GameMaster.Instance.CardManager.ColorPicker.SetActive(false);
        GameMaster.Instance.CardManager.CurrentColor = color;
        GameMaster.Instance.PlayerManager.FinishTurn();
    }

    public void Uno() { }

    public void OnPullCards()
    {
        GameMaster.Instance.CardManager.TakeNewCards(Player, 1, CardState.opened);
        GameMaster.Instance.PlayerManager.FinishTurn();
    }
}