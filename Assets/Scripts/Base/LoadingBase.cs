using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingBase : MonoBehaviour
{
[SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private Slider _slider;

    public string text
    {
        get
        {
            return _text.text;
        }
        set
        {
            _text.text = value;
        }
    }

    public float progress
    {
        get
        {
            return _slider.value;
        }
        set
        {
            _slider.value = value;
        }
    }

    public float maxValue
    {
        get
        {
            return _slider.maxValue;
        }
        set
        {
            _slider.maxValue = value;
        }
    }

    public void Tick(string text = "", float progressAdd = 1)
    {
        Debug.Log($"LoadingBase Tick: {text} {progressAdd} {_text.text} {_slider.value}/{_slider.maxValue}");
        _text.text = text;
        _slider.value += progressAdd;
    }


    // Start is called before the first frame update
    void Start()
    {
        _text.text = "";
        _slider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
