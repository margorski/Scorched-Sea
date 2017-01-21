using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {

    public AudioClip[] hitSounds;
    public AudioClip[] explosionSounds;

    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = gameObject.GetComponent<AudioSource>();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayExplosion()
    {
        //audioSource.PlayOneShot(explosionSounds[Random.Range(0, explosionSounds.Length)]);
    }

    public void PlayHit()
    {
        //audioSource.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length)]);
    }
}
