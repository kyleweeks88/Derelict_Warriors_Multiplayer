using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class SetNameCanvas : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInput;

    string lastValue = string.Empty;

    private void Update()
    {
        CheckSetName();
    }

    void CheckSetName()
    {
        if(!NetworkClient.active) { return; }

        ClientInstance ci = ClientInstance.ReturnClientInstance();
        if (ci == null) { return; }
        
        if(nameInput.text != lastValue)
        {
            lastValue = nameInput.text;
            ci.SetName(nameInput.text);
        }
    }
}
