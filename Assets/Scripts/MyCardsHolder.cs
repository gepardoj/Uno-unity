using UnityEngine;

public class MyCardsHolder : MonoBehaviour
{
    private bool _canClick = false;

    public bool CanClick
    {
        get => _canClick;
        set => _canClick = value;
    }

    public void OnSelectCard(Card card)
    {
        if (_canClick)
        {
            var game = GameMaster.Instance;
            var player = game.PlayerManager.GetPlayerByHolder(gameObject);
            if (game.CardManager.TryMoveCardToDrop(player.Cards, card))
            {
                game.PlayerManager.FinishTurn();
            };
        }
    }
}
