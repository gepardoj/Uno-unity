using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class RotateAround : MonoBehaviour
{
    [SerializeField] private float _amountOfCircle = 1; // 1 is full circle, 0.5 is half circle, etc
    [SerializeField] private bool _lookAtPivot = true;
    [SerializeField, RequiredMember] private Vector3 _cardStartingPosition;
    [SerializeField, RequiredMember] private Vector3 _rotatonVector = new(0, 1, 0);

    public void PlaceObjectsAround()
    {
        var children = new List<RectTransform>(GetComponentsInChildren<RectTransform>());
        children.Remove(GetComponent<RectTransform>());
        float angle = 360.0f / children.Count * _amountOfCircle;
        // print($"{name} children: {children.Count} angle: {angle}");
        var i = 0;
        foreach (Transform child in transform)
        {
            if (child.parent != transform) return;
            child.SetLocalPositionAndRotation(_cardStartingPosition, Quaternion.Euler(Vector3.zero));
            child.RotateAround(transform.position, _rotatonVector, i * angle);
            if (_lookAtPivot) child.LookAt(transform);
            i++;
        }
    }
}
