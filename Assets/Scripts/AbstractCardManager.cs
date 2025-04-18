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

    protected int cardOrderInLayer = 0;

    [SerializeField, RequiredMember] protected SuitCard[] _suitCardsDef;
    [SerializeField, RequiredMember] protected OtherCard[] _otherCardsDef;
    [SerializeField, RequiredMember] protected Card _cardPrefab;
    [SerializeField, RequiredMember] protected Sprite _closedSprite;
    [SerializeField, RequiredMember] protected GameObject _discardPile;
    [SerializeField, RequiredMember] protected Transform _startingCardOrigin;
    [SerializeField, RequiredMember] protected Deck _deck;
    [SerializeField, RequiredMember] protected ColorPicker _colorPicker;
    [SerializeField, RequiredMember] protected TextMeshProUGUI _currentColorText;
    [SerializeField, ReadOnly] protected SuitColor? _currentColor;

    public GameObject DiscardPile => _discardPile;
    public Deck Deck => _deck;
    public ColorPicker ColorPicker => _colorPicker;
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

    protected void Start()
    {
        if (_suitCardsDef == null || _suitCardsDef.Length != SUIT_CARDS_DEF_N)
            throw new Exception($"Suit cards definition should be {SUIT_CARDS_DEF_N}");
        if (_otherCardsDef == null || _otherCardsDef.Length != OTHER_CARDS_DEF_N)
            throw new Exception($"Other cards definitions should be {OTHER_CARDS_DEF_N}");
    }

    protected IEnumerable<Card> CreateCardAndAddTo(Transform fromOrigin, [Optional] List<Card> cardsDest, GameObject cardsHolder, CardType type, SuitColor? color, SuitValue? value, OtherCards? other, CardState state, [Optional] Vector3 rotation, Action<Card> onFinish)
    {
        var card = Instantiate(_cardPrefab, fromOrigin);
        card.Init(type, color, value, other, state, GetSpriteInCardsDef(type, color, value, other), _closedSprite);
        return MoveCardsTo(cardsDest, cardsHolder, new Card[] { card }, state, rotation, onFinish);
    }

    protected IEnumerable<Card> MoveCardsToDiscardPile(IEnumerable<Card> cards, Action<Card> onFinish)
    {
        return MoveCardsTo(null, _discardPile, cards, CardState.opened,
        new Vector3(90, 0, UnityEngine.Random.Range(0, 360)), onFinish);
    }

    /// <summary>
    /// Does not remove cards from source
    /// </summary>
    protected IEnumerable<Card> MoveCardsTo([Optional] List<Card> cardsDest, GameObject cardsHolder, IEnumerable<Card> newCards, CardState state,
      [Optional] Vector3 rotation, [Optional] Action<Card> onFinish)
    {
        foreach (var card in newCards)
        {
            cardsDest?.Add(card);
            card.SetStateAndSprite(state, _closedSprite);
            card.SetSortingOrder(cardOrderInLayer++);
            card.GetComponent<MoveTowards>().MoveTo(cardsHolder.transform, rotation).onComplete += () =>
            {
                onFinish(card);
                if (cardsHolder == DiscardPile)
                    card.FloatTopCard(DiscardPile.transform);
            };
        }
        return newCards;
    }

    protected Sprite GetSpriteInCardsDef(CardType type, SuitColor? color, SuitValue? value, OtherCards? other)
    {
        if (type == CardType.suit)
            return _suitCardsDef.First(card => card.Color == color && card.Value == value).Sprite;
        if (type == CardType.other)
            return _otherCardsDef.First(card => card.Value == other).Sprite;
        throw new Exception("Unknown card type!");
    }
}