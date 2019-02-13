using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FileData
{
    public List<string> keys = new List<string>();
    public List<string> values = new List<string>();


    [System.NonSerialized]
    public DirData parentDir = null;
    [System.NonSerialized]
    public Transform attachedObj = null;

    public int GetFileIndex()
    {
        return parentDir.files.IndexOf(this);
    }

    public List<int> GetParentDirArr()
    {
        List<int> indexs = new List<int>();
        DirData tmp = parentDir;
        while (tmp.parentDir != null)
        {
            int index = tmp.GetDirIndex();
            indexs.Insert(0, index);
            tmp = tmp.parentDir;
        }
        return indexs;
    }
}
