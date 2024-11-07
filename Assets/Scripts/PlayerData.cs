using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

public enum PlayerType { Player, AI_Player }

public class PlayerData : MonoBehaviour
{
    [SerializeField, ReadOnly] private List<Card> _cards;
    [SerializeField, RequiredMember] private GameObject _cardsHolder;
    [SerializeField, RequiredMember] private Image _avatar;

    [SerializeField, RequiredMember] private PlayerType _playerType;

    IPlayer _player; // can be real player or AI player

    public List<Card> Cards
    {
        get => _cards;
        set => _cards = value;
    }
    public GameObject CardsHolder => _cardsHolder;
    public Image Avatar => _avatar;
    public IPlayer Player => _player;

    void Start()
    {
        if (_playerType == PlayerType.Player) _player = new PlayerController();
        else if (_playerType == PlayerType.AI_Player) _player = new AIPlayer();
    }
}