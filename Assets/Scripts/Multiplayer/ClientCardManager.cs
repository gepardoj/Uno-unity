using UnityEngine;
using DG.Tweening;


public class ClientCardManager : AbstractCardManager
{
#nullable enable
    public Card? LastTouchedCard { get; set; }
#nullable disable

    public static float GIVE_CARD_DELAY = 0.1f;
    public static float FIRST_CARD_DELAY = 1f;

    // only for local player
    public void CreateCardAndAddToPlayer(PlayerData player, CardData cardData, byte timeToPlayS)
    {
        var cards = CreateCardAndAddTo(Deck.transform, player.Cards, player.CardsHolder, cardData.Type, cardData.Color, cardData.Value, cardData.Other, cardData.State, Vector3.zero, (card) =>
        {
            MultiplayerGame.Instance.PlayerManager.Player.CardsHolder.GetComponent<PlaceInRow>().Place();
        });
        if (timeToPlayS > 0) foreach (var _ in cards) _.Glow.Play(timeToPlayS);
    }

    public void CreateFirstCard(CardData cardValues)
    {

        CreateCardAndAddTo(Deck.transform, null, _startingCardOrigin.gameObject, cardValues.Type, cardValues.Color, cardValues.Value, cardValues.Other, CardState.opened, Vector3.zero, (card) =>
        {
            card.transform.DORotate(Vector3.up * 360, 2, RotateMode.FastBeyond360)
            .OnComplete(() =>
                card.GetComponent<MoveTowards>()
                .MoveTo(DiscardPile.transform, new Vector3(90, 0, Random.Range(0, 360)))
            );
        });
    }

    public void MoveCardFromPlayerToDiscardPile(PlayerData player, CardData cardData)
    {
        if (MultiplayerGame.Instance.PlayerManager.IsLocalPlayer(player))
        {
            Utils.RemoveAndGetElement(player.Cards, LastTouchedCard);
            MoveCardsToDiscardPile(new Card[] { LastTouchedCard }, (card) =>
            {
                MultiplayerGame.Instance.PlayerManager.Player.CardsHolder.GetComponent<PlaceInRow>().Place();
            });
            LastTouchedCard = null;
        }
        else // for remote player
        {
            var removedCard = Utils.RemoveAndGetFirstNElements(player.Cards, 1);  // doesnt matter which card to remove as they all the same (in client unity game)
            removedCard[0].Sprite = GetSpriteInCardsDef(cardData.Type, cardData.Color, cardData.Value, cardData.Other);
            MoveCardsToDiscardPile(removedCard, (card) =>
            {

            });
        }
    }

    // for remote player
    public void CreateFakeCard(PlayerData player)
    {
        CreateCardAndAddTo(Deck.transform, player.Cards, player.CardsHolder, CardType.suit, SuitColor.red, SuitValue._0, null, CardState.closed, Vector3.zero, (card) =>
        {
            player.CardsHolder.GetComponent<RotateAround>().PlaceObjectsAround();
        });
    }
}