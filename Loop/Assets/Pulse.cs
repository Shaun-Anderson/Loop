using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour {

    public float maxScale;
    public float minScale;
    public float speed;
    private Vector3 orginalScale;

    private void Start()
    {
        orginalScale = transform.localScale;
    }
    // Update is called once per frame
    void Update () {
        float scale = Mathf.PingPong(speed * Time.time, maxScale - minScale) + minScale;
        transform.localScale = new Vector3(scale,scale,1);
	}

    public void Stop () {
        StartCoroutine(StopLerp());
        enabled = false;
    }

    private IEnumerator StopLerp ()
    {
        while (true)
        {
            if(transform.localScale != orginalScale)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, orginalScale, 1 * Time.deltaTime);
                yield return true;
            }
            else
            {
                enabled = false;
                yield break;
            }
        }
    }
}
