using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour {
   
    public void Update()
    {
        CheckPointIfVisible();
    }

    void CheckPointIfVisible()
    {
        Vector3 worldLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        if (transform.position.x <= worldLeft.x)
        {
            Invoke("ReturnToPool", 0);
        }
    }

    public void ReturnToPool()
    {
        transform.SetParent(GameManager.instance.UIManager.objectPoolParent);
        gameObject.SetActive(false);

    }
}
