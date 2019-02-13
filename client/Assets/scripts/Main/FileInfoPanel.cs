using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileInfoPanel : MonoBehaviour
{

    public static FileInfoPanel instance;

    FilePrefab filePrefab;
    public Transform fileInfoTileParent;
    public Transform fileInfoTilePrefab;

    private void Awake()
    {
        instance = this;
    }

    public void SetActive(bool isShow)
    {
        gameObject.SetActive(isShow);
        if (!isShow)
        {
            foreach (Transform trsm in fileInfoTileParent)
            {
                Destroy(trsm.gameObject);
            }
        }
    }

    public void Init(FilePrefab tmp)
    {
        SetActive(true);
        filePrefab = tmp;
        FileData fileData = tmp.attachedFileData;
        for (int i = 0; i < fileData.keys.Count; i++)
        {
            Transform trsm = Instantiate(fileInfoTilePrefab, fileInfoTileParent);
            trsm.GetComponent<FileInfoTilePrefab>().SetText(fileData.keys[i], fileData.values[i]);
        }
    }

    public void Btn_close()
    {
        SetActive(false);
    }

    public void Btn_edit()
    {
        Main.instance.GetFilePanel().Init(filePrefab);
        SetActive(false);
    }
}
