using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AvatarPlayer : FadeInOut
{
    private Image _image;
    [SerializeField, RequiredMember] private TextMeshProUGUI _playerName;

    void Start()
    {
        _image = GetComponent<Image>();
    }

    protected override void OnFinish()
    {
        _image.color = new Color(1, 1, 1, 0);
        _playerName.alpha = 0;
    }

    protected override void OnPlaying()
    {
        _image.color = new Color(1, 1, 1, FuncDeltaTime);
        _playerName.alpha = FuncDeltaTime;
    }

    public void Highlight(bool state)
    {
        _image.color = state ? Color.yellow : Color.white;
    }

    public void SetSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }
}
