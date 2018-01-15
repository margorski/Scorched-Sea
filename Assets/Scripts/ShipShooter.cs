using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipShooter : ShipBase
{
    public enum Weapons
    {
        Blast,
        Storm,
        Armageddon
    }

    public Weapons Weapon;
    public float speed = 1;
    public float angle = 90;
    public int death = 0;
    public int kill = 0;
    public int ArmageddonShot = 1;
    public float clampMin, clampMax;
    public Bullet bullets;
    public bool FocusCamera = false;
    public bool BoatSwims = false;

    bool _isFired = false;

    public float PowerBuildUp;
    public const float _minPower = 3f;
    public const float _maxPower = 20f;
    protected float power = _minPower;

    protected GunState _gunState = GunState.Aiming;

    protected enum GunState
    {
        Aiming,
        AdjustingPower,
        Fired
    }

    void Start()
    {
        BoatSwims = GameManager.Instance.BoatBehavior == GameManager.BoatPosition.Drifting;
    }

    private float _boatVelocity = 0f;
    // Update is called once per frame
    protected override float OnFixedUpdateCalcX(float boatAngle)
    {
        float newX = transform.position.x;
        if (BoatSwims)
        {
            _boatVelocity += -boatAngle * 0.05f * Time.fixedDeltaTime; // speed from 
            _boatVelocity -= _boatVelocity * Time.fixedDeltaTime;
            if (Mathf.Abs(_boatVelocity) <= 0.001f) _boatVelocity = 0f;
            _boatVelocity = Mathf.Clamp(_boatVelocity, -0.15f, 0.15f);
            var deltaX = Mathf.Cos(Mathf.Deg2Rad * boatAngle) * _boatVelocity * Time.fixedDeltaTime;
            newX = Mathf.Clamp(newX + deltaX, -6f, 6f);
        }
        return newX;
    }

    protected virtual void ChangeWeapon()
    {
    }

    protected virtual float AimResult()
    {
        return 0f;
    }

    protected virtual bool IsShootReleased() { return false; }
    protected virtual bool IsShootPressed() { return false; }


    private void GunAction()
    {

        switch (_gunState)
        {
            case GunState.Aiming:
                if (!GameManager.Instance.SoundPlayer.IsPlaying())
                {
                    GameManager.Instance.SoundPlayer.PlayAim();
                }
                if (IsShootPressed())
                {
                    _gunState = GunState.AdjustingPower;
                    GameManager.Instance.SoundPlayer.Stop();
                    GameManager.Instance.SoundPlayer.StartCharging();
                }
                break;
            case GunState.AdjustingPower:
                power += PowerBuildUp;
                power = Mathf.Clamp(power, _minPower, _maxPower);
                ofDeck.SetPosition(1, new Vector3(rendererPosition.x, rendererPosition.y + power / _maxPower * 0.5f, rendererPosition.z));
                if (IsShootReleased() || power == _maxPower)
                {
                    _gunState = GunState.Fired;
                    GameManager.Instance.SoundPlayer.Stop();
                    GameManager.Instance.SoundPlayer.PlayShoot();
                }
                break;
            case GunState.Fired:
                _isInControl = false;
                ofDeck.SetPosition(1, startPowerBarPosition);

                if (Weapon == Weapons.Armageddon)
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

    private void Aim()
    {
        var aimResult = 0f;
        if (_gunState != GunState.AdjustingPower && _gunState != GunState.Fired)
        {
            aimResult = AimResult();
        }
        SetGun(angle + aimResult, aimResult);
    }

    protected override void OnFixedUpdate()
    {
        if (GameManager.Instance.CamMode == GameManager.CameraMode.Stabilized && FocusCamera)
            Camera.main.transform.eulerAngles = transform.eulerAngles;

        if (!_isInControl)
        {
            return;
        }
        ChangeWeapon();
        Aim();
        GunAction();
    }

    private void SetGun(float angleToSet, float z)
    {
        angle = Mathf.Clamp(angleToSet, clampMin, clampMax);
        gun.eulerAngles = new Vector3(0, 0, angle);
        GameManager.Instance.SoundPlayer.MovePitch(z);
    }

    protected void ArmageddonShots()
    {
        List<Bullet> allBulets = new List<Bullet>();
        allBulets.Add(Instantiate(bullets, gun.transform.position + (gun.transform.rotation * new Vector3(0f, 0.1f, 0f)), gun.transform.rotation) as Bullet);
        allBulets.Add(Instantiate(bullets, gun.transform.position + (gun.transform.rotation * new Vector3(0f, 0.1f, 0f)), gun.transform.rotation) as Bullet);
        allBulets.Add(Instantiate(bullets, gun.transform.position + (gun.transform.rotation * new Vector3(0f, 0.1f, 0f)), gun.transform.rotation) as Bullet);
        allBulets.Add(Instantiate(bullets, gun.transform.position + (gun.transform.rotation * new Vector3(0f, 0.1f, 0f)), gun.transform.rotation) as Bullet);
        allBulets.Add(Instantiate(bullets, gun.transform.position + (gun.transform.rotation * new Vector3(0f, 0.1f, 0f)), gun.transform.rotation) as Bullet);
        allBulets.ForEach(x => x.Shoot(Random.Range(10, 13), Random.Range(-50, 50), Weapons.Blast));
    }
}
