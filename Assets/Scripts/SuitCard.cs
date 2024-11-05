using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Suit", menuName = "Cards/Suit card", order = 1)]
class SuitCard : ScriptableObject, ICardDefinition
{
    [SerializeField] private SuitColor _color;
    [SerializeField] private SuitValue _value;

    [SerializeField] private Sprite _sprite;

    public SuitColor Color => _color;
    public SuitValue Value => _value;
    public Sprite Sprite => _sprite;

    public SuitCard(SuitColor color, SuitValue value, Sprite sprite)
    {
        this._color = color;
        this._value = value;
        this._sprite = sprite;
    }

    public ICardDefinition CreateInstance(SuitColor color, SuitValue value, Sprite sprite)
    {
        var clone = ScriptableObject.Instantiate(this);
        clone._color = color;
        clone._value = value;
        clone._sprite = sprite;
        clone.name = clone.name[..^7]; // Remove (Clone) from name
        return clone;
    }
}