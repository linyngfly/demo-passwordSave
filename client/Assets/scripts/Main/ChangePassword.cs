using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePassword : MonoBehaviour
{

    public InputField passwordInput1;
    public InputField passwordInput2;
    public InputField pinInput1;
    public InputField pinInput2;
    public Text infoText;

    public void Btn_password_to()
    {
        transform.Find("choose").gameObject.SetActive(false);
        transform.Find("changePassword").gameObject.SetActive(true);
        passwordInput1.text = "";
        passwordInput2.text = "";
    }

    public void Btn_pin_to()
    {
        transform.Find("choose").gameObject.SetActive(false);
        transform.Find("changePin").gameObject.SetActive(true);
        pinInput1.text = "";
        pinInput2.text = "";
    }

    public void Btn_password_back()
    {
        transform.Find("choose").gameObject.SetActive(true);
        transform.Find("changePassword").gameObject.SetActive(false);
        SetWrongInfo("");
    }

    public void Btn_pin_back()
    {
        transform.Find("choose").gameObject.SetActive(true);
        transform.Find("changePin").gameObject.SetActive(false);
        SetWrongInfo("");
    }


    public void Btn_changePassword_yes()
    {
        string password1 = passwordInput1.text;
        if (password1 == PlayerData.GetPassword())
        {
            SetWrongInfo("不可与旧密码相同");
            return;
        }
        if (password1.Contains(" "))
        {
            SetWrongInfo("密码不可包含空格");
            return;
        }
        if (password1.Length < 4 || password1.Length > 15)
        {
            SetWrongInfo("密码长度为4-15");
            return;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(password1, @"^[\dA-Za-z]{4,15}$"))
        {
            SetWrongInfo("密码必须为英文字母或数字");
            return;
        }
        if (password1 != passwordInput2.text)
        {
            SetWrongInfo("两次密码不一致");
            return;
        }
        Main.instance.ChangePasswordYes(password1);
        Btn_close();
    }

    public void Btn_changePin_yes()
    {
        string pin1 = pinInput1.text;
        if (pin1 == PlayerData.PIN)
        {
            SetWrongInfo("不可与旧PIN码相同");
            return;
        }
        if (pin1.Contains(" "))
        {
            SetWrongInfo("PIN码不可包含空格");
            return;
        }
        if (pin1.Length < 4 || pin1.Length > 15)
        {
            SetWrongInfo("PIN码长度为4-15");
            return;
        }
        char[] pin1InputTextArr = pin1.ToCharArray();
        for (int i = 0; i < pin1InputTextArr.Length; i++)
        {
            if (!char.IsDigit(pin1InputTextArr[i]))
            {
                SetWrongInfo("PIN码必须为数字");
                return;
            }
        }
        if (pin1 != pinInput2.text)
        {
            SetWrongInfo("两次PIN码不一致");
            return;
        }
        string pinKey = PlayerData.GetPinKey();
        PlayerPrefs.SetString(pinKey, pin1);
        PlayerData.PIN = pin1;
        Btn_close();
    }

    void SetWrongInfo(string info)
    {
        StopCoroutine("InfoHide");
        infoText.text = info;
        StartCoroutine("InfoHide");
    }

    IEnumerator InfoHide()
    {
        yield return new WaitForSeconds(2f);
        infoText.text = "";
    }

    public void Btn_close()
    {
        Destroy(gameObject);
    }
}
