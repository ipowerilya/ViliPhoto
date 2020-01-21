using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;


public class nexScene : MonoBehaviour
{
    public GameObject helpPls;
    public GameObject Menu;
    public GameObject loading;

    public void ARScene()
    {
        SceneManager.LoadScene("AR");
    }
    public void MenuScene()
    {
        SceneManager.LoadScene("Menu");
    }
    public void QRScene()
    {
        SceneManager.LoadScene("QR");
    }
    public void Off()
    {
        Application.Quit();
    }
    public void Help()
    {
        helpPls.gameObject.SetActive(true);
        Menu.gameObject.SetActive(false);
    }
    public void BackToMenu()
    {
        helpPls.gameObject.SetActive(false);
        Menu.gameObject.SetActive(true);
    }
    public void HowTo()
    {
        SceneManager.LoadScene("HowTo");
    }
    public void OnClickRateUs()
    {
        Application.OpenURL("market://details?id=" + Application.identifier);
    }
    public void VK()
    {
        Application.OpenURL("https://vk.com/viliargroup");
    }
    public void Telega()
    {
        Application.OpenURL("https://t.me/viliphoto");
    }
    public void WhatsApp()
    {
        Application.OpenURL("https://api.whatsapp.com/send?phone=+79068373050");
    }
    public void Youtube()
    {
        Application.OpenURL("https://www.youtube.com/channel/UCI2CajpqcsjV8iGgUoljDJw?view_as = subscriber");
    }
    public void Site()
    {
        Application.OpenURL("https://www.viliphoto.ru");
    }

    public void IDontNeedMoney()
    {
        Application.OpenURL("https://www.viliphoto.ru/ar");
    }
    public void Inst()
    {
        Application.OpenURL("https://www.instagram.com/viliar.group/");
    }
    public void allowLoading()
    {
        loading.GetComponent<MainRuntimeBehaviour>().isDecoding1 = false;
        loading.GetComponent<MainRuntimeBehaviour>().checkloading = false;
    }

}



