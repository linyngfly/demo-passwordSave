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

    private void Awake()
    {
        instance = this;
        infoText = transform.Find("Text").GetComponent<Text>();
        rotateTrsm = transform.Find("Image");
        group = GetComponent<CanvasGroup>();
    }

    public void Show(string info)
    {
        StopAllCoroutines();
        group.alpha = 0;
        infoText.text = info;
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        StartCoroutine(ShowImg());
    }

    IEnumerator ShowImg()
    {
        yield return new WaitForSeconds(1.5f);
        group.alpha = 1;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        rotateTrsm.Rotate(0, 0, 120 * Time.deltaTime);
    }
}
