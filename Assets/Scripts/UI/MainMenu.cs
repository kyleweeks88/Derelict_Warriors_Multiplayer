using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] MyNetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] GameObject landingPagePanel = null;

    public void HostLobby()
    {
        networkManager.StartHost();

        landingPagePanel.SetActive(false);
    }

    public void StartServerOnlyy()
    {
        networkManager.StartServer();

        landingPagePanel.SetActive(false);
    }

    public void StartSinglePlayer()
    {
        SceneManager.LoadScene("Scene_OfflineTest");
    }
}
