using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {

    public AudioClip[] hitSounds;
    public AudioClip[] explosionSounds;
    public AudioClip chargingSound;
    public AudioClip shootSound;
    public AudioClip aimSound;
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
        audioSource.PlayOneShot(explosionSounds[Random.Range(0, explosionSounds.Length)]);
    }

    public void PlayHit()
    {
        audioSource.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length)]);
    }

    public void StartCharging()
    {
        audioSource.PlayOneShot(chargingSound);
    }

    public void Stop()
    {
        audioSource.loop = false;
        audioSource.pitch = 1.0f;
        audioSource.volume = 1.0f;
        audioSource.Stop();
    }

    public void PlayShoot()
    {
        audioSource.PlayOneShot(shootSound);
    }

    public void PlayAim()
    {
        audioSource.loop = true;
        audioSource.clip = aimSound;
        audioSource.pitch = -0.1f;
        audioSource.volume = 0.3f;
        audioSource.Play();
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }

    public void MovePitch(float deltaAngle)
    {
        if (deltaAngle == 0.0f)
        {
            audioSource.pitch = -0.1f;
            return;
        }
        if (audioSource.pitch <= 0.0f)
        {
            audioSource.pitch = 1f;
        }
        else
        {
            audioSource.pitch += deltaAngle * 0.005f;
        }
    }
}
