using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DirPrefab : MonoBehaviour
{
    public Sprite dirOpenImg;
    public Sprite dirCloseImg;
    [HideInInspector]
    public DirData attachedDirData = null;
    private bool nowIsOpen = false;

    /// <summary>
    /// 设置文件夹名字
    /// </summary>
    /// <param name="dirname"></param>
    public void SetDirName(string dirname)
    {
        transform.Find("dir/dirState/dirname").GetComponent<Text>().text = dirname;
    }

    /// <summary>
    /// 设置标题的位置
    /// </summary>
    public void SetPos(int xPos)
    {
        transform.Find("dir/dirState").GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, 0);
    }


    public void UpdateLayout()
    {
        StopAllCoroutines();
        StartCoroutine(UpdateLayout(GetComponent<RectTransform>()));
    }

    IEnumerator UpdateLayout(RectTransform rect)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        yield return new WaitForEndOfFrame();
        Vector3 vecScale = rect.localScale;
        float width = rect.rect.width;
        float height = rect.rect.height;
        while (rect.rect.width == 0)
        {
            Debug.Log(rect.rect.width);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnToggleClick(bool isOn)
    {
        if (isOn)
        {
            nowIsOpen = !nowIsOpen;
            attachedDirData.isFold = !nowIsOpen;
            transform.Find("dir/dirState").GetComponent<Image>().sprite = nowIsOpen ? dirOpenImg : dirCloseImg;
            transform.Find("files").gameObject.SetActive(nowIsOpen);
            if (nowIsOpen)
            {
                Transform fileParent = transform.Find("files");
                if (fileParent.childCount == 0)
                {
                    for (int i = 0; i < attachedDirData.dirs.Count; i++)
                    {
                        DirData oneDirData = attachedDirData.dirs[i];
                        Transform dirObj = Instantiate(Main.instance.dirPrefab, fileParent);
                        dirObj.GetComponent<Toggle>().group = Main.instance.baseToggleGroup;
                        oneDirData.attachedObj = dirObj;
                        oneDirData.parentDir = attachedDirData;
                        DirPrefab tmpDirPrefab = dirObj.GetComponent<DirPrefab>();
                        tmpDirPrefab.SetDirName(oneDirData.dirName);
                        tmpDirPrefab.SetPos(oneDirData.GetParentDirArr().Count * 100);
                        tmpDirPrefab.attachedDirData = oneDirData;
                    }
                    for (int i = 0; i < attachedDirData.files.Count; i++)
                    {
                        FileData oneFileData = attachedDirData.files[i];
                        Transform fileObj = Instantiate(Main.instance.filePrefab, fileParent);
                        fileObj.GetComponent<Toggle>().group = Main.instance.baseToggleGroup;
                        oneFileData.attachedObj = fileObj;
                        oneFileData.parentDir = attachedDirData;
                        fileObj.GetComponent<FilePrefab>().Init(oneFileData);
                    }
                }
            }
            Main.instance.ResetSize();
        }
    }

    public void Btn_edit()
    {
        Main.instance.GetDirPanel().Init(this);
    }

    public void Btn_up()
    {
        int myIndex = attachedDirData.GetDirIndex();
        if (myIndex == 0)
        {
            return;
        }
        DirData upData = attachedDirData.parentDir.dirs[myIndex - 1];
        attachedDirData.parentDir.dirs[myIndex - 1] = attachedDirData;
        attachedDirData.parentDir.dirs[myIndex] = upData;
        transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
        Main.instance.SaveData();
    }

    public void Btn_down()
    {
        int lastIndex = attachedDirData.parentDir.dirs.Count - 1;
        int myIndex = attachedDirData.GetDirIndex();
        if (myIndex == lastIndex)
        {
            return;
        }
        DirData downData = attachedDirData.parentDir.dirs[myIndex + 1];
        attachedDirData.parentDir.dirs[myIndex + 1] = attachedDirData;
        attachedDirData.parentDir.dirs[myIndex] = downData;
        downData.attachedObj.SetSiblingIndex(downData.attachedObj.GetSiblingIndex() - 1);
        Main.instance.SaveData();
    }

    public void Btn_del()
    {
        attachedDirData.parentDir.dirs.Remove(attachedDirData);
        Destroy(gameObject);
        Main.instance.SaveData();
        Main.instance.ResetSize();
    }

    public void Btn_save(string newName)
    {
        if (attachedDirData.dirName == newName)
        {
            return;
        }
        attachedDirData.dirName = newName;
        SetDirName(newName);
        Main.instance.SaveData();
    }

    public void Btn_newFile()
    {
        if (attachedDirData.isFold)
        {
            Toggle tmpToggle = GetComponent<Toggle>();
            if (!tmpToggle.isOn)
            {
                tmpToggle.isOn = true;
            }
            else
            {
                OnToggleClick(true);
            }
        }
        Transform parentTrsm = transform.Find("files");
        Transform fileObj = Instantiate(Main.instance.filePrefab, parentTrsm);
        fileObj.GetComponent<Toggle>().group = Main.instance.baseToggleGroup;
        FileData tmp = new FileData();
        attachedDirData.files.Add(tmp);
        tmp.parentDir = attachedDirData;
        tmp.keys.AddRange(new List<string> { "名称", "账号", "密码", "备注" });
        tmp.values.AddRange(new List<string> { "默认名", "", "", "" });
        tmp.attachedObj = fileObj;
        fileObj.GetComponent<FilePrefab>().Init(tmp);
        Main.instance.SaveData();
        Main.instance.ResetSize();
    }

    public void Btn_newDir()
    {
        int dirArrCount = attachedDirData.GetParentDirArr().Count;
        if (dirArrCount >= 4)
        {
            return;
        }
        if (attachedDirData.isFold)
        {
            Toggle tmpToggle = GetComponent<Toggle>();
            if (!tmpToggle.isOn)
            {
                tmpToggle.isOn = true;
            }
            else
            {
                OnToggleClick(true);
            }
        }
        Transform parentTrsm = transform.Find("files");
        Transform dirObj = Instantiate(Main.instance.dirPrefab, parentTrsm);
        dirObj.GetComponent<Toggle>().group = Main.instance.baseToggleGroup;
        DirData tmp = new DirData();
        attachedDirData.dirs.Add(tmp);
        tmp.parentDir = attachedDirData;
        tmp.dirName = "默认分组";
        tmp.attachedObj = dirObj;
        DirPrefab tmpDirPrefab = dirObj.GetComponent<DirPrefab>();
        tmpDirPrefab.SetDirName(tmp.dirName);
        tmpDirPrefab.SetPos((dirArrCount + 1) * 100);
        tmpDirPrefab.attachedDirData = tmp;
        dirObj.SetSiblingIndex(tmp.GetDirIndex());
        Main.instance.SaveData();
        Main.instance.ResetSize();
    }
}
