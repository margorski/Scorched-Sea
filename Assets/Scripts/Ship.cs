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
    public static bool isDead;
    private bool _isWeaponChanging = false;
    private bool _isAiming = false;
    public float power = 0;
    int _minPower, _maxPower;
    int death = 0;
    int kill = 0;
    int win = 0;


    // Use this for initialization
    void Start () {
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
        PowerAdjust();   
    }


    public void Die()
    {
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
    }

    void Aim()
    {
        //Aiming
    }
}
