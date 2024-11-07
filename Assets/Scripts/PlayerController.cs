
using UnityEngine;

/// <summary>
/// Controlled by player
/// </summary>
class PlayerController : IPlayer
{
    public bool GetTurn()
    {
        var cardsHolder = Object.FindObjectOfType<MyCardsHolder>();
        cardsHolder.CanClick = true;
        MonoBehaviour.print($"get turn {cardsHolder.name} {cardsHolder.enabled}");
        return false;
    }
    public void OnEndTurn()
    {
        var cardsHolder = Object.FindObjectOfType<MyCardsHolder>();
        cardsHolder.CanClick = false;
    }
    public void UseCard() { }
    public void Uno() { }
}