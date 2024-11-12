using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FadeText : FadeInOut
{
    private TextMeshProUGUI _textUI;

    void Start()
    {
        _textUI = GetComponent<TextMeshProUGUI>();
    }

    public void AddPlay(string text)
    {
        _textUI = GetComponent<TextMeshProUGUI>();
        _textUI.text = text;
        _textUI.alpha = 0;
        AddPlay();
    }

    public void Play(string text)
    {
        _textUI = GetComponent<TextMeshProUGUI>();
        _textUI.text = text;
        _textUI.alpha = 0;
        Play();
    }

    protected override void OnFinish()
    {
        _textUI.alpha = 0;
    }

    protected override void OnPlaying()
    {
        _textUI.alpha = FuncDeltaTime;
    }
}
