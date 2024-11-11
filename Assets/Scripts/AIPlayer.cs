using System.Collections;
using System.Linq;
using UnityEngine;

class AIPlayer : IPlayerLogic
{
    private PlayerData _player;

    const float MIN_DELAY_MS = 1.5f;
    const float MAX_DELAY_MS = 2.5f;

    public PlayerData Player => _player;

    public AIPlayer(PlayerData player)
    {
        _player = player;
    }

    public void OnGetTurn(bool? shouldDeclareColor)
    {
        _player.StartCoroutine(PerfomAction(Random.Range(MIN_DELAY_MS, MAX_DELAY_MS), shouldDeclareColor));
    }
    public void OnEndTurn() { }
    public void OnChooseCard(Card card) { }
    public void Uno() { }

    IEnumerator PerfomAction(float waitTime, bool? shouldDeclareColor)
    {
        yield return new WaitForSeconds(waitTime);
        var color = ChooseColor();
        if (shouldDeclareColor ?? false) GameMaster.Instance.CardManager.CurrentColor = color;
        var card = ChooseCard();
        if (card)
        {
            GameMaster.Instance.PlayerManager.PlayCard(card, color);
        }
        else
        {
            GameMaster.Instance.PlayerManager.PullCards(CardManager.PULL_CARDS_N);
        }
    }

    Card ChooseCard()
    {
        var cardManager = GameMaster.Instance.CardManager;
        foreach (var card in Player.Cards)
        {
            if (cardManager.IsCardMatchLastDrop(card)) return card;
        }
        return null;
    }

    SuitColor ChooseColor()
    {
        var colors = Player.Cards.Where(card => card.Color != null);
        colors = colors.DistinctBy(card => card.Color);
        // no suitcards left, choose random color
        if (colors.Count() == 0) return Utils.RandomEnum<SuitColor>();
        // choose color from the player's cards
        return (SuitColor)colors.ToArray()[Random.Range(0, colors.Count())].Color;
    }

    public void OnChosenColor(SuitColor color) { }
}