using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Scripting;

public class CardManager : MonoBehaviour
{
    private static readonly int SUIT_COLORS_N = 4;
    private static readonly int SUIT_VALUES_N = 13;
    private static readonly int SUIT_CARDS_DEF_N = SUIT_COLORS_N * SUIT_VALUES_N;
    private static readonly int OTHER_CARDS_DEF_N = 2;
    private static readonly int TOTAL_CARDS_N = 108;
    private static readonly int WILD_NUMBER = 4;
    private static readonly int WILDDRAW_NUMBER = 4;

    public static readonly int START_CARDS_N = 7;
    public static readonly int PULL_CARDS_N = 1;
    public static readonly int DRAW_CARDS_N = 2;
    public static readonly int WILDDRAW_CARDS_N = 4;


    [SerializeField, RequiredMember] private SuitCard[] _suitCardsDef;
    [SerializeField, RequiredMember] private OtherCard[] _otherCardsDef;

    [SerializeField, RequiredMember] private Card _cardPrefab;
    [SerializeField, RequiredMember] private Sprite _closedSprite;

    [SerializeField, RequiredMember] private GameObject _dropCardsHolder;
    [SerializeField, RequiredMember] private CardsPull _cardsPull;

    [SerializeField, RequiredMember] private GameObject _colorPicker;

    [SerializeField, RequiredMember] private TextMeshProUGUI _currentColorText;
    [SerializeField, ReadOnly] private SuitColor? _currentColor;


    [SerializeField, ReadOnly] private List<Card> _availableCards = new();
    [SerializeField, ReadOnly] private List<Card> _dropCards = new();

    public CardsPull CardsPull => _cardsPull;
    public GameObject ColorPicker => _colorPicker;
    public SuitColor? CurrentColor
    {
        get => _currentColor;
        set
        {
            _currentColor = value;
            var text = value != null ? Enum.GetName(typeof(SuitColor), value) : "Unknown";
            _currentColorText.text = $"Current Color: {text}";
        }
    }

    public void ManualInit()
    {
        if (_suitCardsDef == null || _suitCardsDef.Length != SUIT_CARDS_DEF_N)
            throw new Exception($"Suit cards definition should be {SUIT_CARDS_DEF_N}");
        if (_otherCardsDef == null || _otherCardsDef.Length != OTHER_CARDS_DEF_N)
            throw new Exception($"Other cards definitions should be {OTHER_CARDS_DEF_N}");

        _colorPicker.GetComponent<RotateAround>().PlaceCards();

        GenerateCards();
        ShuffleCards();
        PopupFirstCard();

        /// DEBUG:
        // var temp = new List<Card>();
        // CreateAndAddCardTo(temp, CardType.suit, SuitColor.red, SuitValue._draw, null);
        // CreateAndAddCardTo(temp, CardType.other, null, null, OtherCards.wild);
        // MoveCardToDrop(temp, temp.First(), null);
        // print($"Available = {_availableCards.Count}");
    }

    void GenerateCards()
    {
        foreach (SuitColor color in Enum.GetValues(typeof(SuitColor)))
        {
            foreach (SuitValue value in Enum.GetValues(typeof(SuitValue)))
            {
                CreateAndAddCardToPull(CardType.suit, color, value, null);
                if (value != SuitValue._0) // 1 zero value, rests are by 2
                {
                    CreateAndAddCardToPull(CardType.suit, color, value, null);
                }
            }
        }
        foreach (var _ in Enumerable.Range(1, WILD_NUMBER)) CreateAndAddCardToPull(CardType.other, null, null, OtherCards.wild);
        foreach (var _ in Enumerable.Range(1, WILDDRAW_NUMBER)) CreateAndAddCardToPull(CardType.other, null, null, OtherCards.wilddraw);

        if (_availableCards == null || _availableCards.Count != TOTAL_CARDS_N) throw new Exception($"Total cards should be {TOTAL_CARDS_N}, but found {_availableCards.Count}");
    }

    void CreateAndAddCardToPull(CardType type, SuitColor? color, SuitValue? value, OtherCards? other)
    {
        CreateAndAddCardTo(_availableCards, type, color, value, other);
    }

    void CreateAndAddCardTo(List<Card> cardsDest, CardType type, SuitColor? color, SuitValue? value, OtherCards? other)
    {
        var card = Instantiate(_cardPrefab);
        card.transform.SetParent(transform, false);
        card.Init(type, color, value, other, GetSpriteInCardsDef(type, color, value, other), _closedSprite);
        cardsDest.Add(card);
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

    /// <summary>
    ///  Pop a card from the cards stack to the drop. 
    ///  If Wild Draw found, then it goes to the end of the stack. And next card will popup
    /// </summary>
    void PopupFirstCard()
    {
        var i = 0;
        Card foundCard = null;
        foreach (var card in _availableCards)
        {
            if (!card.IsWildDraw)
            {
                foundCard = card;
                break;
            }
            i++;
        }
        _availableCards.RemoveAt(i);
        if (i >= 1)
        {
            var cards = _availableCards.GetRange(0, i);
            _availableCards.AddRange(cards);
        }
        CurrentColor = foundCard.Color;
        MoveCardsTo(_dropCards, _dropCardsHolder, new Card[] { foundCard }, CardState.opened);
    }

    /// <summary>
    /// Does not remove cards from source
    /// </summary>
    void MoveCardsTo(List<Card> cardsDest, GameObject cardsHolder, IEnumerable<Card> newCards, CardState cardState,
    [Optional] Vector3 rotation)
    {
        foreach (var card in newCards)
        {
            cardsDest.Add(card);
            card.SetStateAndSprite(cardState, _closedSprite);
            card.transform.SetParent(cardsHolder.transform, false);
            card.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(rotation));
        }
        var rotateAround = cardsHolder.GetComponent<RotateAround>();
        if (rotateAround) rotateAround.PlaceCards();
    }

    public void MoveCardToDrop(List<Card> cardsSource, Card card, SuitColor? color)
    {
        Utils.RemoveAndGetElement(cardsSource, card);
        CurrentColor = card.Type == CardType.other && color != null ? color : card.Color;
        MoveCardsTo(_dropCards, _dropCardsHolder, new Card[] { card },
            CardState.opened, new Vector3(0, 0, UnityEngine.Random.Range(0, 361)));
    }

    public bool TryMoveCardToDrop(List<Card> cardsSource, Card card, SuitColor? color)
    {
        if (IsCardMatchLastDrop(card))
        {
            MoveCardToDrop(cardsSource, card, color);
            //print("match");
            return true;
        }
        //print($"player = {cardsSource.Count} drop = {_dropCards.Count}");
        return false;
    }

    public void TakeNewCards(PlayerData player, int cardsAmount, CardState cardState)
    {
        var newCards = Utils.RemoveAndGetFirstElements(_availableCards, cardsAmount); // first N element
        MoveCardsTo(player.Cards, player.CardsHolder, newCards, cardState);
    }

    public bool IsCardMatchLastDrop(Card card)
    {
        if (_dropCards.Count == 0) throw new Exception("The drop is empty!");
        var lastCardInDrop = _dropCards.Last();
        if (card.Type == CardType.suit)
        {
            return card.Color == CurrentColor || card.Value == lastCardInDrop.Value;
        }
        else if (card.Type == CardType.other)
        {
            return true;
        }
        return false;
    }

    public Card GetFirstCardInDrop()
    {
        return _dropCards.First();
    }
}
