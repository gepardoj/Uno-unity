using UnityEngine;

public class ClientCardManager : AbstractCardManager
{
#nullable enable
    public Card? LastTouchedCard { get; set; }
#nullable disable

    public void CreateCardAndAddToPlayer(PlayerData player, CardData cardData, byte timeToPlayS)
    {
        var cards = CreateCardAndAddTo(player.Cards, player.CardsHolder, cardData.Type, cardData.Color, cardData.Value, cardData.Other, cardData.State);
        if (timeToPlayS > 0) foreach (var _ in cards) _.Glow.Play(timeToPlayS);
    }

    public void CreateCardAndAddToDiscardPile(CardData cardValues)
    {
        CreateCardAndAddTo(null, _discardPile, cardValues.Type, cardValues.Color, cardValues.Value, cardValues.Other, CardState.opened, new Vector3(0, 0, Random.Range(0, 360)));
    }

    public void MoveCardFromPlayerToDiscardPile(PlayerData player, CardData cardData)
    {
        if (MultiplayerGame.Instance.PlayerManager.IsLocalPlayer(player))
        {
            Utils.RemoveAndGetElement(player.Cards, LastTouchedCard);
            MoveCardsTo(null, _discardPile, new Card[] { LastTouchedCard }, CardState.opened, new Vector3(0, 0, Random.Range(0, 360)));
            LastTouchedCard = null;
        }
        else
        {
            var removedCard = Utils.RemoveAndGetFirstNElements(player.Cards, 1);  // doesnt matter which card to remove as they all the same (in client unity game)
            Destroy(removedCard[0].gameObject);
            CreateCardAndAddToDiscardPile(cardData);
        }
    }

    // for other players
    public void CreateFakeCard(PlayerData player)
    {
        CreateCardAndAddTo(player.Cards, player.CardsHolder, CardType.suit, SuitColor.red, SuitValue._0, null, CardState.closed);
    }

    public void RemoveFakeCard(PlayerData player)
    {
        Destroy(player.Cards[0].gameObject);
        player.Cards.RemoveAt(0);
    }
}