using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginMain : MonoBehaviour
{
    public static LoginMain instance;
    public bool isLocal = true;
    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        Application.targetFrameRate = 30;
    }

    public void Btn_local()
    {
        ShowLoginPanel("login/localPanel");
    }

    public void Btn_online()
    {
        ShowLoginPanel("login/onlinePanel");
    }

    public void Btn_info()
    {
        ShowLoginPanel("login/infoPanel");
    }


    void ShowLoginPanel(string path)
    {
        Object obj = Resources.Load(path);
        if (obj)
        {
            Instantiate(obj, transform);
        }
    }

    public void Btn_exit()
    {
        Application.Quit();
    }
}
