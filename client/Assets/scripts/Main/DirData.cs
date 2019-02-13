using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DirData
{
    public string dirName = "";
    public List<FileData> files = new List<FileData>();
    public List<DirData> dirs = new List<DirData>();

    [System.NonSerialized]
    public DirData parentDir = null;
    [System.NonSerialized]
    public Transform attachedObj = null;
    [System.NonSerialized]
    public bool isFold = true;

    public int GetDirIndex()
    {
        return parentDir.dirs.IndexOf(this);
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
