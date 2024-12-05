using UnityEngine;
using UnityEngine.EventSystems;

public class UnoBtn : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Scene.IsMultiplayer) Connection.Instance.SendUno();
        else if (Scene.IsSingleplayer) GameMaster.Instance.PlayerManager.Uno();
    }
}