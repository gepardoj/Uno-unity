using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting;

public class ColoredItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField, RequiredMember] private SuitColor _color;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameMaster.Instance.PlayerManager.OnChooseColor(_color);
    }
}
