using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HealthDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStats myStats = null;

    [Header("Health Ref")]
    [SerializeField] private Image healthBarImage = null;

    [Header("Stamina Ref")]
    [SerializeField] Image staminaBarImage = null;


    private void OnEnable()
    {
        myStats.Event_StatChanged += HandleStatChanged;
    }

    private void OnDisable()
    {
        myStats.Event_StatChanged -= HandleStatChanged;

    }

    void HandleStatChanged(string key, float currentValue, float maxValue)
    {
        if(key == "Health")
        {
            healthBarImage.fillAmount = currentValue / maxValue;
            //Debug.Log(stat.GetCurrentValue()+" / "+ stat.GetMaxValue());
        }

        if(key == "Stamina")
        {
            staminaBarImage.fillAmount = currentValue / maxValue;
        }
    }
}
