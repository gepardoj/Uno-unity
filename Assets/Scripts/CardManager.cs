using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

public class CardManager : MonoBehaviour
{
    const int SUIT_COLORS_N = 4;
    const int SUIT_VALUES_N = 13;
    const int SUIT_CARDS_DEF_N = SUIT_COLORS_N * SUIT_VALUES_N;
    const int OTHER_CARDS_DEF_N = 2;

    const int TOTAL_CARDS_N = 108;
    const int WILD_NUMBER = 4;
    const int WILDDRAW_NUMBER = 4;
    public static int START_CARDS_N = 7;


    [SerializeField, RequiredMember] private SuitCard[] _suitCardsDef;
    [SerializeField, RequiredMember] private OtherCard[] _otherCardsDef;

    [SerializeField, RequiredMember] private Card _cardPrefab;
    [SerializeField, RequiredMember] private Sprite _closedSprite;

    [SerializeField, RequiredMember] private GameObject _dropCardsHolder;

    private List<Card> _availableCards;
    private List<Card> _dropCards;

    public void ManualInit()
    {
        if (_suitCardsDef == null || _suitCardsDef.Length != SUIT_CARDS_DEF_N)
            throw new Exception($"Suit cards definition should be {SUIT_CARDS_DEF_N}");
        if (_otherCardsDef == null || _otherCardsDef.Length != OTHER_CARDS_DEF_N)
            throw new Exception($"Other cards definitions should be {OTHER_CARDS_DEF_N}");

        GenerateCards();
        ShuffleCards();
        _dropCards = new List<Card>();
        PopupUntilSuitCardAndMove();
        // print($"Available = {_availableCards.Count}");
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

    // TODO: check if redundant
    public List<Card> GiveCardsToPlayer(int cardsAmount, CardState state, GameObject cardHolder)
    {
        var playerCards = _availableCards.Take(cardsAmount).ToList();
        _availableCards.RemoveRange(0, cardsAmount);
        foreach (var card in playerCards)
        {
            card.SetStateAndSprite(state, _closedSprite);
            card.transform.SetParent(cardHolder.transform, false);
        }
        var rotateAround = cardHolder.GetComponent<RotateAround>();
        if (rotateAround) rotateAround.PlaceCards();
        return playerCards;
    }

    void PopupUntilSuitCardAndMove()
    {
        var i = 0;
        Card foundCard = null;
        foreach (var card in _availableCards)
        {
            if (card.Type == CardType.suit)
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
        MoveCardTo(_dropCards, _dropCardsHolder, new Card[] { foundCard }, CardState.opened);
    }

    void MoveCardTo(List<Card> cardsDest, GameObject cardsHolder, IEnumerable<Card> newCards, CardState cardState)
    {
        foreach (var card in newCards)
        {
            cardsDest.Add(card);
            card.SetStateAndSprite(cardState, _closedSprite);
            card.transform.SetParent(cardsHolder.transform, false);
            card.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 361)));
        }
        var rotateAround = cardsHolder.GetComponent<RotateAround>();
        if (rotateAround) rotateAround.PlaceCards();
    }

    public void MoveCardToDrop(List<Card> cardsSource, Card card)
    {
        var foundCard = Utils.RemoveAndGetElement(cardsSource, card);
        MoveCardTo(_dropCards, _dropCardsHolder, new Card[] { foundCard }, CardState.opened);
    }

    public bool TryMoveCardToDrop(List<Card> cardsSource, Card card)
    {
        if (IsCardMatchLastDrop(card))
        {
            var foundCard = Utils.RemoveAndGetElement(cardsSource, card);
            MoveCardTo(_dropCards, _dropCardsHolder, new Card[] { foundCard }, CardState.opened);
            //print("match");
            return true;
        }
        //print($"player = {cardsSource.Count} drop = {_dropCards.Count}");
        return false;
    }

    public void TakeNewCards(List<Card> cardsDest, GameObject cardsHolder, int cardsAmount, CardState cardState)
    {
        var newCards = Utils.RemoveAndGetFirstElements(_availableCards, cardsAmount); // first N element
        cardsDest.AddRange(newCards);
        MoveCardTo(cardsDest, cardsHolder, newCards, cardState);
    }

    public bool IsCardMatchLastDrop(Card card)
    {
        if (_dropCards.Count == 0) return true;
        var lastCardInDrop = _dropCards.Last();
        if (card.Type == CardType.suit)
        {
            return lastCardInDrop.Type == CardType.suit &&
            (card.Color == lastCardInDrop.Color || card.Value == lastCardInDrop.Value);
        }
        else if (card.Type == CardType.other)
        {
            return true;
        }
        return false;
    }
}
