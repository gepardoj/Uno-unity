using UnityEngine;

public class Deck : MonoBehaviour
{
    private bool _canClick = false;
    [SerializeField] private ParticleSystem _particleSystem;

    public bool CanClick
    {
        get => _canClick;
        set => _canClick = value;
    }

    void Start()
    {
        Highlight(false);
    }

    public void Highlight(bool value)
    {
        _particleSystem.gameObject.SetActive(value);
    }

    void OnMouseDown()
    {
        if (Scene.IsMultiplayer)
        {
            Connection.Instance.SendDrawCardsFromDeck();
            return;
        }
        if (CanClick)
        {
            GameMaster.Instance.PlayerManager.DrawCards(Const.DRAW_CARDS_N);
        }
    }
}
