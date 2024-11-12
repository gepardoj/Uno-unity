using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

public abstract class FadeInOut : MonoBehaviour
{
    static readonly float DELAY_TO_ADD = 0.5f;

    [SerializeField, RequiredMember] private float _duration = 1f;
    [SerializeField, RequiredMember] private Vector3 _direction = Vector3.zero;
    private bool _clone = false;
    private float _deltaTime = 0f;
    private float _funcDeltaTime = 0f;
    private bool _playing = false;
    private int _vectorAlphaY;
    private float _delayToAdd = 0f;

    private bool IsClone => _clone;
    private bool IsOrigin => !_clone;

    protected float FuncDeltaTime => _funcDeltaTime;

    protected abstract void OnFinish();
    protected abstract void OnPlaying();


    public void AddPlay()
    {
        StartCoroutine(WaitAddPlay());
    }

    private IEnumerator WaitAddPlay()
    {
        var delay = _delayToAdd;
        _delayToAdd += DELAY_TO_ADD;
        yield return new WaitForSeconds(delay);
        var clone = Instantiate(this, transform.parent);
        clone._clone = true;
        clone.Play();
    }

    public void Play()
    {
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

    private IEnumerator Fade()
    {
        FadeInText();
        yield return new WaitForSeconds(_duration / 2);
        FadeOutText();
        yield return new WaitForSeconds(_duration / 2);
        _playing = false;
        OnFinish();
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
            _funcDeltaTime = Math.EaseOutSine(_deltaTime);
            gameObject.transform.localPosition += _deltaTime * _direction;
            OnPlaying();
            // print($"Raw: {_alpha}, Func: {_textUI.alpha}");
        }
    }
}
