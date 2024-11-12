using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FadeText : MonoBehaviour
{
    static readonly float DELAY_TO_ADD = 0.5f;

    [SerializeField, RequiredMember] private float _duration = 1f;
    [SerializeField, RequiredMember] private Vector3 _direction = Vector3.zero;
    private bool _clone = false;
    private float _deltaTime = 0f;
    private bool _playing = false;
    private int _vectorAlphaY;
    private TextMeshProUGUI _textUI;
    private float _delayToAdd = 0f;

    public bool IsClone => _clone;
    public bool IsOrigin => !_clone;

    void Start()
    {
        _textUI = GetComponent<TextMeshProUGUI>();
    }

    public void AddPlay(string text)
    {
        StartCoroutine(WaitAddPlay(text));
    }

    IEnumerator WaitAddPlay(string text)
    {
        var delay = _delayToAdd;
        _delayToAdd += DELAY_TO_ADD;
        yield return new WaitForSeconds(delay);
        var clone = Instantiate(this, transform.parent);
        clone._clone = true;
        clone.Play(text);
    }

    public void Play(string text)
    {
        _textUI = GetComponent<TextMeshProUGUI>();
        _textUI.text = text;
        _textUI.alpha = 0;
        _deltaTime = 0f;
        _playing = true;
        StartCoroutine(Fade());
    }

    void FadeInText()
    {
        _vectorAlphaY = 1;
    }

    void FadeOutText()
    {
        _vectorAlphaY = -1;
        // print("Changing direction");
    }

    IEnumerator Fade()
    {
        FadeInText();
        yield return new WaitForSeconds(_duration / 2);
        FadeOutText();
        yield return new WaitForSeconds(_duration / 2);
        _playing = false;
        _textUI.alpha = 0;
        if (IsClone) Destroy(gameObject);
    }

    void Update()
    {
        if (IsOrigin)
        {
            if (_delayToAdd > 0)
                _delayToAdd -= Time.deltaTime;
            if (_delayToAdd < 0) _delayToAdd = 0;
        }
        if (_playing)
        {
            var halfDuration = _duration / 2;
            _deltaTime += Time.deltaTime * _vectorAlphaY / halfDuration;
            _textUI.alpha = Math.EaseOutSine(_deltaTime);
            gameObject.transform.localPosition += _deltaTime * _direction;
            // print($"Raw: {_alpha}, Func: {_textUI.alpha}");
        }
    }
}
