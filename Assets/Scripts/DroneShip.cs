using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneShip : ShipBase {

    // Use this for initialization
    void Start () {
        _boatMaxVelocity = Random.Range(BoatMaxVelocityMin, BoatMaxVelocityMax);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    protected override void OnDie() 
    {
        GameManager.Instance.DroneEnemyDestroyed(EnemyType.Drone);
    }


    public float BoatMaxVelocityMin;
    public float BoatMaxVelocityMax;
    private float _boatMaxVelocity;
    private float _boatVelocity = 0f;

    public Transform PlayerLocation;

    // Update is called once per frame
    protected override float OnFixedUpdateCalcX(float boatAngle)
    {
        float direction = 0f;
        direction = PlayerLocation == null ? 0f : Mathf.Sign(PlayerLocation.position.x - transform.position.x);
        var driveVelocity = direction  * 5f * Mathf.Cos(Mathf.Deg2Rad * boatAngle) * Time.fixedDeltaTime;
        float newX = transform.position.x;
        _boatVelocity += driveVelocity - boatAngle * 0.05f * Time.fixedDeltaTime; // speed from 
        _boatVelocity -= _boatVelocity * Time.fixedDeltaTime;
        if (Mathf.Abs(_boatVelocity) <= 0.001f) _boatVelocity = 0f;
        _boatVelocity = Mathf.Clamp(_boatVelocity, -_boatMaxVelocity, _boatMaxVelocity);
        var deltaX = Mathf.Cos(Mathf.Deg2Rad * boatAngle) * _boatVelocity * Time.fixedDeltaTime;
        return newX + deltaX;
    }
}
