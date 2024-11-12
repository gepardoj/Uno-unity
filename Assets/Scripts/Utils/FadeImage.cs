using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FadeImage : FadeInOut
{
    private Image _image;

    void Start()
    {
        _image = GetComponent<Image>();
    }

    protected override void OnFinish()
    {
        _image.color = new Color(1, 1, 1, 0);
    }

    protected override void OnPlaying()
    {
        _image.color = new Color(1, 1, 1, FuncDeltaTime);
    }
}
