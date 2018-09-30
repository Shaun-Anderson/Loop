using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour {

    public ParticleSystem emit;
    public SpriteRenderer spriteRenderer;
    public Material ogMaterial;
    public Material pickUpMaterial;
    private bool effected;

    public void Update()
    {

        if(!effected)
        {
            CheckPointIfVisible();
        }
    }

    public void Pickup () {
        GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(UIAnimation.ChangeLocalScale(this.transform, new Vector3(0, 0, 0), 1));
        //lightSprite.color = new Color(255, 255, 255, 0.1f);

        GameManager.instance.soundManager.PlayClip(GameManager.instance.soundManager.pickUpSound, 0.4f);

        spriteRenderer.material = pickUpMaterial;
        effected = true;
        Invoke("ReturnToPool", 5);
    }

    void CheckPointIfVisible()
    {
        Vector3 worldLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        if (transform.position.x <= worldLeft.x)
        {
            Invoke("ReturnToPool", 0);
        }
    }

    public void Break () {
        GetComponent<BoxCollider2D>().enabled = false;

        GameManager.instance.soundManager.PlayClip(GameManager.instance.soundManager.hitSound, 0.05f);
        emit.Play();

        GetComponent<SpriteRenderer>().enabled = false;
        effected = true;
        Invoke("ReturnToPool", 10);

    }

    public void ReturnToPool ()
    {
        StopAllCoroutines();
        transform.localScale = Vector3.one;
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;

        spriteRenderer.material = ogMaterial;

        transform.SetParent(GameManager.instance.UIManager.objectPoolParent);
        gameObject.SetActive(false);

    }
}
