using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{

    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("Scavenging");
    }
}
