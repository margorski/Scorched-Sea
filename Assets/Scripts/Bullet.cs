using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public int NumberOfTurns = 5;
    private Ship.Weapons bulletType;
    private Rigidbody2D rb;
    private bool _leci = false;
    private BoxCollider2D collider;
    private Vector2 velocityVector;
    public float colliderTimer = 0.5f;
    // Use this for initialization
    void Start () {
        rb = gameObject.GetComponent<Rigidbody2D>();
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = velocityVector;
        collider = gameObject.GetComponent<BoxCollider2D>();
        collider.enabled = false;
    }

    public float tempAngle = 45f;
    public float tempPower = 2f;
    
	// Update is called once per frame
	void Update () { 
		
	}


    public void OnBecameInvisible()
    {
        if (gameObject.transform.position.x >= -9.5 && gameObject.transform.position.x <= 9.5)
            return;
        Die();
    }

    public void Shoot(float power, float angle, Ship.Weapons bulletType) 
    {
        angle += 90;
        if (_leci) return;
        _leci = true;
        this.bulletType = bulletType;
        velocityVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)) * power;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ship") && bulletType == Ship.Weapons.Blast)
        {
            collision.gameObject.GetComponent<Ship>().Die();
            Die();
        }
    }

    private void FixedUpdate()  
    {

        rb.AddForce(new Vector2(GameManager.Instance.WindForce, 0.0f));

        if (colliderTimer > 0.0f)
        {
            colliderTimer -= Time.fixedDeltaTime;
            if (colliderTimer <= 0.0f)
                collider.enabled = true;
        }
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
        return collider.enabled && transform.position.y <= Waver.Instance.GetY(transform.position.x);
    }

    private void Die()
    {
        Destroy(gameObject);
        GameManager.Instance.NextPhase();
    }
}
