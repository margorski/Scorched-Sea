using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiShipShooter : ShipShooter {


    protected override void ChangeWeapon()
    {
       // me not know how
    }

    protected override bool IsShootReleased()
    {
        return _firePressed  == false;
    }
    protected override bool IsShootPressed()
    {
        return _firePressed;
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (_firePressed && powerCounter-- == 0) _firePressed = false;
    }


    private GameObject player = null;
    private const float constAim = 30f;
    private float aim = 45f;
    private int powerCounter;
    protected override float AimResult()
    {
        
        var playerX = player.transform.position.x;
        Debug.Log(angle);
        return angle < -aim && Mathf.Abs(angle - aim) > 0.001f ? 1f : -1f;
    }

    protected override void OnCurrentSetActive(bool isActive)
    {
        player = FindObjectOfType<Ship>().gameObject;
        //massive algorithm thinks haaard
        var wind = GameManager.Instance.WindForce;
        var playerX = player.transform.position.x;
        if (transform.position.x < playerX) aim = constAim;
        if (transform.position.x > playerX) aim = -1f * constAim;
        playerX = Mathf.Abs(transform.position.x - player.transform.position.x);
        var aimAngle = aim * Mathf.Deg2Rad;
        var mass = 10f;
        var gravity = 1f;
        var tempSth = wind / mass / gravity * Mathf.Cos(-aimAngle) + Mathf.Sin(-aimAngle);
        var aimPower = gravity * playerX / Mathf.Cos(-aimAngle) / tempSth;
        Debug.Log(aimPower);
        powerCounter = Mathf.RoundToInt((Mathf.Pow(aimPower, 0.5f) - _minPower) / 0.2f);

        if (isActive) Invoke("StartShooting", 2f);
    }

    private bool _firePressed = false;
    private void StartShooting()
    {
        _firePressed = true;
        //Invoke("StopShooting", 0.8f + Random.Range(-0.07f, 0.07f));
    }
    private void StopShooting()
    {
        //_firePressed = false;
    }

}
