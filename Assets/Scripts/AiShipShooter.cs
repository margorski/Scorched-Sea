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


    private GameObject player = null;
    private float aim = 0f;
    protected override float AimResult()
    {
        //massive algorithm thinks haaard
        
        if (player != null)
        {
            var playerX = player.transform.position.x;
            if (transform.position.x < playerX && angle > -aim) return -1f;
            if (transform.position.x > playerX && angle < aim) return 1f;
            return 0f;
        }
        return angle < aim ? 1f : -1f;
    }

    protected override void OnCurrentSetActive(bool isActive)
    {
        player = FindObjectOfType<Ship>().gameObject;
        aim = Random.Range(15f, 30f);
        if(isActive) Invoke("StartShooting", 2f);
    }

    private bool _firePressed = false;
    private void StartShooting()
    {
        _firePressed = true;
        Invoke("StopShooting", 0.7f + Random.Range(-0.05f, 0.05f));
    }
    private void StopShooting()
    {
        _firePressed = false;
    }

}
