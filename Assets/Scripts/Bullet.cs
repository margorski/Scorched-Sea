using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    private Ship.Weapons bulletType;
    private Rigidbody2D rb;
	// Use this for initialization
	void Start () {
        rb = transform.FindChild("Tip").gameObject.GetComponent<Rigidbody2D>();
        rb.centerOfMass = new Vector2(0.2f, 0.0f);
	}

    public float tempAngle = 45;
    public float tempPower = 100.0f;
    
	// Update is called once per frame
	void Update () { 
		
	}

    public void Shoot(Vector3 startPoint, float power, float angle, Ship.Weapons bulletType) 
    {
        this.bulletType = bulletType;
        transform.position = startPoint;
        transform.Rotate(new Vector3(0.0f, 0.0f, 45));
        var normalizedForce = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        rb.velocity = normalizedForce * power;  
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void FixedUpdate()  
    {
        Debug.Log(Input.GetButtonDown("Fire1"));
        if (Input.GetButtonDown("Fire1"))
            Shoot(Vector3.zero, tempPower, tempAngle, Ship.Weapons.Blast);
    }
}
