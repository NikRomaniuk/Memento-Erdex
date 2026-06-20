using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CanvasLayoutFixer : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(FixLayoutRoutine());
    }

    private IEnumerator FixLayoutRoutine()
    {
        yield return new WaitForEndOfFrame();

        RectTransform rectTransform = GetComponent<RectTransform>();
        
        if (rectTransform != null)
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }
}