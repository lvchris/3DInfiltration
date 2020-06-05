using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Restart"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
