using UnityEngine;

public class MyCardsHolder : MonoBehaviour
{
    private bool _canClick = false;

    public bool CanClick
    {
        get => _canClick;
        set => _canClick = value;
    }

    public void OnSelectCard(Card card)
    {
        if (CanClick)
        {
            GameMaster.Instance.PlayerManager.OnChooseCard(card);
        }
    }
}
