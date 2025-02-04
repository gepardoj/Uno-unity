using UnityEngine;

public class PlaceInRow : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 step;

    void Start()
    {
        Place();
    }

    public void Place()
    {
        var childrenSize = 0;
        foreach (Transform child in transform)
        {
            childrenSize++;
        }
        var _startPosition = startPosition;
        var half = (childrenSize - 1) / 2;
        if (half > 0)
            _startPosition.x -= step.x * half;
        var i = 0;
        foreach (Transform child in transform)
        {
            if (child.parent != transform) return;
            var card = child.GetComponent<Card>();
            if (card)
            {
                card.SetSortingOrder(i);
            }
            else
                child.GetComponent<SpriteRenderer>().sortingOrder = i;
            child.SetLocalPositionAndRotation(_startPosition + step * i, Quaternion.identity);
            i++;
        }
    }
}
