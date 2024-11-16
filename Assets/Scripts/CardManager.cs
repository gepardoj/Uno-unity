using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class CardManager : AbstractCardManager
{
    private static readonly int TOTAL_CARDS_N = 108;
    private static readonly int WILD_NUMBER = 4;
    private static readonly int WILDDRAW_NUMBER = 4;

    [SerializeField, ReadOnly] private List<Card> _availableCards = new();
    [SerializeField, ReadOnly] private List<Card> _discardedCards = new();

    public Deck Deck => _deck;
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
        _colorPicker.GetComponent<RotateAround>().PlaceCards();

        GenerateCards();
        _availableCards = ShuffleCards(_availableCards);
        PopupFirstCard();

        // DEBUG:

        // var temp = new List<Card>();
        // CreateAndAddCardTo(temp, CardType.suit, SuitColor.red, SuitValue._draw, null);
        // CreateAndAddCardTo(temp, CardType.other, null, null, OtherCards.wild);
        // MoveCardToDrop(temp, temp.First(), null);
        // print($"Available = {_availableCards.Count}");
    }

    public void DEBUG_EmptyDeck()
    {
        MoveCardsTo(_discardedCards, _discardPile, _availableCards.GetRange(0, _availableCards.Count),
        CardState.opened, new Vector3(0, 0, UnityEngine.Random.Range(0, 361)));
        _availableCards.RemoveRange(0, _availableCards.Count);
    }

    void GenerateCards()
    {
        foreach (SuitColor color in Enum.GetValues(typeof(SuitColor)))
        {
            foreach (SuitValue value in Enum.GetValues(typeof(SuitValue)))
            {
                CreateAndAddCardToDeck(CardType.suit, color, value, null);
                if (value != SuitValue._0) // 1 zero value, rests are by 2
                {
                    CreateAndAddCardToDeck(CardType.suit, color, value, null);
                }
            }
        }
        foreach (var _ in Enumerable.Range(1, WILD_NUMBER)) CreateAndAddCardToDeck(CardType.other, null, null, OtherCards.wild);
        foreach (var _ in Enumerable.Range(1, WILDDRAW_NUMBER)) CreateAndAddCardToDeck(CardType.other, null, null, OtherCards.wilddraw);

        if (_availableCards == null || _availableCards.Count != TOTAL_CARDS_N)
            throw new Exception($"Total cards should be {TOTAL_CARDS_N}, but found {_availableCards.Count}");
    }

    void CreateAndAddCardToDeck(CardType type, SuitColor? color, SuitValue? value, OtherCards? other)
    {
        CreateCardAndAddTo(_availableCards, gameObject, type, color, value, other, CardState.closed);
    }

    List<Card> ShuffleCards(List<Card> cards)
    {
        var random = new System.Random();
        return cards.OrderBy(card => random.NextDouble()).ToList();
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
        MoveCardsTo(_discardedCards, _discardPile, new Card[] { foundCard }, CardState.opened);
    }

    public void RemAndMoveCardToDiscardPile(List<Card> cardsSource, Card card, SuitColor? color)
    {
        Utils.RemoveAndGetElement(cardsSource, card);
        CurrentColor = card.Type == CardType.other && color != null ? color : card.Color;
        MoveCardsTo(_discardedCards, _discardPile, new Card[] { card },
            CardState.opened, new Vector3(0, 0, UnityEngine.Random.Range(0, 361)));
    }

    public bool TryMoveCardToDrop(List<Card> cardsSource, Card card, SuitColor? color)
    {
        if (IsCardMatchLastInDiscardPile(card))
        {
            RemAndMoveCardToDiscardPile(cardsSource, card, color);
            //print("match");
            return true;
        }
        //print($"player = {cardsSource.Count} drop = {_dropCards.Count}");
        return false;
    }

    public void TakeNewCards(PlayerData player, int cardsAmount, CardState cardState)
    {
        if (IsDeckEmptyThenShuffle(cardsAmount))
            player.StatusText.AddPlay(Const.SHUFFLED_TEXT);
        var newCards = Utils.RemoveAndGetFirstNElements(_availableCards, cardsAmount); // first N element
        MoveCardsTo(player.Cards, player.CardsHolder, newCards, cardState);
    }

    bool IsDeckEmptyThenShuffle(int amount)
    {
        if (_availableCards.Count < amount)
        {
            var cards = Utils.RemoveAndGetFirstNElements(_discardedCards, _discardedCards.Count - 1);
            cards = ShuffleCards(cards);
            MoveCardsTo(_availableCards, gameObject, cards, CardState.closed);
            return true;
        }
        return false;
    }

    public bool IsCardMatchLastInDiscardPile(Card card)
    {
        if (_discardedCards.Count == 0) throw new Exception("The drop is empty!");
        var lastCardInDrop = _discardedCards.Last();
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

    public Card GetFirstCardInDiscardPile()
    {
        return _discardedCards.First();
    }
}