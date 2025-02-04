using System;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{
    public static Action<SuitColor> OnChosenColor;

    void Start()
    {
        GetComponent<RotateAround>().PlaceObjectsAround();
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public static void SetColor(SuitColor color)
    {
        OnChosenColor.Invoke(color);
    }
}