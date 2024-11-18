using UnityEngine;

public class ClientCardManager : AbstractCardManager
{
    public Card? LastTouchedCard { get; set; }

    public void CreateCardAndAddToPlayer(PlayerData player, CardType type, SuitColor? color, SuitValue? value, OtherCards? other, CardState state)
    {
        CreateCardAndAddTo(player.Cards, player.CardsHolder, type, color, value, other, state);
    }

    public void CreateCardAndAddToDiscardPile(CardType type, SuitColor? color, SuitValue? value, OtherCards? other, CardState state)
    {
        CreateCardAndAddTo(null, _discardPile, type, color, value, other, CardState.opened, new Vector3(0, 0, Random.Range(0, 360)));
    }

    public void MoveCardFromPlayerToDiscardPile(bool res)
    {
        if (res)
        {
            var player = MultiplayerGame.Instance.PlayerManager.Player;
            Utils.RemoveAndGetElement(player.Cards, LastTouchedCard);
            MoveCardsTo(null, _discardPile, new Card[] { LastTouchedCard }, CardState.opened, new Vector3(0, 0, Random.Range(0, 360)));
        }
        LastTouchedCard = null;
    }
}