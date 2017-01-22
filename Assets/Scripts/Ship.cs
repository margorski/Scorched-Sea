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

    delegate void Destroy();

    Destroy destroyAll;
    public Weapons Weapon;
    private Vector3 myRotation = Vector3.zero;
    public bool _isDead = false;
    public float power = 10;
    public float amplutude = 0.1f;
    public float speed = 1;
    public float angle = 90;
    public float destroyAngle = 0;
    public float _minPower, _maxPower;
    public int death = 0;
    public int kill = 0;
    public float clampMin, clampMax;
    public Bullet bullets;
    public bool FocusCamera = false;
    Transform gun;
    List<Transform> allElements;

    private GunState _gunState = GunState.Aiming;

    Transform _deck;

    Transform _leftSide;
    Transform _rightSide;
    Transform _Bottom;
    LineRenderer ofDeck;

    Vector3 rendererPosition;
    Vector3 startPowerBarPosition;
    enum GunState
    {
        Aiming,
        AdjustingPower,
        Fired
    }

    public void SetCurrent(bool current)
    {
        var width = 0.02f;
        if (current)
            width += 0.03f;
        var colorGreen = Color.green;
        var colorYellow = Color.yellow;
        if(current)
        {
            foreach (var element in allElements)
            {
                var temp = element.GetComponent<LineRenderer>();
                temp.endColor = temp.startColor = colorYellow;
                temp.endWidth = temp.startWidth = width;
            }
        }
        else
        {
            foreach (var element in allElements)
            {
                var temp = element.GetComponent<LineRenderer>();
                temp.endColor = temp.startColor = colorGreen;
                temp.endWidth = temp.startWidth = width;
            }
        }
    }
    // Use this for initialization
    void Awake () {

        allElements = new List<Transform>();
        gameObject.AddComponent<LineRenderer>();
        gun = transform.FindChild("Dzialo");
        allElements.Add(gun);
        _minPower = 3f;
        _maxPower = 20f;
        power = _minPower;
        _deck = transform.FindChild("Poklad");
        allElements.Add(_deck);
        Weapon = Weapons.Blast;
        _leftSide = transform.Find("LewaBurta");
        allElements.Add(_leftSide);
        _rightSide = transform.Find("PrawaBurta");
        allElements.Add(_rightSide);
        _Bottom = transform.Find("Dno");
        allElements.Add(_Bottom);
        ofDeck = gameObject.GetComponent<LineRenderer>();
        ofDeck.useWorldSpace = false;
        _isDead = false;
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
        destroyAll = DestroyElementsDown;
        destroyAll += DestroyElementsGun;
        destroyAll += DestroyElementsLeft;
        destroyAll += DestroyElementsRight;
        destroyAll += DestroyElementsUp;
        var indexOfPlayer = GameManager.Instance.Players.IndexOf(this);
        if (indexOfPlayer != -1)
            Hud.Instance.SelectWeapon(indexOfPlayer, Weapon);
    }

    private bool fireButtonPressed = false;
    void Update()
    {
        fireButtonPressed = Input.GetButton("Fire1");
    }
	
	// Update is called once per frame
	void FixedUpdate () {
            if (_isDead)
            {
                destroyAll();
                Invoke("DeadAll", 1.5f);
                return;
            }

       
        float boatAngle;
        transform.position = new Vector3(transform.position.x, Waver.Instance.GetY(transform.position.x, out boatAngle), transform.position.z);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, boatAngle);
        if(GameManager.Instance.CamMode == GameManager.CameraMode.Stabilized && FocusCamera)
            Camera.main.transform.eulerAngles = transform.eulerAngles;

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
        _isDead = true;
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
                if (!GameManager.Instance.SoundPlayer.IsPlaying())
                {
                    GameManager.Instance.SoundPlayer.PlayAim();
                }
                if (fireButtonPressed)
                {
                    _gunState = GunState.AdjustingPower;
                    GameManager.Instance.SoundPlayer.Stop();
                    GameManager.Instance.SoundPlayer.StartCharging();
                }
                break;
            case GunState.AdjustingPower:
                power += 0.2f;
                power = Mathf.Clamp(power, _minPower, _maxPower);
                ofDeck.SetPosition(1, new Vector3(rendererPosition.x, rendererPosition.y + power/_maxPower * 0.5f, rendererPosition.z));
                if (fireButtonPressed == false || power == _maxPower)
                {
                    _gunState = GunState.Fired;
                    GameManager.Instance.SoundPlayer.Stop();
                    GameManager.Instance.SoundPlayer.PlayShoot();
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
        if (_gunState == GunState.AdjustingPower || _gunState == GunState.Fired)
            return;

        float z = -Input.GetAxisRaw("Horizontal") * speed * Time.fixedDeltaTime;
        angle += z;
        angle = Mathf.Clamp(angle, clampMin, clampMax);
        gun.eulerAngles = new Vector3(0, 0, angle);
        GameManager.Instance.SoundPlayer.MovePitch(z);
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
        GameManager.Instance.Players[GameManager.Instance.Players.IndexOf(this)] = null;
       // gameObject.SetActive(false);
    }
}
