using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    private void Start()
    {
        //FindObjectOfType<ObjectManager>().enabled = false;
        //Time.timeScale = 0;
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartTime()
    {
        FindObjectOfType<ObjectManager>().enabled = true;

        Time.timeScale = 1;
    }
}
