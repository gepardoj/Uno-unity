using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class RotateAround : MonoBehaviour
{
    [SerializeField] private float _amountOfCircle = 1; // 1 is full circle, 0.5 is half circle, etc
    [SerializeField] private bool _lookAtPivot = true;
    [SerializeField, RequiredMember] private Vector3 _cardStartingPosition;

    public void PlaceCards()
    {
        var children = new List<RectTransform>(GetComponentsInChildren<RectTransform>());
        children.Remove(GetComponent<RectTransform>());
        float angle = 360.0f / children.Count * _amountOfCircle;
        for (var i = 0; i < children.Count; i++)
        {
            children[i].SetLocalPositionAndRotation(_cardStartingPosition, Quaternion.Euler(Vector3.zero));
            children[i].RotateAround(transform.position, Vector3.up, i * angle);
            if (_lookAtPivot) children[i].LookAt(transform);
        }
    }
}
