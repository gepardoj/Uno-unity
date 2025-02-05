using DG.Tweening;
using UnityEngine;

public class Floating : MonoBehaviour
{
    private Sequence _seq;
    private Quaternion _prevRotation;

    void OnEnable()
    {
        _prevRotation = transform.localRotation;
        transform.localRotation = Quaternion.Euler(92, 14, 180);
        var isForth = true;
        _seq = DOTween.Sequence().SetAutoKill(false);
        _seq.Append(transform.DOMove(transform.position + Vector3.down * .05f, 2).SetEase(Ease.Linear));
        _seq.OnStepComplete(() =>
        {
            _seq.Pause();
            if (isForth)
                _seq.PlayBackwards();
            else _seq.PlayForward();
            isForth = !isForth;
        });
    }

    void OnDisable()
    {
        _seq.Kill();
        transform.localRotation = _prevRotation;
    }
}
