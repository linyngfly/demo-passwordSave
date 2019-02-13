using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LocalPanel : MonoBehaviour
{

    public InputField accountInput;
    public InputField passwordInput;
    public Text wrongInfoText;

    private void OnEnable()
    {
        string account = PlayerPrefs.GetString("lastAccount_local");
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

        string localpassword = PlayerPrefs.GetString(PlayerData.GetLocalPasswordKey(account));
        if (localpassword == "") // 不存在
        {
            SetWrongInfo("账号不存在");
            return;
        }

        string nowEncryPassword = PlayerData.EncryPassword(password);
        if (nowEncryPassword != localpassword)
        {
            SetWrongInfo("密码错误");
            return;
        }

        PlayerData.isLocal = true;
        PlayerPrefs.SetString("lastAccount_local", account);
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

        if (PlayerPrefs.GetString(PlayerData.GetLocalPasswordKey(account)) != "") // 存在
        {
            SetWrongInfo("账号已存在");
            return;
        }

        PlayerPrefs.SetString("lastAccount_local", account);
        string str = PlayerData.EncryPassword(password);
        PlayerPrefs.SetString(PlayerData.GetLocalPasswordKey(account), PlayerData.EncryPassword(password));
        SetWrongInfo("注册成功");
        passwordInput.text = "";
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
}
