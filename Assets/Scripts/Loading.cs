using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField]
    private bool ShowText = true;
    public bool showText
    {
        get
        {
            return ShowText;
        }
        set
        {
            ShowText = value;
        }
    }

    [SerializeField]
    private GameObject LoadingGroup;
    public GameObject loadingGroup
    {
        get
        {
            return LoadingGroup;
        }
    }

    private Slider LoadingSlider;
    public Slider slider
    {
        get
        {
            return LoadingSlider;
        }
    }

    private TextMeshProUGUI LoadingText;
    public TextMeshProUGUI text
    {
        get
        {
            return LoadingText;
        }
    }

    [SerializeField]
    private float Max = 1f;
    public float max
    {
        get
        {
            return Max;
        }
        set
        {
            Max = value;
            if (LoadingSlider != null)
            {
                LoadingSlider.maxValue = value;
            }
        }
    }

    [SerializeField]
    private float Value = 0f;
    public float value
    {
        get
        {
            return Value;
        }
        set
        {
            Value = Mathf.Clamp(value, 0, Max);
            if (LoadingSlider != null)
            {
                LoadingSlider.value = value;
            }
        }
    }

    [SerializeField]
    private string Text = "";
    public string textValue
    {
        get
        {
            return Text;
        }
        set
        {
            Text = value;
            if (LoadingText != null)
            {
                LoadingText.text = value;
            }
        }
    }

    public void Tick(string text = "", float value = 1f)
    {
        this.value += value;
        this.textValue = text;
    }

    void Start()
    {
        var loadingGroup = GameObject.Find("LoadingGroup");
        if (loadingGroup != null)
            ChangeLoadingGroup(loadingGroup);
    }

    public void ChangeLoadingGroup(GameObject loadingGroup)
    {
        if(LoadingGroup == loadingGroup)
        {
            Debug.LogWarning("LoadingGroup is same");
            return;
        }

        if (LoadingGroup != null)
        {
            loadingGroup.transform.SetParent(LoadingGroup.transform.parent);
            loadingGroup.transform.localPosition = LoadingGroup.transform.localPosition;
            loadingGroup.transform.SetSiblingIndex(LoadingGroup.transform.GetSiblingIndex());
            Destroy(LoadingGroup);
        }

        LoadingGroup = loadingGroup;
        LoadingSlider = LoadingGroup.GetComponentInChildren<Slider>();
        LoadingText = LoadingGroup.GetComponentInChildren<TextMeshProUGUI>();
        LoadingSlider.maxValue = Max;
        LoadingSlider.value = Value;
        LoadingText.text = Text;
    }
}
