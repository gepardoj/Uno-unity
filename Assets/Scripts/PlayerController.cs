
/// <summary>
/// Controlled by player
/// </summary>
class PlayerController : IPlayerLogic
{
    private Card _choosedCard;
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

    public void OnChooseCard(Card card)
    {
        if (GameMaster.Instance.CardManager.IsCardMatchLastDrop(card))
            if (card.Type == CardType.suit)
            {
                GameMaster.Instance.PlayerManager.PlayCard(card);
            }
            else if (card.Type == CardType.other)
            {
                _choosedCard = card;
                WaitForChoosingColor();
            }
    }

    void WaitForChoosingColor()
    {
        GameMaster.Instance.CardManager.ColorPicker.SetActive(true);
        GameMaster.Instance.CardManager.CardsPull.CanClick = false;
        Player.CardsHolder.GetComponent<MyCardsHolder>().CanClick = false;
    }

    public void OnChooseColor(SuitColor color)
    {
        GameMaster.Instance.CardManager.ColorPicker.SetActive(false);
        GameMaster.Instance.PlayerManager.PlayCard(_choosedCard, color);
    }

    public void Uno() { }
}