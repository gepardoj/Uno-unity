using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Scripting;

public class AbstractCardManager : MonoBehaviour
{
    private static readonly int SUIT_COLORS_N = 4;
    private static readonly int SUIT_VALUES_N = 13;
    private static readonly int SUIT_CARDS_DEF_N = SUIT_COLORS_N * SUIT_VALUES_N;
    private static readonly int OTHER_CARDS_DEF_N = 2;

    [SerializeField, RequiredMember] protected SuitCard[] _suitCardsDef;
    [SerializeField, RequiredMember] protected OtherCard[] _otherCardsDef;
    [SerializeField, RequiredMember] protected Card _cardPrefab;
    [SerializeField, RequiredMember] protected Sprite _closedSprite;
    [SerializeField, RequiredMember] protected GameObject _discardPile;
    [SerializeField, RequiredMember] protected Deck _deck;
    [SerializeField, RequiredMember] protected GameObject _colorPicker;
    [SerializeField, RequiredMember] protected TextMeshProUGUI _currentColorText;
    [SerializeField, ReadOnly] protected SuitColor? _currentColor;

    protected void Start()
    {
        // print("Abstract card manager");
        if (_suitCardsDef == null || _suitCardsDef.Length != SUIT_CARDS_DEF_N)
            throw new Exception($"Suit cards definition should be {SUIT_CARDS_DEF_N}");
        if (_otherCardsDef == null || _otherCardsDef.Length != OTHER_CARDS_DEF_N)
            throw new Exception($"Other cards definitions should be {OTHER_CARDS_DEF_N}");
    }

    protected void CreateCardAndAddTo([Optional] List<Card> cardsDest, GameObject cardsHolder, CardType type, SuitColor? color, SuitValue? value, OtherCards? other, CardState state, [Optional] Vector3 rotation)
    {
        var card = Instantiate(_cardPrefab);
        card.Init(type, color, value, other, state, GetSpriteInCardsDef(type, color, value, other), _closedSprite);
        MoveCardsTo(cardsDest, cardsHolder, new Card[] { card }, state, rotation);
    }

    /// <summary>
    /// Does not remove cards from source
    /// </summary>
    protected void MoveCardsTo([Optional] List<Card> cardsDest, GameObject cardsHolder, IEnumerable<Card> newCards, CardState state,
      [Optional] Vector3 rotation)
    {
        foreach (var card in newCards)
        {
            cardsDest?.Add(card);
            card.SetStateAndSprite(state, _closedSprite);
            card.transform.SetParent(cardsHolder.transform, false);
            card.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(rotation));
        }
        var rotateAround = cardsHolder.GetComponent<RotateAround>();
        if (rotateAround) rotateAround.PlaceCards();
    }

    protected Sprite GetSpriteInCardsDef(CardType type, SuitColor? color, SuitValue? value, OtherCards? other)
    {
        if (type == CardType.suit)
            return _suitCardsDef.First(card => card.Color == color && card.Value == value).Sprite;
        if (type == CardType.other)
            return _otherCardsDef.First(card => card.Value == other).Sprite;
        throw new Exception("Cannot be");
    }
}