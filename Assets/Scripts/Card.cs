using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CardState { closed, opened }
public enum CardType { suit, other }
public enum SuitColor { red, green, blue, yellow }
public enum SuitValue { _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, cancel, _draw, reverse }
public enum OtherCards { wild, wilddraw }

public struct CardData
{
    private CardType _type;
    private SuitColor? _color;
    private SuitValue? _value;
    private OtherCards? _other;
    private CardState _state;

    public CardType Type => _type;
    public SuitColor? Color => _color;
    public SuitValue? Value => _value;
    public OtherCards? Other => _other;
    public CardState State { get => _state; set => _state = value; }

    public CardData(CardType type, SuitColor? color, SuitValue? value, OtherCards? other, CardState state)
    {
        _type = type;
        _color = color;
        _value = value;
        _other = other;
        _state = state;
    }

    public override string ToString()
    {
        return $"{Type} {Color} {Value} {Other}";
    }

}

public class Card : MonoBehaviour, IPointerClickHandler
{
    private CardType _type;
    private SuitColor? _color;
    private SuitValue? _value;
    private OtherCards? _other;
    private CardState _state;
    private Sprite _sprite;

    public CardType Type => _type;
    public SuitColor? Color => _color;
    public SuitValue? Value => _value;
    public OtherCards? Other => _other;
    public CardState State => _state;
    public Sprite Sprite => _sprite;

    public bool IsColoredAction
        => Value == SuitValue.cancel || Value == SuitValue._draw || Value == SuitValue.reverse;
    public bool IsWild
        => Other == OtherCards.wild;
    public bool IsWildDraw
        => Other == OtherCards.wilddraw;


    public void Init(CardType type, SuitColor? color, SuitValue? value, OtherCards? other, CardState state, Sprite sprite, Sprite closedSprite)
    {
        _type = type;
        _color = color;
        _value = value;
        _other = other;
        _sprite = sprite;
        _state = state;
        SetStateAndSprite(_state, closedSprite);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var myCardsHolder = transform.parent.GetComponent<MyCardsHolder>();
        if (myCardsHolder) myCardsHolder.OnSelectCard(this);
    }

    public void SetStateAndSprite(CardState state, Sprite closedSprite)
    {
        _state = state;
        if (_state == CardState.closed)
        {
            GetComponent<Image>().sprite = closedSprite;
        }
        else if (_state == CardState.opened)
        {
            GetComponent<Image>().sprite = _sprite;
        }
    }

    public static CardData MapFromBytes(byte[] data)
    {
        var offset = 0;
        var type = (CardType)data[offset++];
        var color = data[offset++];
        var value = data[offset++];
        var other = data[offset++];
        var state = (CardState)data[offset++];
        return new CardData(
            type,
            color == API.BYTE_NULL ? null : (SuitColor)color,
            value == API.BYTE_NULL ? null : (SuitValue)value,
            other == API.BYTE_NULL ? null : (OtherCards)other,
            state
        );
    }

    public byte[] ToBytes()
    {
        return new byte[] {
            (byte)Type,
            Color == null ? API.BYTE_NULL : (byte)Color,
            Value == null ? API.BYTE_NULL : (byte)Value,
            Other == null ? API.BYTE_NULL : (byte)Other,
            (byte)State
        };
    }
}