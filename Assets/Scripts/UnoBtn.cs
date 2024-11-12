using UnityEngine;
using UnityEngine.EventSystems;

public class UnoBtn : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        GameMaster.Instance.PlayerManager.Uno();
    }
}
