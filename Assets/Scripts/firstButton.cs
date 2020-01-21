using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class firstButton : MonoBehaviour
{
    public GameObject firstBtn;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("FirstBtn") == 0)
        {
            //First launch
            PlayerPrefs.SetInt("FirstBtn", 1);
            firstBtn.gameObject.SetActive(true);
            PlayerPrefs.Save();
        }
        else
        {
            //Load scene_02 if its not the first launch
            firstBtn.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
