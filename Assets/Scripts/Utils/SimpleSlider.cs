using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SimpleSlider : MonoBehaviour
{
    private float _duration = 1f;
    private bool _isPlaying = false;
    private Slider _slider;

    public void Play(float timeS)
    {
        gameObject.SetActive(true);
        _slider = GetComponent<Slider>();
        _slider.value = timeS;
        _slider.maxValue = timeS;
        _duration = timeS;
        StartCoroutine(Playing());
    }

    IEnumerator Playing()
    {
        _isPlaying = true;
        yield return new WaitForSeconds(_duration);
        _isPlaying = false;
        gameObject.SetActive(false);
    }

    public void Stop()
    {
        _isPlaying = false;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (_isPlaying)
        {
            _slider.value -= Time.deltaTime;
        }
    }
}
