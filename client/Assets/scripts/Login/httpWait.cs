using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class httpWait : MonoBehaviour {
    public Transform rotateTrsm;
    public CanvasGroup group;
    private void OnEnable()
    {
        group.alpha = 0;
        StartCoroutine(Show());
    }

    IEnumerator Show()
    {
        yield return new WaitForSeconds(1.5f);
        group.alpha = 1;
    }

    // Update is called once per frame
    void Update () {
        rotateTrsm.Rotate(0, 0, 120 * Time.deltaTime);
	}
}
