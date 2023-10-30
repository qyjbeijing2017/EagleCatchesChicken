using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingBase : MonoBehaviour
{
[SerializeField]
    private TextMeshProUGUI m_Text;
    [SerializeField]
    private Slider m_Slider;

    public string text
    {
        get
        {
            return m_Text.text;
        }
        set
        {
            m_Text.text = value;
        }
    }

    public float progress
    {
        get
        {
            return m_Slider.value;
        }
        set
        {
            m_Slider.value = value;
        }
    }

    public float maxValue
    {
        get
        {
            return m_Slider.maxValue;
        }
        set
        {
            m_Slider.maxValue = value;
        }
    }

    public void Tick(string text = "", float progressAdd = 0)
    {
        m_Text.text = text;
        m_Slider.value += progressAdd;
    }


    // Start is called before the first frame update
    void Start()
    {
        m_Text.text = "";
        m_Slider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
