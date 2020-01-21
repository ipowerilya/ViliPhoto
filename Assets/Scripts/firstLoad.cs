using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class firstLoad : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        if (PlayerPrefs.GetInt("FirstLaunch") == 0)
        {
            //First launch
            PlayerPrefs.SetInt("FirstLaunch", 1);
            SceneManager.LoadScene("HowTo");
            PlayerPrefs.Save();
        }
        else
        {
            //Load scene_02 if its not the first launch
            SceneManager.LoadScene("AR");
        }
    }

}
