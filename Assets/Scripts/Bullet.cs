using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public int NumberOfTurns = 5;
    private Ship.Weapons bulletType;
    private Rigidbody2D rb;
    private bool _leci = false;

    private Vector2 velocityVector;

    // Use this for initialization
    void Start () {
        rb = gameObject.GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = velocityVector;
    }

    public float tempAngle = 45f;
    public float tempPower = 2f;
    
	// Update is called once per frame
	void Update () { 
		
	}

    public void Shoot(Vector3 startPoint, float power, float angle, Ship.Weapons bulletType) 
    {
        angle += 90;
        if (_leci) return;
        _leci = true;
        this.bulletType = bulletType;
        transform.position = startPoint;
        velocityVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)) * power;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ship"))
        {
            collision.gameObject.GetComponent<Ship>().Die();
            Die();
        }
    }

    private void FixedUpdate()  
    {
        Debug.Log(Input.GetButtonDown("Fire1"));
        if (Input.GetButtonDown("Fire1"))
            Shoot(Vector3.zero, tempPower, tempAngle, Ship.Weapons.Blast);
        if (_leci)
        {
            var angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        if (CollideWithWater())
        {
            if (bulletType == Ship.Weapons.Storm)
            {
                // do storm
                Waver.Instance.AddFunc(transform.position.x, NumberOfTurns);
            }
            Die();
        }
    }

    private bool CollideWithWater()
    {
        return transform.position.y <= Waver.Instance.GetY(transform.position.x);
    }

    private void Die()
    {
        Destroy(gameObject);
        GameManager.Instance.NextPhase();
    }
}
