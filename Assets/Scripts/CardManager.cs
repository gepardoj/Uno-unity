using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

enum CardType { suit, other }
enum SuitColor { red, green, blue, yellow }
enum SuitValue { _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, cancel, _draw, reverse }
enum OtherCards { wild, wilddraw }


public class CardManager : MonoBehaviour
{
    const int SUIT_COLORS_N = 4;
    const int SUIT_VALUES_N = 13;
    const int SUIT_CARDS_DEF_N = SUIT_COLORS_N * SUIT_VALUES_N;
    const int OTHER_CARDS_DEF_N = 2;

    const int TOTAL_CARDS_N = 108;
    const int WILD_NUMBER = 4;
    const int WILDDRAW_NUMBER = 4;
    const int CARDS_PER_PLAYER_N = 7;


    [SerializeField, RequiredMember] private SuitCard[] _suitCardsDef;
    [SerializeField, RequiredMember] private OtherCard[] _otherCardsDef;

    [SerializeField, RequiredMember] private Card _cardPrefab;
    [SerializeField, RequiredMember] private Sprite _closedSprite;
    [SerializeField, RequiredMember] private GameObject _playerCardHolder;
    [SerializeField, RequiredMember] private GameObject _leftOpponentCardHolder;
    [SerializeField, RequiredMember] private GameObject _topOpponentCardHolder;
    [SerializeField, RequiredMember] private GameObject _rightOpponentCardHolder;

    private List<Card> _availableCards;
    private List<Card> _playerCards;
    private List<Card> _leftOpponentCards;
    private List<Card> _topOpponentCards;
    private List<Card> _rightOpponentCards;

    void Start()
    {
        if (_suitCardsDef == null || _suitCardsDef.Length != SUIT_CARDS_DEF_N)
            throw new Exception($"Suit cards definition should be {SUIT_CARDS_DEF_N}");
        if (_otherCardsDef == null || _otherCardsDef.Length != OTHER_CARDS_DEF_N)
            throw new Exception($"Other cards definitions should be {OTHER_CARDS_DEF_N}");

        GenerateCards();
        ShuffleCards();
        _playerCards = GiveCardsToPlayer(CardState.Opened, _playerCardHolder);
        _leftOpponentCards = GiveCardsToPlayer(CardState.Closed, _leftOpponentCardHolder);
        _topOpponentCards = GiveCardsToPlayer(CardState.Closed, _topOpponentCardHolder);
        _rightOpponentCards = GiveCardsToPlayer(CardState.Closed, _rightOpponentCardHolder);
        print(_availableCards.Count);
    }

    void GenerateCards()
    {
        _availableCards = new List<Card>();
        int i = 0;
        foreach (SuitColor color in Enum.GetValues(typeof(SuitColor)))
        {
            foreach (SuitValue value in Enum.GetValues(typeof(SuitValue)))
            {
                CreateAndAdd(CardType.suit, color, value, null, i++);
                if (value != SuitValue._0) // 1 zero value, rests are by 2
                {
                    CreateAndAdd(CardType.suit, color, value, null, i++);
                }
            }
        }
        foreach (var _ in Enumerable.Range(1, WILD_NUMBER)) CreateAndAdd(CardType.other, null, null, OtherCards.wild, i++);
        foreach (var _ in Enumerable.Range(1, WILDDRAW_NUMBER)) CreateAndAdd(CardType.other, null, null, OtherCards.wilddraw, i++);

        if (_availableCards == null || _availableCards.Count != TOTAL_CARDS_N) throw new Exception($"Total cards should be {TOTAL_CARDS_N}, but found {_availableCards.Count}");
    }

    void CreateAndAdd(CardType type, SuitColor? color, SuitValue? value, OtherCards? other, int index)
    {
        var card = Instantiate(_cardPrefab);
        card.transform.SetParent(transform, false);
        card.Init(type, color, value, other, GetSpriteInCardsDef(type, color, value, other), _closedSprite);
        _availableCards.Add(card);
    }

    Sprite GetSpriteInCardsDef(CardType type, SuitColor? color, SuitValue? value, OtherCards? other)
    {
        if (type == CardType.suit)
            return _suitCardsDef.First(card => card.Color == color && card.Value == value).Sprite;
        if (type == CardType.other)
            return _otherCardsDef.First(card => card.Value == other).Sprite;
        throw new Exception("Cannot be");
    }

    void ShuffleCards()
    {
        var random = new System.Random();
        _availableCards = _availableCards.OrderBy(card => random.NextDouble()).ToList();
    }

    List<Card> GiveCardsToPlayer(CardState state, GameObject cardHolder)
    {
        var playerCards = _availableCards.Take(CARDS_PER_PLAYER_N).ToList();
        _availableCards = _availableCards.Skip(CARDS_PER_PLAYER_N).ToList();
        foreach (var card in playerCards)
        {
            card.SetStateAndSprite(state, _closedSprite);
            card.transform.SetParent(cardHolder.transform, false);
        }
        var rotateAround = cardHolder.GetComponent<RotateAround>();
        if (rotateAround) rotateAround.PlaceCards();
        return playerCards;
    }
}
