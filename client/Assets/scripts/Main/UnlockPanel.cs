using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockPanel : MonoBehaviour
{
    public static UnlockPanel instance;
    private void Awake()
    {
        instance = this;
    }

    public Text numText;
    string inputText = "";

    public void Btn_Number(int num)
    {
        if (inputText.Length >= 10)
        {
            return;
        }
        inputText += num;
        numText.text += "*";
        if (inputText == PlayerData.PIN)
        {
            Destroy(gameObject);
        }
    }

    public void Btn_exit()
    {
        Application.Quit();
    }

    public void Btn_backspace()
    {
        if(inputText.Length == 0)
        {
            return;
        }
        inputText = inputText.Substring(0, inputText.Length - 1);
        numText.text = numText.text.Substring(0, numText.text.Length - 1);
    }
}
