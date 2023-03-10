using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public int NumberOfTurns = 5;
    private Ship.Weapons bulletType;
    private Rigidbody2D rb;
    private bool _leci = false;
    private BoxCollider2D _collider;
    private Vector2 velocityVector;
    public float colliderTimer = 0.5f;
    public GameObject ExplosionPrefab;
    // Use this for initialization
    void Start () {
        rb = gameObject.GetComponent<Rigidbody2D>();
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = velocityVector;
        _collider = gameObject.GetComponent<BoxCollider2D>();
        _collider.enabled = false;
    }
        

    // Update is called once per frame
    void Update () { 
		
	}


    public void OnBecameInvisible()
    {
        if (gameObject.transform.position.x >= -9.5 && gameObject.transform.position.x <= 9.5)
            return;
        GameManager.Instance.SoundPlayer.PlayHit();
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
            if (bulletType == Ship.Weapons.Blast)
            {
                Instantiate(ExplosionPrefab, transform.position, transform.rotation);
                GameManager.Instance.SoundPlayer.PlayHit();
            }
            collision.gameObject.GetComponent<IHitable>().Die();
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
                _collider.enabled = true;
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
            if (bulletType == Ship.Weapons.Blast)
            {
                Instantiate(ExplosionPrefab, transform.position, transform.rotation);
                Waver.Instance.AddRipple(transform.position.x);
            }
            Die();
            GameManager.Instance.SoundPlayer.PlayExplosion();
        }
    }

    private bool CollideWithWater()
    {
        return _collider.enabled && transform.position.y <= Waver.Instance.GetY(transform.position.x);
    }

    private void Die()
    {
        Destroy(gameObject);
        var allBulletsInGame = GameObject.FindGameObjectsWithTag("Bullet");
        if (allBulletsInGame.Length == 1)
            GameManager.Instance.AllBulletsDestroyed();
    }
}
