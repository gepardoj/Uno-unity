using System.Collections;
using UnityEngine;

class AIPlayer : IPlayerLogic
{
    private PlayerData _player;
    const float MIN_DELAY_MS = 0.5f;
    const float MAX_DELAY_MS = 2f;

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
        if (card) GameMaster.Instance.UseCardByAI(Player, card);
        else
        {
            Debug.Log($"Card not found. {Player.name} Taking new card");
            GameMaster.Instance.CardManager.TakeNewCards(Player.Cards, Player.CardsHolder, 1, CardState.closed);
        }
        GameMaster.Instance.PlayerManager.NextTurn();
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
}