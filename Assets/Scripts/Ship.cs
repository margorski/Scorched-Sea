using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Ihitable
{
    void Die();
}

public class Ship : MonoBehaviour, Ihitable {

    public enum Weapons
    {
        Blast,
        Storm
    }


    public Weapons Weapon;
    private Vector3 myRotation = Vector3.zero;
    private bool _isWeaponChanging = false;
    private bool _isAiming = false;
    public float power = 10;
    public float amplutude = 0.1f;
    public float speed = 1;
    public float angle = 90;
    public float _minPower, _maxPower;
    public int death = 0;
    public int kill = 0;
    public float clampMin, clampMax;
    private Vector3 _cameraPosition;
    public Bullet bullets;
    Transform gun;
    public string playerName;

    private GunState _gunState = GunState.Aiming;

    GameObject _deck;

    LineRenderer _leftSide;
    LineRenderer _rightSide;
    LineRenderer _Bottom;
    LineRenderer _gun;
    LineRenderer ofDeck;

    Vector3 rendererPosition;
    Vector3 startPowerBarPosition;
    enum GunState
    {
        Aiming,
        AdjustingPower,
        Fired
    }

    // Use this for initialization
    void Start () {


        gameObject.AddComponent<LineRenderer>();
        gun = transform.FindChild("Dzialo");
        _minPower = 3f;
        _maxPower = 20f;
        power = _minPower;
        _deck = transform.FindChild("Poklad").gameObject;
        Weapon = Weapons.Blast;
        _leftSide = transform.Find("LewaBurta").gameObject.GetComponent<LineRenderer>();
        _rightSide = transform.Find("PrawaBurta").gameObject.GetComponent<LineRenderer>();
        _Bottom = transform.Find("Dno").gameObject.GetComponent<LineRenderer>();
        ofDeck = gameObject.GetComponent<LineRenderer>();
        ofDeck.useWorldSpace = false;
        ofDeck.startWidth = 0.1f;
        
        rendererPosition = _deck.GetComponent<LineRenderer>().GetPosition(0);

        float deltaX;
        if (Mathf.Sign(transform.position.x) > 0.0f)
            deltaX = 0.9f;
        else
            deltaX = -0.3f;

        rendererPosition = new Vector3(rendererPosition.x + deltaX, rendererPosition.y, rendererPosition.z);

        for (int i = 0; i < 2; i++)
        {
            ofDeck.SetPosition(i, rendererPosition);
        }
        var ofDeckPosition = ofDeck.GetPosition(1);
        startPowerBarPosition = new Vector3(rendererPosition.x, rendererPosition.y, rendererPosition.z);
    }

    private bool fireButtonPressed = false;
    void Update()
    {
        fireButtonPressed = Input.GetButton("Fire1");
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float boatAngle;
        transform.position = new Vector3(transform.position.x, Waver.Instance.GetY(transform.position.x, out boatAngle), transform.position.z);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, boatAngle);


        if (GameManager.Instance.GetCurrentPlayer() != this
             || GameManager.Instance.TurnPhase != GameManager.TurnPhaseType.PlayerMove)
        {
            return;
        }

        ChangeWeapon();
        Aim();
        GunAction();
    }


    public void Die()
    {
        // Destroy(gameObject);
        //Some other cool stuff
        gameObject.SetActive(false);
    }


    void ChangeWeapon()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (Weapon == Weapons.Blast)
                Weapon = Weapons.Storm;
            else Weapon = Weapons.Blast;
            Hud.Instance.SelectWeapon(GameManager.Instance.currentPlayer, Weapon);
        }
    }

    void GunAction()
    {
        switch (_gunState)
        {
            case GunState.Aiming:
                if (fireButtonPressed)
                {
                    _gunState = GunState.AdjustingPower;
                }
                break;
            case GunState.AdjustingPower:
                power += 0.2f;
                power = Mathf.Clamp(power, _minPower, _maxPower);
                ofDeck.SetPosition(1, new Vector3(rendererPosition.x, rendererPosition.y + power/_maxPower * 0.5f, rendererPosition.z));
                if (fireButtonPressed == false)
                {
                    _gunState = GunState.Fired;
                }
                break;
            case GunState.Fired:
            	ofDeck.SetPosition(1, startPowerBarPosition);
                Bullet bullet = Instantiate(bullets, gun.transform.position + (gun.transform.rotation * new Vector3(0f,0.1f,0f)), gun.transform.rotation) as Bullet;
                bullet.Shoot(power, angle, Weapon);
                GameManager.Instance.NextPhase();
                power = _minPower;
                _gunState = GunState.Aiming;
                break;
        }
    }

    void Aim()
    {
        if (_gunState == GunState.AdjustingPower)
            return;

        float z = -Input.GetAxisRaw("Horizontal") * speed * Time.fixedDeltaTime;
        angle += z;
        angle = Mathf.Clamp(angle, clampMin, clampMax);
        gun.eulerAngles = new Vector3(0, 0, angle);
    }

    void DestroyElements()
    {

    }
}
