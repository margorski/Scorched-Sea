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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            switch (Weapon)
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

    protected override bool IsShootReleased()
    {
        return fireButtonPressed == false && _mouseClick == MouseClick.None;
    }
    protected override bool IsShootPressed()
    {
        return fireButtonPressed || _mouseClick == MouseClick.Double;
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
