using UnityEngine;
using UnityEngine.EventSystems;

public class Deck : MonoBehaviour, IPointerClickHandler
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
            GameMaster.Instance.PlayerManager.DrawCards(Const.DRAW_CARDS_N);
        }
    }
}
