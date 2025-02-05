using DG.Tweening;
using UnityEngine;

public class MoveTowards : MonoBehaviour
{
    public Tween MoveTo(Transform target, Vector3 rotation)
    {
        var temp = transform.position;
        transform.SetParent(null, false);
        transform.position = temp;
        transform.DORotate(rotation, ClientCardManager.CARD_SPEED, RotateMode.Fast);
        return transform.DOMove(target.position, ClientCardManager.CARD_SPEED).OnComplete(() => transform.SetParent(target));
    }
}