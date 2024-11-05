using UnityEngine;
using UnityEngine.UI;

class Card : MonoBehaviour, ICardDefinition
{
    private CardType _type;
    private SuitColor? _color;
    private SuitValue? _value;
    private OtherCards? _other;
    private bool _closed;
    private Sprite _sprite;

    public CardType Type => _type;
    public SuitColor? Color => _color;
    public SuitValue? Value => _value;
    public OtherCards? OtherCard => _other;
    public bool Closed
    {
        get => _closed;
        set => _closed = value;
    }
    public Sprite Sprite => _sprite;

    public void Init(CardType type, SuitColor? color, SuitValue? value, OtherCards? other, Sprite sprite)
    {
        _type = type;
        _color = color;
        _value = value;
        _other = other;
        _sprite = sprite;
        _closed = false;
        GetComponent<Image>().sprite = _sprite;
    }
}