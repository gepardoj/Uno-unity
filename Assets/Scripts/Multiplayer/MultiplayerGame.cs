using TMPro;
using UnityEngine;

public class MultiplayerGame : MonoBehaviour
{
    private static MultiplayerGame _instance;
    private ClientCardManager _cardManager;
    private ClientPlayerManager _playerManager;
    [SerializeField] private SimpleSlider _timeSlider;

    public static MultiplayerGame Instance => _instance;
    public ClientCardManager CardManager => _cardManager;
    public ClientPlayerManager PlayerManager => _playerManager;
    public SimpleSlider TimeSlider => _timeSlider;

    public TextMeshProUGUI debugText;

    void Start()
    {
        if (_instance == null)
        {
            print("MultiplayerGame:: instance is initialized");
            _instance = this;
            Connection.Instance.AttemptConnect();
        }
        else if (_instance != this)
        {
            print("MultiplayerGame:: instance is already initialized, deleting another one...");
            Destroy(gameObject);
        }

        _cardManager = GetComponentInChildren<ClientCardManager>();
        _playerManager = GetComponentInChildren<ClientPlayerManager>();
    }

    public void OnSelectCard(Card card)
    {
        if (!PlayerManager.IsLocalPlayersTurn()) return;
        CardManager.LastTouchedCard = card;
        if (card.IsWild || card.IsWildDraw)
        {
            var colorPicker = CardManager.ColorPicker;
            colorPicker.SetActive(true);
            void action(SuitColor color)
            {
                Connection.Instance.SendPlayCard(card, color);
                colorPicker.SetActive(false);
                ColorPicker.OnChosenColor -= action;
            }
            ColorPicker.OnChosenColor += action;
        }
        else
        {
            CardManager.ColorPicker.SetActive(false);
            Connection.Instance.SendPlayCard(card, null);
        }
    }
}
