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

    public void GetTurn()
    {
        _player.StartCoroutine(PerfomAction(Random.Range(MIN_DELAY_MS, MAX_DELAY_MS)));
    }
    public void OnEndTurn() { }
    public void UseCard(Card card) { }
    public void Uno() { }

    IEnumerator PerfomAction(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //Debug.Log($"Waited for {waitTime}");
        var card = ChooseCard();
        if (card)
        {
            GameMaster.Instance.CardManager.MoveCardToDrop(Player.Cards, card);
            if (card.Type == CardType.other)
            {
                GameMaster.Instance.CardManager.CurrentColor = ChooseColor();
            }
        }
        else
        {
            // Debug.Log($"{Player.name} Taking new card");
            OnPullCards();
        }
        GameMaster.Instance.PlayerManager.FinishTurn();
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
        colors = Player.Cards.DistinctBy(card => card.Color);
        if (colors.Count() == 0) return Utils.RandomEnum<SuitColor>(); // no suitcards left, choose random color
        return (SuitColor)colors.ToArray()[Random.Range(0, colors.Count())].Color;
    }

    public void OnPullCards()
    {
        GameMaster.Instance.CardManager.TakeNewCards(Player, 1, CardState.closed);
    }
}