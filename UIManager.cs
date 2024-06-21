using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class UIManager : MonoBehaviour
{
    public float fadeTime = 1f;
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;
    public Ease ease;
    public List<GameObject> Guns = new List<GameObject>();
    public List<GameObject> items = new List<GameObject>();
    private bool fading;
    public bool firstItemFix;
    public void PanelFadeIn()
    {
        if (fading) return;
        fading = true;
        rectTransform.gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        rectTransform.transform.localScale = new Vector3 (0.8f, 0.8f, 1f);
        rectTransform.DOScale(new Vector3(1f, 1f, 1f), fadeTime).SetEase(ease);
        canvasGroup.DOFade(1, fadeTime);
        StartCoroutine(ItemsAnimation());
        StartCoroutine(OtherAnimation());
    }
    public void  PanelFadeOut()
    {
        if (fading) return;
        fading = true;
        canvasGroup.alpha = 1f;
        rectTransform.transform.localScale = new Vector3(1f, 1f, 1f);
        rectTransform.DOScale(new Vector3(0.8f, 0.8f, 1f), fadeTime).SetEase(ease);
        canvasGroup.DOFade(0, fadeTime);
        StartCoroutine(DisableAfterFade(fadeTime));
    }
    IEnumerator DisableAfterFade(float time)
    {
        yield return new WaitForSeconds(time);
        rectTransform.gameObject.SetActive(false);

        Vector2 LocalPosG = Guns[0].transform.localPosition;
        Guns[0].transform.localPosition = new Vector2(LocalPosG.x, LocalPosG.y - 100f);

        fading = false;
    }

    IEnumerator ItemsAnimation()
    {
        foreach (var item in Guns)
        {
            item.GetComponent<CanvasGroup>().alpha = 0f;
        }

        foreach (var item in Guns)
        {
            Vector2 LocalPos = item.transform.localPosition;
            item.transform.DOLocalMove(new Vector2(LocalPos.x, LocalPos.y +100f), fadeTime);
            item.GetComponent<CanvasGroup>().DOFade(1, fadeTime);
            yield return new WaitForSeconds(0.15f);
        }
        fading = false;
    }

    IEnumerator OtherAnimation()
    {
        if(firstItemFix)
        {
            Vector2 pos = items[0].transform.localPosition;
            items[0].transform.localPosition = new Vector2(pos.x +100,pos.y);
            firstItemFix = false;
        }
        foreach (var item in items)
        {
            item.GetComponent<CanvasGroup>().alpha = 0f;
            Vector2 LocalPosI = item.transform.localPosition;
            item.transform.localPosition = new Vector2(LocalPosI.x - 100, LocalPosI.y);

        }

        foreach (var item in items)
        {
            Vector2 LocalPos = item.transform.localPosition;
            item.transform.DOLocalMove(new Vector2(LocalPos.x+100, LocalPos.y), fadeTime);
            item.GetComponent<CanvasGroup>().DOFade(1, fadeTime);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
