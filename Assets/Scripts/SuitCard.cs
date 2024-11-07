using UnityEngine;

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
        _color = color;
        _value = value;
        _sprite = sprite;
    }
}