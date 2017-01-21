using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waver : MonoBehaviour {



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float GetY(float x)
    {
        return Mathf.Sin(Mathf.Deg2Rad * x);
    }
}
