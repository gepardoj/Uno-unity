using UnityEngine;

internal class MyCardsHolder : MonoBehaviour
{
    public void OnSelectCard(Card card)
    {
        var cardManager = FindObjectOfType<CardManager>();
        cardManager.TryMoveCardToDrop(cardManager.PlayerCards, card);
    }
}
