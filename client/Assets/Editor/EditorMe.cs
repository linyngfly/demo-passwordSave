using UnityEngine;
using UnityEditor;

public class MenuItems
{
    [MenuItem("Ahuang/ClearPlayerPrefs")]
    private static void NewMenuOption()
    {
        PlayerPrefs.DeleteAll();
    }
}