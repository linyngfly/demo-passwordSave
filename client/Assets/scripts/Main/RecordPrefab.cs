using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordPrefab : MonoBehaviour
{

    private static List<string> UsedKeys = new List<string>() { "名称", "账号", "密码", "备注" };

    public static bool IsUsedKey(string key)
    {
        return UsedKeys.Contains(key);
    }

    bool isDefault = true;
    public InputField keyInput;
    public InputField valueInput;
    public void Init(string key, string value)
    {
        keyInput.text = key;
        valueInput.text = value;
        if (!IsUsedKey(key))
        {
            keyInput.interactable = true;
            isDefault = false;
        } else
        {
            keyInput.interactable = false;
        }

    }


    public bool IsLegal()
    {
        if (isDefault)
        {
            return true;
        }
        string key = keyInput.text.Trim();
        if (key == "" || IsUsedKey(key))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void Btn_del()
    {
        if (isDefault)
        {
            valueInput.text = "";
            return;
        }
        if (valueInput.text != "")
        {
            valueInput.text = "";
        }
        else if (keyInput.text != "")
        {
            keyInput.text = "";
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
