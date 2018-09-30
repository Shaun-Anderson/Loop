using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour {

    public bool isCollector;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        transform.parent.GetComponent<Player>().CollisionDetected(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Gem")
        {
            // Grab Gem
            if (!isCollector)
            {
                other.gameObject.GetComponent<Gem>().Break();
            }
            else
            {
                other.gameObject.GetComponent<Gem>().Pickup();
                transform.parent.GetComponent<Player>().PickUpCollisionDetected();
            }
        }
    }
}
