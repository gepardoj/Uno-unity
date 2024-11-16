using UnityEngine;

[CreateAssetMenu(fileName = "Other", menuName = "Cards/Other", order = 2)]
public class OtherCard : ScriptableObject
{
    [SerializeField] private OtherCards _value;
    [SerializeField] private Sprite _sprite;

    public OtherCards Value => _value;
    public Sprite Sprite => _sprite;
}