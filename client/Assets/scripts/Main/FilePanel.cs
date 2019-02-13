using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilePanel : MonoBehaviour
{

    public static FilePanel instance;

    FilePrefab filePrefab;
    public Transform recordParent;
    public Transform recordPrefab;
    public Transform beizhuPrefab;

    private void Awake()
    {
        instance = this;
    }

    public void Init(FilePrefab tmp)
    {
        SetActive(true);
        filePrefab = tmp;
        FileData fileData = tmp.attachedFileData;
        for (int i = 0; i < fileData.keys.Count - 1; i++)
        {
            Instantiate(recordPrefab, recordParent).GetComponent<RecordPrefab>().Init(fileData.keys[i], fileData.values[i]);
        }
        Instantiate(beizhuPrefab, recordParent).GetComponent<RecordPrefab>().Init(fileData.keys[fileData.keys.Count - 1], fileData.values[fileData.keys.Count - 1]);
    }

    void SetActive(bool isShow)
    {
        gameObject.SetActive(isShow);
        if (!isShow)
        {
            foreach (Transform trsm in recordParent)
            {
                Destroy(trsm.gameObject);
            }
        }
    }

    public void Btn_close()
    {
        SetActive(false);
    }

    public void Btn_up()
    {
        SetActive(false);
        filePrefab.Btn_up();
    }

    public void Btn_down()
    {
        SetActive(false);
        filePrefab.Btn_down();
    }

    public void Btn_del()
    {
        transform.Find("btn_del2").gameObject.SetActive(true);
    }

    public void Btn_del_yes()
    {
        transform.Find("btn_del2").gameObject.SetActive(false);
        SetActive(false);
        filePrefab.Btn_del();
    }

    public void Btn_del_cancel()
    {
        transform.Find("btn_del2").gameObject.SetActive(false);
    }

    public void Btn_add()
    {
        Transform trsm = Instantiate(recordPrefab, recordParent);
        trsm.SetSiblingIndex(recordParent.childCount - 2);
        trsm.GetComponent<RecordPrefab>().Init("", "");
    }

    public void Btn_save()
    {

        List<string> keys = new List<string>();
        List<string> values = new List<string>();
        foreach (Transform trsm in recordParent)
        {
            RecordPrefab tmp = trsm.GetComponent<RecordPrefab>();
            if (!tmp.IsLegal())
            {
                continue;
            }
            keys.Add(tmp.keyInput.text.Trim());
            values.Add(tmp.valueInput.text.Trim());
        }

        SetActive(false);
        filePrefab.Btn_save(keys, values);
    }

}
