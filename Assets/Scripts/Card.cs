using UnityEngine;
using UnityEngine.UI;

enum CardState { Closed, Opened }

class Card : MonoBehaviour, ICardDefinition
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
    public OtherCards? OtherCard => _other;
    public CardState State
    {
        get => _state;
        set => _state = value;
    }
    public Sprite Sprite => _sprite;

    public void Init(CardType type, SuitColor? color, SuitValue? value, OtherCards? other, Sprite sprite, Sprite closedSprite)
    {
        _type = type;
        _color = color;
        _value = value;
        _other = other;
        _sprite = sprite;
        _state = CardState.Closed;
        SetStateAndSprite(_state, closedSprite);
    }

    public void SetStateAndSprite(CardState state, Sprite closedSprite)
    {
        _state = state;
        if (_state == CardState.Closed)
        {
            GetComponent<Image>().sprite = closedSprite;
        }
        else if (_state == CardState.Opened)
        {
            GetComponent<Image>().sprite = _sprite;
        }
    }
}