using UnityEngine;
using UnityEngine.EventSystems;

public class CardsPull : MonoBehaviour, IPointerClickHandler
{
    private bool _canClick = false;

    public bool CanClick
    {
        get => _canClick;
        set => _canClick = value;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CanClick)
        {
            GameMaster.Instance.CardManager.TakeNewCards(GameMaster.Instance.PlayerManager.RealPlayer, 1, CardState.opened);
            GameMaster.Instance.PlayerManager.FinishTurn();
        }
    }
}
