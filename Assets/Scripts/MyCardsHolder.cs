using UnityEngine;

internal class MyCardsHolder : MonoBehaviour
{
    public void OnSelectCard(Card card)
    {
        var cardManager = FindObjectOfType<CardManager>();
        cardManager.MoveCardToDrop(card);
        card.transform.SetParent(cardManager.dropCardsHolder.transform, false);
        card.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, Random.Range(0, 361)));
    }
}
