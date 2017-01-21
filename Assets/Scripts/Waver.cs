﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waver : MonoBehaviour {

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

    public float GetY(float x)
    {
        return Mathf.Sin(Mathf.Deg2Rad * x * 20f);
    }
}
