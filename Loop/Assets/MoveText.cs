using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveText : MonoBehaviour {

	// Use this for initialization
	void Kill () {
        transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(transform.localScale.x, 0, transform.localScale.z), 3 * Time.deltaTime);
        Destroy(gameObject, 5);
	}
	
	// Update is called once per frame
	void Update () {
        if(GameManager.instance.gameStarted)
        {
            Invoke("Kill", 1);
        }
	}
}
