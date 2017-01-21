using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionRay : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Ship"))
        {
            collider.gameObject.GetComponent<Ship>().Die();
            GameManager.Instance.SoundPlayer.PlayExplosion();
        }
    }
}
