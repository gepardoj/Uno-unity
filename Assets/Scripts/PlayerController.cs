
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

    public void OnGetTurn(bool? shouldDeclareColor, bool? prevPlayerSaidUno)
    {
        if (shouldDeclareColor ?? false)
        {
            WaitForChoosingColor();
            return;
        }
        var cardsHolder = _player.CardsHolder.GetComponent<MyCardsHolder>();
        cardsHolder.CanClick = true;
        GameMaster.Instance.CardManager.Deck.CanClick = true;
        // MonoBehaviour.print($"get turn {cardsHolder.name} {cardsHolder.enabled}");
    }

    public void OnEndTurn()
    {
        _player.CardsHolder.GetComponent<MyCardsHolder>().CanClick = false;
        GameMaster.Instance.CardManager.Deck.CanClick = false;
    }

    public void OnChosenCard(Card card)
    {
        if (GameMaster.Instance.CardManager.IsCardMatchLastInDiscardPile(card))
            if (card.Type == CardType.suit)
            {
                CheckUnoCondition();
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
        GameMaster.Instance.CardManager.Deck.CanClick = false;
        Player.CardsHolder.GetComponent<MyCardsHolder>().CanClick = false;
    }

    public void OnChosenColor(SuitColor color)
    {
        GameMaster.Instance.CardManager.ColorPicker.SetActive(false);
        GameMaster.Instance.CardManager.Deck.CanClick = true;
        Player.CardsHolder.GetComponent<MyCardsHolder>().CanClick = true;
        if (_choosedCard)
        {
            CheckUnoCondition();
            GameMaster.Instance.PlayerManager.PlayCard(_choosedCard, color);
        }
        else
        {
            GameMaster.Instance.CardManager.CurrentColor = color;
        }
    }

    void CheckUnoCondition()
    {
        // Uno said when penultimate card playing and AI has 50% chance remember to say Uno =) 
        if (Player.Cards.Count == 2)
        {
            Player.SaidUno = false;
        }
        else Player.SaidUno = null;
    }
}