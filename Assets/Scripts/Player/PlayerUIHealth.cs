using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHealth : MonoBehaviour
{
    [SerializeField] 
    private Slider HealthBar;

    List<Slider> HealthBarList = new List<Slider>();

    Camera mainCamera;
    void Awake()
    {
        mainCamera = Camera.main;
        Button btn = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        var sources = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
        for(int i = 0; i < sources.Length; i++)
        {
            if (HealthBarList.Count <= i)
            {
                var newHealthBar = Instantiate(HealthBar, transform);
                HealthBarList.Add(newHealthBar);
            }
            HealthBarList[i].value = sources[i].healthPercent;
            var screenPos = mainCamera.WorldToScreenPoint(sources[i].healthBar.position);
            HealthBarList[i].transform.position = screenPos;
            HealthBarList[i].gameObject.SetActive(true);
        }

        for(int i = sources.Length; i < HealthBarList.Count; i++)
        {
            HealthBarList[i].gameObject.SetActive(false);
        }
    }
}
