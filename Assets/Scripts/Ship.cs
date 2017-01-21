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


    Weapons _weapon;
    private Vector3 myRotation = Vector3.zero;
    public static bool isDead;
    private bool _isWeaponChanging = false;
    private bool _isAiming = false;
    private bool _isShake = false;
    public float power = 0;
    public float amplutude = 0.1f;
    public float speed = 1;
    public float angle = 90;
    int _minPower, _maxPower;
    int death = 0;
    int kill = 0;
    int win = 0;
    public float clampMin, clampMax;
    private Vector3 _cameraPosition;

    Transform gun;


    // Use this for initialization
    void Start () {
        gun = transform.FindChild("Dzialo");
        _minPower = 0;
        _maxPower = 100;
        _weapon = Weapons.Blast;
        isDead = false;


    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isDead)
            return;
        ChangeWeapon();
        Aim();
        PowerAdjust();

        transform.position = new Vector3(transform.position.x, Waver.Instance.GetY(transform.position.x), transform.position.z);
    }


    public void Die()
    {
        isDead = true;
        Destroy(gameObject);
        //Some other cool stuff
    }


    void ChangeWeapon()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (_weapon == Weapons.Blast)
                _weapon = Weapons.Storm;
            else _weapon = Weapons.Blast;
        }
    }

    void PowerAdjust()
    {
        if (Input.GetButton("Fire1"))
        {
            power += 0.5f;
            power = Mathf.Clamp(power,(float)_minPower,(float)_maxPower);
        }
        if (Input.GetKeyUp("space"))
        {
            _isShake = true;
            power = 0;
        }

    }

    void Aim()
    {
        float z = -Input.GetAxisRaw("Horizontal") * speed * Time.fixedDeltaTime;
        angle += z;
        angle = Mathf.Clamp(angle, clampMin, clampMax);
        gun.eulerAngles = new Vector3(0, 0, angle);
    }

}
