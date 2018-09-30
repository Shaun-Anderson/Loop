using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIAnimation { 

    public static IEnumerator SinglePulse(RectTransform transform, Vector3 vector3)
    {
        yield return GameManager.instance.StartCoroutine(UIAnimation.ChangeLocalScale(transform, vector3));
        yield return GameManager.instance.StartCoroutine(UIAnimation.ChangeLocalScale(transform, new Vector3(1,1,1)));
        yield break;
    }

    public static IEnumerator ChangeRectSize(RectTransform rect, Vector2 endSize, float speed)
    {
        while (true)
        {
            if (rect.sizeDelta != endSize)
            {
                rect.sizeDelta = Vector3.MoveTowards(rect.sizeDelta, endSize, speed * Time.deltaTime);
                yield return true;
            }
            else
            {
                rect.sizeDelta = endSize;
                yield break;
            }
        }
    }

    public static IEnumerator ChangeRectHeight(Rect rect, float endHeight, float speed)
    {
        while (true)
        {
            if (rect.height != endHeight)
            {
                Debug.Log(rect.height + " || " + endHeight);
                rect.height = Mathf.MoveTowards(rect.height, endHeight, speed * Time.deltaTime);
                yield return true;
            }
            else
            {
                yield break;
            }
        }
    }
    // Helper methods.
    static IEnumerator ChangeLocalScale(RectTransform card, Vector3 endScale) {
        while (true)
        {
            if (card.localScale != endScale)
            {
                card.localScale = Vector3.MoveTowards(card.localScale, endScale, 5 * Time.deltaTime);
                yield return true;
            }
            else
            {
                yield break;
            }
        }
    }

    public static IEnumerator ChangeLocalScale(Transform transform, Vector3 endScale, float speed)
    {
        while (true)
        {
            if (transform.localScale != endScale)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, endScale, speed * Time.deltaTime);
                yield return true;
            }
            else
            {
                yield break;
            }
        }
    }

    static IEnumerator Shake (Transform transform, float amount, float duration) {
        Vector3 origin = transform.position;
        while (duration > 0.01f)
        {
            transform.position = origin + Random.insideUnitSphere * amount;
            duration -= Time.deltaTime;
            yield return null;
        }
        transform.position = origin;
        yield break;
    }

    public static IEnumerator MoveTo (Transform transform, Vector3 endPos, float speed, Transform parent)
    {
        while (true)
        {
            if(Vector3.Distance(transform.position, endPos) > 1f ) {
                transform.position = Vector3.Lerp(transform.position, endPos, Time.deltaTime * speed);
                yield return true;
            }
            else {
                transform.position = endPos;
                transform.SetParent(parent);
                yield break;
            }
        }
    }

    public static IEnumerator MoveRect(RectTransform transform, Vector2 endPos, float speed)
    {
        while (true)
        {
            if (transform.anchoredPosition != endPos)
            {
                transform.anchoredPosition = Vector3.MoveTowards(transform.anchoredPosition, endPos, Time.deltaTime * speed);
                yield return true;
            }
            else
            {
                transform.anchoredPosition = endPos;
                yield break;
            }
        }
    }
    public static IEnumerator Move_FromBottom(RectTransform rect, float topAmount, float bottomAmount, bool lerp = true)
    {
        float height = GameManager.instance.canvas.rect.height;
        rect.offsetMax = new Vector2(0, -height);
        rect.offsetMin = new Vector2(0, -height);
        while (true)
        {
            if (Vector2.Distance(rect.offsetMax, new Vector2(rect.offsetMax.x, bottomAmount)) > 1)
            {
                if(lerp) {
                    rect.offsetMin = Vector2.Lerp(rect.offsetMin, new Vector2(rect.offsetMin.x, topAmount), Time.deltaTime * 3);
                    rect.offsetMax = Vector2.Lerp(rect.offsetMax, new Vector2(rect.offsetMax.x, bottomAmount), Time.deltaTime * 3);
                } else {
                    rect.offsetMin = Vector2.MoveTowards(rect.offsetMin, new Vector2(rect.offsetMin.x, topAmount), 50);
                    rect.offsetMax = Vector2.MoveTowards(rect.offsetMax, new Vector2(rect.offsetMax.x, bottomAmount), 50);
                }
                yield return true;
            }
            else
            {
                Debug.Log("honnnn");
                rect.offsetMin = new Vector2(rect.offsetMin.x, topAmount);
                rect.offsetMax = new Vector2(rect.offsetMax.x, bottomAmount);
                yield break;
            }
        }
    }

    public static IEnumerator Move_ToBottom(RectTransform rect, bool lerp = true)
    {
        float height = GameManager.instance.canvas.rect.height;
        Debug.Log(GameManager.instance.canvas.rect.height);
        while (true)
        {
            if (Vector2.Distance(rect.offsetMax, new Vector2(rect.offsetMax.x, -height)) > 1)
            {
                if(lerp) {
                    rect.offsetMin = Vector2.Lerp(rect.offsetMin, new Vector2(rect.offsetMin.x, -height), Time.deltaTime * 3);
                    rect.offsetMax = Vector2.Lerp(rect.offsetMax, new Vector2(rect.offsetMax.x, -height), Time.deltaTime * 3);
                } else {
                    rect.offsetMin = Vector2.MoveTowards(rect.offsetMin, new Vector2(rect.offsetMin.x, -height), 50);
                    rect.offsetMax = Vector2.MoveTowards(rect.offsetMax, new Vector2(rect.offsetMax.x, -height), 50);
                }

                yield return true;
            }
            else
            {
                rect.offsetMin = new Vector2(rect.offsetMin.x, -height);
                rect.offsetMax = new Vector2(rect.offsetMax.x, -height);
                yield break;
            }
        }
    }

    public static IEnumerator Fade (CanvasGroup element, float startAlpha, float toAlpha, float time)
    {
        element.alpha = startAlpha;

        while(true)
        {
            if(!Mathf.Approximately(element.alpha, toAlpha)) {
                element.alpha = Mathf.MoveTowards(element.alpha, toAlpha, time * Time.deltaTime);
                yield return true;
            } else {
                if(toAlpha <= 0)
                {
                }
                yield break;
            }
        }
    }



}
