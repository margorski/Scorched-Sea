using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : ShipShooter {


    private float _doubleClickInterval = 0.3f;
    private float _clickTimestamp;
    private enum MouseClick
    {
        None,
        NotSure,
        Single,
        Double
    }
    private MouseClick _mouseClick = MouseClick.None;
    private bool fireButtonPressed = false;
    void Update()
    {
        fireButtonPressed = Input.GetButton("Fire1");
        switch (_mouseClick)
        {
            case MouseClick.None:
                if (Input.GetMouseButtonDown(0))
                {
                    _clickTimestamp = Time.time;
                    _mouseClick = MouseClick.NotSure;
                }
                break;
            case MouseClick.NotSure:
                if (Input.GetMouseButtonDown(0) && (_clickTimestamp + _doubleClickInterval) > Time.time)
                {
                    _mouseClick = MouseClick.Double;
                }
                if ((_clickTimestamp + _doubleClickInterval) < Time.time)
                {
                    _mouseClick = MouseClick.Single;
                }
                break;
            case MouseClick.Single:
            case MouseClick.Double:
                if (Input.GetMouseButton(0) == false)
                {
                    _mouseClick = MouseClick.None;
                }
                break;
        }
    }

    protected override void ChangeWeapon()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            switch(Weapon)
            {
                case Weapons.Blast:
                    Weapon = Weapons.Storm;
                    break;
                case Weapons.Storm:
                    if (ArmageddonShot >= 1)
                        Weapon = Weapons.Armageddon;
                    else Weapon = Weapons.Blast;
                    break;
                case Weapons.Armageddon:
                    Weapon = Weapons.Blast;
                    break;

            }
        }
    }

    protected override void GunAction()
    {
        switch (_gunState)
        {
            case GunState.Aiming:
                if (!GameManager.Instance.SoundPlayer.IsPlaying())
                {
                    GameManager.Instance.SoundPlayer.PlayAim();
                }
                if (fireButtonPressed || _mouseClick == MouseClick.Double)
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
                if ((fireButtonPressed == false && _mouseClick == MouseClick.None) || power == _maxPower)
                {
                    _gunState = GunState.Fired;
                    GameManager.Instance.SoundPlayer.Stop();
                    GameManager.Instance.SoundPlayer.PlayShoot();
                }
                break;
            case GunState.Fired:
            	ofDeck.SetPosition(1, startPowerBarPosition);

                if(Weapon == Weapons.Armageddon)
                {
                    ArmageddonShots();
                    ArmageddonShot = 0;
                    Weapon = Weapons.Blast;
                }
                else
                {
                    Bullet bullet = Instantiate(bullets, gun.transform.position + (gun.transform.rotation * new Vector3(0f, 0.1f, 0f)), gun.transform.rotation) as Bullet;
                    bullet.Shoot(power, angle, Weapon);

                }
                GameManager.Instance.ShipFired();
                power = _minPower;
                _gunState = GunState.Aiming;
                break;
        }
    }

    protected override float AimResult()
    {
        if (_mouseClick == MouseClick.Single)
        {
            var mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePoint.z = 0f;
            var GunToMouse = mousePoint - gun.position;
            var gunAngle = Mathf.Atan2(GunToMouse.y, GunToMouse.x) * Mathf.Rad2Deg - 90f;
            return (gunAngle - angle) * Time.fixedDeltaTime;
            
        }
        else
        {
            return -Input.GetAxisRaw("Horizontal") * speed * Time.fixedDeltaTime;
        }
    }

}
