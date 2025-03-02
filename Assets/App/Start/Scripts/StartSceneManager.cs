using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;
    private AudioSource audioSource; 

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnStartButtonClicked()
    {
        audioSource.PlayOneShot(clickSound);
        SceneManager.LoadScene("Scavenging");
    }
}
