using System.Collections;
using System.Linq;
using UnityEngine;

class AIPlayer : IPlayerLogic
{
    private PlayerData _player;

    const float MIN_DELAY = 1.5f;
    const float MAX_DELAY = 2.5f;
    const float UNO_DELAY = 0.5f;

    public PlayerData Player => _player;

    public AIPlayer(PlayerData player)
    {
        _player = player;
    }

    public void OnGetTurn(bool? shouldDeclareColor, bool? prevPlayerSaidUno)
    {
        Player.StartCoroutine(PerfomAction(Random.Range(MIN_DELAY, MAX_DELAY), shouldDeclareColor, prevPlayerSaidUno));
    }
    public void OnEndTurn() { }
    public void OnChosenCard(Card card) { }
    public void Uno() { }

    IEnumerator PerfomAction(float waitTime, bool? shouldDeclareColor, bool? prevPlayerSaidUno)
    {
        yield return new WaitForSeconds(waitTime);
        CheckPrevPlayerUno(prevPlayerSaidUno);
        var color = ChooseColor();
        if (shouldDeclareColor ?? false) GameMaster.Instance.CardManager.CurrentColor = color;
        var card = ChooseCard();
        CheckUnoCondition();
        if (card)
        {
            GameMaster.Instance.PlayerManager.PlayCard(card, color);
        }
        else
        {
            GameMaster.Instance.PlayerManager.DrawCards(Const.PULL_CARDS_N);
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

    void CheckPrevPlayerUno(bool? prevPlayerSaidUno)
    {
        if (prevPlayerSaidUno != null && prevPlayerSaidUno == false)
        {
            if (Random.Range(0, 2) == 0) GameMaster.Instance.PlayerManager.UnoPenalty();
        }
    }

    void CheckUnoCondition()
    {
        // Uno said when penultimate card playing and AI has 50% chance remember to say Uno =) 
        if (Player.Cards.Count == 2)
        {
            if (Random.Range(0, 2) == 0)
            {
                Player.StartCoroutine(WaitSayUno());
            }
            else
            {
                Debug.Log($"{Player.name} forgot to say uno");
                Player.SaidUno = false;
            }
        }
        else Player.SaidUno = null;
    }

    IEnumerator WaitSayUno()
    {
        yield return new WaitForSeconds(UNO_DELAY);
        Debug.Log($"{Player.name} said uno");
        Player.SaidUno = true;
        Player.StatusText.AddPlay(Const.UNO_TEXT);
    }
}