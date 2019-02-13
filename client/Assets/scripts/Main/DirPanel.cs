using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirPanel : MonoBehaviour
{
    public static DirPanel instance = null;
    DirPrefab dirPrefab;
    void Awake()
    {
        instance = this;
    }

    void SetActive(bool isShow)
    {
        gameObject.SetActive(isShow);
    }

    public void Init(DirPrefab tmp)
    {
        dirPrefab = tmp;
        transform.Find("groupName").GetComponent<InputField>().text = tmp.attachedDirData.dirName;
        SetActive(true);
    }

    public void Btn_Close()
    {
        SetActive(false);
    }

    public void Btn_up()
    {
        SetActive(false);
        dirPrefab.Btn_up();
    }

    public void Btn_down()
    {
        SetActive(false);
        dirPrefab.Btn_down();
    }

    public void Btn_del()
    {
        transform.Find("delBtn2").gameObject.SetActive(true);
    }

    public void Btn_del_yes()
    {
        transform.Find("delBtn2").gameObject.SetActive(false);
        SetActive(false);
        dirPrefab.Btn_del();
    }

    public void Btn_del_cancel()
    {
        transform.Find("delBtn2").gameObject.SetActive(false);
    }

    public void Btn_save()
    {
        string tmpname = transform.Find("groupName").GetComponent<InputField>().text;
        tmpname = tmpname.Trim();
        if (tmpname.Length == 0)
        {
            SetActive(false);
            return;
        }
        SetActive(false);
        dirPrefab.Btn_save(tmpname);
    }

    public void Btn_newFile()
    {
        SetActive(false);
        dirPrefab.Btn_newFile();
    }

    public void Btn_newDir()
    {
        SetActive(false);
        dirPrefab.Btn_newDir();
    }
}
