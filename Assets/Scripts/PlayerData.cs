using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

public enum PlayerType { Player, AI_Player }

public class PlayerData : MonoBehaviour
{
    [SerializeField, ReadOnly] private List<Card> _cards = new();
    [SerializeField, RequiredMember] private GameObject _cardsHolder;
    [SerializeField, RequiredMember] private Image _avatar;

    [SerializeField, RequiredMember] private PlayerType _playerType;

    IPlayerLogic _player; // can be real player or AI player

    public List<Card> Cards
    {
        get => _cards;
        set => _cards = value;
    }
    public GameObject CardsHolder => _cardsHolder;
    public Image Avatar => _avatar;
    public IPlayerLogic Player => _player;

    void Start()
    {
        GameMaster.Instance.CardManager.TakeNewCards(_cards, _cardsHolder, CardManager.START_CARDS_N,
            _playerType == PlayerType.Player ? CardState.opened : CardState.closed);

        if (_playerType == PlayerType.Player) _player = new PlayerController(this);
        else if (_playerType == PlayerType.AI_Player) _player = new AIPlayer(this);
    }
}