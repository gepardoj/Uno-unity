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
            GameMaster.Instance.PlayerManager.PullCards(CardManager.PULL_CARDS_N);
        }
    }
}
