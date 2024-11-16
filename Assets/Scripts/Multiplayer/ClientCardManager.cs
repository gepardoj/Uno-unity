using UnityEngine;

public class ClientCardManager : AbstractCardManager
{
    public void CreateCardAndAddToPlayer(CardType type, SuitColor? color, SuitValue? value, OtherCards? other, CardState state)
    {
        var player = MultiplayerGame.Instance.PlayerManager.Player;
        CreateCardAndAddTo(player.Cards, player.CardsHolder, type, color, value, other, CardState.opened);
    }

    public void CreateCardAndAddToDiscardPile(CardType type, SuitColor? color, SuitValue? value, OtherCards? other, CardState state)
    {
        CreateCardAndAddTo(null, _discardPile, type, color, value, other, CardState.opened, new Vector3(0, 0, Random.Range(0, 360)));
    }
}