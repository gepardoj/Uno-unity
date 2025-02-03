using DG.Tweening;
using UnityEngine;

public class MoveTowards : MonoBehaviour
{
    [SerializeField] private float duration = 1;
    public Tween MoveTo(Transform target, Vector3 rotation)
    {
        var temp = transform.position;
        transform.SetParent(null, true);
        transform.position = temp;
        transform.DORotate(rotation, duration, RotateMode.Fast);
        return transform.DOMove(target.position, duration).OnComplete(() => transform.SetParent(target));
    }
}