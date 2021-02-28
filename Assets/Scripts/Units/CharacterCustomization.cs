using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomization : MonoBehaviour
{
    [Header("Hairstyles")]
    public List<GameObject> hairStyles = new List<GameObject>();
    int currentHairstyle = 0;

    private void Update()
    {
        for (int i = 0; i < hairStyles.Count; i++)
        {
            if(i == currentHairstyle)
            {
                hairStyles[i].SetActive(true);
            }
            else
            {
                hairStyles[i].SetActive(false);
            }
        }
    }

    public void NextHairStyle()
    {
        if(currentHairstyle == hairStyles.Count - 1)
        {
            currentHairstyle = 0;
        }
        else
        {
            currentHairstyle++;
        }
    }

    public void PrevHairStyle()
    {
        if (currentHairstyle == hairStyles.Count - 1)
        {
            currentHairstyle = 0;
        }
        else
        {
            currentHairstyle--;
        }
    }
}
