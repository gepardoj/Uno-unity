using UnityEngine;

public class MyCardsHolder : MonoBehaviour
{
    private bool _canClick = false;

    public bool CanClick
    {
        get => _canClick;
        set => _canClick = value;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
            Card highestOrderCard = null;
            foreach (var hit in hits)
            {
                Card card;
                if (card = hit.collider.gameObject.GetComponent<Card>())
                {
                    if (highestOrderCard == null) highestOrderCard = card;
                    var order = card.GetComponent<SpriteRenderer>().sortingOrder;
                    if (order > highestOrderCard.GetComponent<SpriteRenderer>().sortingOrder)
                    {
                        highestOrderCard = card;
                    }
                    // print("hit " + hit.collider.gameObject.name + "" + order);
                }
            }
            if (highestOrderCard) highestOrderCard.OnClick();
        }
    }
}
