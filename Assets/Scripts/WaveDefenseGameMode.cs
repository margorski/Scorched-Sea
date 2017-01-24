using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using URandom = UnityEngine.Random;

internal class WaveDefenseGameMode : IGameMode
{
    public static int IndexOfPlayer = 1;
    public Ship Player;

    public int SpawnMinX = 2;
    public int SpawnMaxX = 7;
    public int DefenseScore;

    private Spawner DroneSpawnerInstanceRight = null;
    private Spawner DroneSpawnerInstanceLeft = null;

    public void EndMode()
    {
        DefenseScore = 0;
        Player.Die();
        DroneSpawnerInstanceRight.StopSpawning();
        DroneSpawnerInstanceLeft.StopSpawning();
        DroneSpawnerInstanceRight.DestroySpawns();
        DroneSpawnerInstanceLeft.DestroySpawns();
        GameManager.Instance.DestroyRelay(DroneSpawnerInstanceLeft.gameObject);
        GameManager.Instance.DestroyRelay(DroneSpawnerInstanceRight.gameObject);
    }

    public void OnFixedUpdate()
    {
        if(Player != null) Hud.Instance.SelectWeapon(1, Player.Weapon);
    }

    public void OnUpdate()
    {
        if (respawning && respawnTime <= Time.time)
        {
            respawning = false;
            StartMode();
        }
    }

    public void StartMode()
    {
        var xStart = -URandom.Range(-1f, 1f);
        var newShip = GameManager.Instance.InstantiateRelay(GameManager.Spawnables.PlayerShip);
        newShip.transform.position = new Vector3(xStart, Waver.Instance.GetY(xStart), 0f);
        Player = newShip.GetComponent<Ship>();
        Player.FocusCamera = true;
        Player.SetCurrent(true);
        DefenseScore = 0;
        Hud.Instance.SelectPlayer(1);
        Hud.Instance.SetPlayerTextEnabled(2, false);
        if (DroneSpawnerInstanceLeft == null)
        {
            DroneSpawnerInstanceLeft = GameManager.Instance.InstantiateRelay(GameManager.Spawnables.DroneShipSpawner).GetComponent<Spawner>();
            DroneSpawnerInstanceLeft.PlayerLocation = Player.transform;
            DroneSpawnerInstanceLeft.WorldXSpawnPosition *= -1f;
        }
        if (DroneSpawnerInstanceRight == null)
        {
            DroneSpawnerInstanceRight = GameManager.Instance.InstantiateRelay(GameManager.Spawnables.DroneShipSpawner).GetComponent<Spawner>();
            DroneSpawnerInstanceRight.PlayerLocation = Player.transform;
        }
        DroneSpawnerInstanceLeft.StartSpawning();
        DroneSpawnerInstanceRight.StartSpawning();
    }

    public ShipShooter CurrentlyPlayingShip()
    {
        return Player;
    }

    public void OnAllBulletsDestroyed()
    {
    }

    public void OnShipFired()
    {
    }

    public List<ShipShooter> GetAllPlayerShips()
    {
        return new List<ShipShooter>() { Player };
    }

    public void EnemyDestroyed(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Drone:
                if (respawning) break;
                DefenseScore += 15;
                Hud.Instance.UpdateDefenseScore(DefenseScore);
                break;
        }
    }

    private bool respawning = false;
    private float respawnTime;
    public void PlayerDied(ShipShooter player)
    {
        if (Player == player)
        {
            DroneSpawnerInstanceLeft.StopSpawning();
            DroneSpawnerInstanceLeft.DestroySpawns();
            DroneSpawnerInstanceRight.StopSpawning();
            DroneSpawnerInstanceRight.DestroySpawns();
            respawnTime = Time.time + 2f;
            respawning = true;
        }
    }


}
