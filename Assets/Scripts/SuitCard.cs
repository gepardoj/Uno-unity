using UnityEngine;

[CreateAssetMenu(fileName = "Suit", menuName = "Cards/Suit card", order = 1)]
class SuitCard : ScriptableObject
{
    [SerializeField] private SuitColor _color;
    [SerializeField] private SuitValue _value;

    [SerializeField] private Sprite _sprite;

    public SuitColor Color => _color;
    public SuitValue Value => _value;
    public Sprite Sprite => _sprite;
}