public class ClientCardManager : AbstractCardManager
{
    public void CreateCardAndAddToPlayer(CardType type, SuitColor? color, SuitValue? value, OtherCards? other, CardState state)
    {
        var card = Instantiate(_cardPrefab);
        var player = MultiplayerGame.Instance.PlayerManager.Player;
        card.transform.SetParent(player.CardsHolder.transform, false);
        card.Init(type, color, value, other, CardState.opened, GetSpriteInCardsDef(type, color, value, other), _closedSprite);
        player.Cards.Add(card);
    }
}