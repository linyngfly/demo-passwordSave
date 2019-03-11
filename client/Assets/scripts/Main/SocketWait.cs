using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SocketWait : MonoBehaviour
{
    public static SocketWait instance;

    Text infoText;
    Transform rotateTrsm;
    CanvasGroup group;
    float nowTime = 0;

    private void Awake()
    {
        instance = this;
        infoText = transform.Find("Text").GetComponent<Text>();
        rotateTrsm = transform.Find("Image");
        group = GetComponent<CanvasGroup>();
    }

    public void Show(string info)
    {
        infoText.text = info;
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }


    public void Hide()
    {
        group.alpha = 0;
        nowTime = 0;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        rotateTrsm.Rotate(0, 0, 120 * Time.deltaTime);
        nowTime += Time.deltaTime;
        if (nowTime > 1f && group.alpha == 0)
        {
            group.alpha = 1;
        }
    }
}
