using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeRenderer : FadeInOut
{
    private SpriteRenderer _sprite;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    protected override void OnFinish()
    {
        _sprite.color = new Color(1, 1, 1, 0);
    }

    protected override void OnPlaying()
    {
        _sprite.color = new Color(1, 1, 1, FuncDeltaTime);
    }
}
