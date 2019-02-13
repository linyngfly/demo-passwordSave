using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileInfoTilePrefab : MonoBehaviour
{

    public void SetText(string key, string value)
    {
        Text infoText = transform.Find("Text").GetComponent<Text>();
        infoText.text = "<color=red><b>" + key + "：</b></color>" + value;
        StartCoroutine(GetH());
    }

    IEnumerator GetH()
    {
        yield return new WaitForEndOfFrame();
        RectTransform infoText = transform.Find("Text").GetComponent<RectTransform>();
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, infoText.sizeDelta.y + 20);
    }
}
