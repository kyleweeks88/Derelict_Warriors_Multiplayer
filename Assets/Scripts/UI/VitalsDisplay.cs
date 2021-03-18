using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class VitalsDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] VitalsManager vitalsMgmt = null;

    [Header("Health Ref")]
    [SerializeField] private Image healthBarImage = null;

    [Header("Stamina Ref")]
    [SerializeField] Image staminaBarImage = null;


    private void OnEnable()
    {
        vitalsMgmt.health.Event_ValueChanged += HandleHealthChanged;
        vitalsMgmt.stamina.Event_ValueChanged += HandleStaminaChanged;
    }

    private void OnDisable()
    {
        vitalsMgmt.health.Event_ValueChanged -= HandleHealthChanged;
        vitalsMgmt.stamina.Event_ValueChanged -= HandleStaminaChanged;
    }

    void HandleHealthChanged(float curVal, float maxVal)
    {
        healthBarImage.fillAmount = curVal / maxVal;
    }

    void HandleStaminaChanged(float curVal, float maxVal)
    {
        staminaBarImage.fillAmount = curVal / maxVal;
    }
}
