using UnityEngine;

public class DeletePlayerPrefs : MonoBehaviour
{
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
