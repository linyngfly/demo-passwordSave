using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnlinePanel : MonoBehaviour
{

    public InputField accountInput;
    public InputField passwordInput;
    public GameObject httpWaitPanel;
    public Text wrongInfoText;
    string loginIp = "";

    private void Start()
    {
        if (LoginMain.instance.isLocal)
        {
            loginIp = "http://127.0.0.1:5001";
        }
        else
        {
            loginIp = "http://47.105.51.196:5001";
        }
    }

    private void OnEnable()
    {
        string account = PlayerPrefs.GetString("lastAccount_online");
        if (account == "")
        {
            accountInput.text = "";
        }
        else
        {
            accountInput.text = account;
        }
        passwordInput.text = "";
    }

    public void Btn_login()
    {
        string account = accountInput.text;
        if (account.Contains(" "))
        {
            SetWrongInfo("账号不可包含空格");
            return;
        }
        if (account.Length < 3 || account.Length > 10)
        {
            SetWrongInfo("账号长度为3-10");
            return;
        }
        string password = passwordInput.text;
        if (password.Contains(" "))
        {
            SetWrongInfo("密码不可包含空格");
            return;
        }
        if (password.Length < 4 || password.Length > 15)
        {
            SetWrongInfo("密码长度为4-15");
            return;
        }
        if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"^[\dA-Za-z]{4,15}$"))
        {
            SetWrongInfo("密码必须为英文字母或数字");
            return;
        }
        WWWForm form = new WWWForm();
        form.AddField("method", "login");
        form.AddField("username", account);
        string encry_password = PlayerData.EncryPassword(password);
        form.AddField("password", encry_password);
        SetHttpPanelActive(true);
        StartCoroutine(LoginHttp(loginIp, form));
    }


    public void Btn_reg()
    {
        string account = accountInput.text;
        if (account.Contains(" "))
        {
            SetWrongInfo("账号不可包含空格");
            return;
        }
        if (account.Length < 3 || account.Length > 10)
        {
            SetWrongInfo("账号长度为3-10");
            return;
        }
        string password = passwordInput.text;
        if (password.Contains(" "))
        {
            SetWrongInfo("密码不可包含空格");
            return;
        }
        if (password.Length < 4 || password.Length > 15)
        {
            SetWrongInfo("密码长度为4-15");
            return;
        }
        if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"^[\dA-Za-z]{4,15}$"))
        {
            SetWrongInfo("密码必须为英文字母或数字");
            return;
        }

        WWWForm form = new WWWForm();
        form.AddField("method", "register");
        form.AddField("username", account);
        string encry_password = PlayerData.EncryPassword(password);
        form.AddField("password", encry_password);
        SetHttpPanelActive(true);
        StartCoroutine(RegHttp(loginIp, form));
    }

    void SetWrongInfo(string info)
    {
        StopCoroutine("ClearWrongInfo");
        wrongInfoText.text = info;
        StartCoroutine("ClearWrongInfo");
    }

    IEnumerator ClearWrongInfo()
    {
        yield return new WaitForSeconds(2f);
        wrongInfoText.text = "";
    }

    public void Btn_close()
    {
        Destroy(gameObject);
    }

    IEnumerator RegHttp(string _url, WWWForm _wForm)
    {
        UnityWebRequest www = UnityWebRequest.Post(_url, _wForm);
        www.timeout = 5;
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            SetWrongInfo("网络错误");
        }
        else
        {
            Http_reg_data tmp = JsonUtility.FromJson<Http_reg_data>(www.downloadHandler.text);
            if (tmp.code == 1)
            {
                SetWrongInfo("用户名已存在");
            }
            else if (tmp.code == 0)
            {
                SetWrongInfo("注册成功，请登录");
                passwordInput.text = "";
            }
            else
            {
                SetWrongInfo("网络错误");
            }
        }
        SetHttpPanelActive(false);
    }

    IEnumerator LoginHttp(string _url, WWWForm _wForm)
    {
        UnityWebRequest www = UnityWebRequest.Post(_url, _wForm);
        www.timeout = 5;
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            SetWrongInfo("网络错误");
        }
        else
        {

            Http_login_data tmp = JsonUtility.FromJson<Http_login_data>(www.downloadHandler.text);

            if (tmp.code == 1 || tmp.code == 2)
            {
                SetWrongInfo("用户或密码错误");
            }
            else if (tmp.code == 0)
            {
                string account = accountInput.text;
                string password = passwordInput.text;
                PlayerData.isLocal = false;
                PlayerPrefs.SetString("lastAccount_online", account);
                PlayerData.uid = tmp.uid;
                PlayerData.host = tmp.host;
                PlayerData.port = tmp.port;
                PlayerData.token = tmp.token;
                PlayerData.account = account;
                PlayerData.SetPassword(password);
                string pinKey = PlayerData.GetPinKey();
                string pin = PlayerPrefs.GetString(pinKey);
                if (pin == "")
                {
                    PlayerData.PIN = PlayerData.Default_Pin;
                }
                else
                {
                    PlayerData.PIN = pin;
                }
                SceneManager.LoadScene("main");
            }
            else
            {
                SetWrongInfo("网络错误");
            }
        }
        SetHttpPanelActive(false);
    }

    void SetHttpPanelActive(bool isShow)
    {
        httpWaitPanel.SetActive(isShow);
    }

    class Http_reg_data
    {
        public int code = 0;
    }

    class Http_login_data
    {
        public int code = 0;
        public int uid = 0;
        public string host = "";
        public int port = 0;
        public int token = 0;
    }
}
