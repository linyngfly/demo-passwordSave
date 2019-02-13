using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilePrefab : MonoBehaviour
{

    [HideInInspector]
    public FileData attachedFileData = null;

    public void Init(FileData tmp)
    {
        attachedFileData = tmp;
        SetFileName();
        SetPos();
    }

    public void SetFileName()
    {
        transform.Find("Text").GetComponent<Text>().text = attachedFileData.values[0];
    }

    /// <summary>
    /// 设置标题的位置
    /// </summary>
    void SetPos()
    {
        transform.Find("Text").GetComponent<RectTransform>().anchoredPosition = new Vector2(attachedFileData.GetParentDirArr().Count * 100 + 10, 0);
    }


    public void OnToggleClick(bool isOn)
    {
        if (isOn)
        {
            Main.instance.GetFileInfoPanel().Init(this);
        }
    }

    public void Btn_up()
    {
        int myIndex = attachedFileData.GetFileIndex();
        if (myIndex == 0)
        {
            return;
        }
        FileData upData = attachedFileData.parentDir.files[myIndex - 1];
        attachedFileData.parentDir.files[myIndex - 1] = attachedFileData;
        attachedFileData.parentDir.files[myIndex] = upData;
        transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
        Main.instance.SaveData();
    }

    public void Btn_down()
    {
        int lastIndex = attachedFileData.parentDir.files.Count - 1;
        int myIndex = attachedFileData.GetFileIndex();
        if (myIndex == lastIndex)
        {
            return;
        }
        FileData downData = attachedFileData.parentDir.files[myIndex + 1];
        attachedFileData.parentDir.files[myIndex + 1] = attachedFileData;
        attachedFileData.parentDir.files[myIndex] = downData;
        downData.attachedObj.SetSiblingIndex(downData.attachedObj.GetSiblingIndex() - 1);
        Main.instance.SaveData();
    }

    public void Btn_del()
    {
        attachedFileData.parentDir.files.Remove(attachedFileData);
        Destroy(gameObject);
        Main.instance.SaveData();
        Main.instance.ResetSize();
    }

    public void Btn_save(List<string> keys, List<string> values)
    {
        attachedFileData.keys = keys;
        attachedFileData.values = values;
        SetFileName();
        Main.instance.SaveData();
    }
}
