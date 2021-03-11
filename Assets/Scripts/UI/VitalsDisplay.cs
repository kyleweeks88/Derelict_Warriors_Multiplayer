using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class VitalsDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] HealthManager healthManager = null;
    [SerializeField] StaminaManager staminaManager = null;

    [Header("Health Ref")]
    [SerializeField] private Image healthBarImage = null;

    [Header("Stamina Ref")]
    [SerializeField] Image staminaBarImage = null;


    private void OnEnable()
    {
        healthManager.Event_HealthChanged += HandleHealthChanged;
        //staminaManager.Event_StaminaChanged += HandleStaminaChanged;
    }

    private void OnDisable()
    {
        healthManager.Event_HealthChanged -= HandleHealthChanged;
        //staminaManager.Event_StaminaChanged -= HandleStaminaChanged;
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
