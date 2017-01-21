using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waver : MonoBehaviour {

    public float MainAmplitude = 3f;
    public float Pindol200 = 45f;    //for 1 -> 1 sec is 1 degree
    public float DegreesPhase = 0f;
    public float Frequency = 10f; //1 in position is 10 in angle

    private float _currentTime = 0f;

    public static Waver Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Waver();
            }
            return _instance;
        }
    }

    private static Waver _instance;
    private Waver() { }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        _currentTime = Time.fixedTime;
    }

    public float GetY(float x)
    {
        return Mathf.Sin(Mathf.Deg2Rad * ((x + DegreesPhase)* Frequency) + Mathf.Deg2Rad * (_currentTime * Pindol200)) * MainAmplitude;
    }
}
