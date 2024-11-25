using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Scripting;

public enum PlayerType { Player, AI_Player, RemotePlayer }

public class PlayerData : MonoBehaviour
{
    [SerializeField, ReadOnly] private List<Card> _cards = new();
    [SerializeField, RequiredMember] private GameObject _cardsHolder;
    [SerializeField, RequiredMember] private AvatarPlayer _avatar;
    [SerializeField, RequiredMember] private FadeText _statusText;
    [SerializeField, RequiredMember] private TextMeshProUGUI _name;

    [SerializeField, RequiredMember] private PlayerType _playerType;

    private string _id;
    public string Id
    {
        get => _id;
        set
        {
            _id = value;
            _name.text = value[^4..];
            gameObject.SetActive(true);
        }
    }

    private IPlayerLogic _player; // can be real player or AI player
    private bool? _saidUno = null;

    public List<Card> Cards
    {
        get => _cards;
        set => _cards = value;
    }
    public GameObject CardsHolder => _cardsHolder;
    public AvatarPlayer Avatar => _avatar;
    public FadeText StatusText => _statusText;
    public PlayerType PlayerType => _playerType;
    public IPlayerLogic Player => _player;
    public bool? SaidUno
    {
        get => _saidUno;
        set => _saidUno = value;
    }

    void Start()
    {
        if (Scene.IsSingleplayer)
        {
            GameMaster.Instance.CardManager.DrawCards(this, Const.START_CARDS_N,
            _playerType == PlayerType.Player ? CardState.opened : CardState.closed);
        }
        if (_playerType == PlayerType.Player) _player = new PlayerController(this);
        else if (_playerType == PlayerType.AI_Player) _player = new AIPlayer(this);
        else if (_playerType == PlayerType.RemotePlayer)
        {
            gameObject.SetActive(false);
            _player = null;
        }
    }

    public void DrawCards(int amount)
    {
        GameMaster.Instance.CardManager.DrawCards(this, amount,
            PlayerType == PlayerType.Player ? CardState.opened : CardState.closed);
        StatusText.AddPlay(Const.DRAW_TEXT(amount));
    }
}