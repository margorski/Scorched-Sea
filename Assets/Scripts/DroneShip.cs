using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneShip : MonoBehaviour, IHitable {


    delegate void Destroy();
    Destroy destroyAll;
    public bool _isDead = false;
    public float destroyAngle = 0;
    Transform gun;
    List<Transform> allElements;
    Transform _deck;
    Transform _leftSide;
    Transform _rightSide;
    Transform _Bottom;

    void Awake()
    {
        allElements = new List<Transform>();
        gameObject.AddComponent<LineRenderer>();
        gun = transform.FindChild("Dzialo");
        allElements.Add(gun);
        _deck = transform.FindChild("Poklad");
        allElements.Add(_deck);
        _leftSide = transform.Find("LewaBurta");
        allElements.Add(_leftSide);
        _rightSide = transform.Find("PrawaBurta");
        allElements.Add(_rightSide);
        _Bottom = transform.Find("Dno");
        allElements.Add(_Bottom);
        _isDead = false;

        destroyAll = DestroyElementsDown;
        destroyAll += DestroyElementsLeft;
        destroyAll += DestroyElementsRight;
        destroyAll += DestroyElementsUp;
    }

    // Use this for initialization
    void Start () {
        _boatMaxVelocity = Random.Range(BoatMaxVelocityMin, BoatMaxVelocityMax);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;
        GameManager.Instance.SoundPlayer.PlayExplosion();
        GameManager.Instance.DroneEnemyDestroyed(EnemyType.Drone);
    }


    public float BoatMaxVelocityMin;
    public float BoatMaxVelocityMax;
    private float _boatMaxVelocity;
    private float _boatVelocity = 0f;

    public Transform PlayerLocation;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_isDead)
        {
            destroyAll();
            Invoke("DeadAll", 1.5f);
            return;
        }
        float boatAngle;
        var boatY = Waver.Instance.GetY(transform.position.x, out boatAngle);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, boatAngle);
        float direction = 0f;
        direction = PlayerLocation == null ? 0f : Mathf.Sign(PlayerLocation.position.x - transform.position.x);
        var driveVelocity = direction  * 5f * Mathf.Cos(Mathf.Deg2Rad * boatAngle) * Time.fixedDeltaTime;
        float newX = transform.position.x;
        _boatVelocity += driveVelocity - boatAngle * 0.05f * Time.fixedDeltaTime; // speed from 
        _boatVelocity -= _boatVelocity * Time.fixedDeltaTime;
        if (Mathf.Abs(_boatVelocity) <= 0.001f) _boatVelocity = 0f;
        _boatVelocity = Mathf.Clamp(_boatVelocity, -_boatMaxVelocity, _boatMaxVelocity);
        var deltaX = Mathf.Cos(Mathf.Deg2Rad * boatAngle) * _boatVelocity * Time.fixedDeltaTime;
        transform.position = new Vector3(newX + deltaX, boatY, transform.position.z);

    }

    void DestroyElementsRight()
    {
        float randX = Random.Range(0.005f, 0.01f);
        float randY = Random.Range(-0.005f, 0.01f);
        float speed = Random.Range(2, 4.5f);
        _rightSide.Rotate(new Vector3((int)Random.Range(-1, 1) * speed, 0, (int)Random.Range(-1, 1) * speed));

        _rightSide.position = new Vector3(_rightSide.position.x + randX, _rightSide.position.y + randY, _rightSide.position.z);
    }

    void DestroyElementsLeft()
    {
        float randX = Random.Range(0.005f, 0.01f);
        float randY = Random.Range(-0.005f, 0.005f);
        float speed = Random.Range(2, 4.5f);
        _leftSide.Rotate(new Vector3((int)Random.Range(-1, 1) * speed, 0, (int)Random.Range(-1, 1) * speed));

        _leftSide.position = new Vector3(_leftSide.position.x - randX, _leftSide.position.y - randY, _leftSide.position.z);
    }

    void DestroyElementsDown()
    {
        float randX = Random.Range(0.005f, 0.01f);
        float randY = Random.Range(0.001f, 0.005f);
        float speed = Random.Range(2, 4.5f);
        _Bottom.Rotate(new Vector3((int)Random.Range(-1, 1) * speed, 0, (int)Random.Range(-1, 1) * speed));

        _Bottom.position = new Vector3(_Bottom.position.x - randX, _Bottom.position.y - randY, _Bottom.position.z);
    }

    void DestroyElementsUp()
    {
        float randX = Random.Range(0.005f, 0.01f);
        float randY = Random.Range(0.001f, 0.005f);
        float speed = Random.Range(2, 4.5f);
        _deck.Rotate(new Vector3((int)Random.Range(-1, 1) * speed, 0, (int)Random.Range(-1, 1) * speed));

        _deck.position = new Vector3(_deck.position.x - randX, _deck.position.y + randY, _deck.position.z);
    }

    void DestroyElementsGun()
    {
        float randX = Random.Range(0.005f, 0.01f);
        float randY = Random.Range(0.005f, 0.009f);
        float speed = Random.Range(2, 4.5f);
        gun.Rotate(new Vector3((int)Random.Range(-1, 1) * speed, 0, (int)Random.Range(-1, 1) * speed));
        gun.position = new Vector3(gun.position.x - randX, gun.position.y + randY, gun.position.z);

    }
    void DeadAll()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Ship"))
        {
            collider.gameObject.GetComponent<IHitable>().Die();
            Die();
        }
    }
}
