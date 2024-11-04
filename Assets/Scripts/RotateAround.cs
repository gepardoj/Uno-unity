using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [SerializeField] private float _amountOfCircle = 1;
    void Start()
    {
        var children = GetComponentsInChildren<RectTransform>();
        float angle = 360.0f / children.Length * _amountOfCircle;
        for (var i = 0; i < children.Length; i++)
        {
            children[i].transform.RotateAround(transform.position, Vector3.up, i * angle);
            children[i].LookAt(transform);
        }
    }
}
