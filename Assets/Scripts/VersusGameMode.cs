using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using URandom = UnityEngine.Random;

internal class Player
{
    public string Name;
    public int Wins = 0;
    public int Loses = 0;
    public int index;
    public bool IsAi = false;

    public Player(int i) { index = i; Name = "Player" + i; }

    public static implicit operator int(Player player)
    {
        return player.index;
    }

    public static implicit operator Player(int index)
    {
        return new Player(index);
    }
}

internal class PlayerEqComp : IEqualityComparer<Player>
{
    public bool Equals(Player x, Player y)
    {
        return x.index == y.index;
    }

    public int GetHashCode(Player obj)
    {
        return obj.index.GetHashCode();
    }
}

public enum TurnPhaseType
{
    PlayerMove,
    BulletMove,
    EndOfTurn,
    EndOfRound
}

internal class VersusGameMode : IGameMode
{
    public static int NumberOfPlayers = 2;
    public readonly Dictionary<Player, ShipShooter> Players = new Dictionary<Player, ShipShooter>(new PlayerEqComp());

    public int SpawnMinX = 5;
    public int SpawnMaxX = 7;
    public int PlayerInControl = 0;
    public TurnPhaseType TurnPhase = TurnPhaseType.PlayerMove;
    public float TurnEndDelay = 0.5f;
    public float RoundEndDelay = 3.0f;

    private float timer;
    private int StartingPlayer = 0;
    private int _turnCounter = 0;

    public void EndMode()
    {
        Players.Values.ToList().ForEach(x => x.Die());
        Players.Clear();
    }

    public void OnFixedUpdate()
    {
        foreach(var player in Players)
            Hud.Instance.SelectWeapon(player.Key.index, player.Value.Weapon);
        if (TurnPhase == TurnPhaseType.EndOfTurn || TurnPhase == TurnPhaseType.EndOfRound)
        {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0.0f)
            {
                NextPhase();
            }
        }
    }

    public void OnUpdate()
    {
    }

    public void StartMode()
    {
        InitPlayers();
        InitShips();
        InitRound();
        Hud.Instance.SetPlayerTextEnabled(0, true);
        Hud.Instance.SetPlayerTextEnabled(1, true);
    }

    public ShipShooter CurrentlyPlayingShip()
    {
        if(Players.ContainsKey(PlayerInControl)) return Players[PlayerInControl];
        return null;
    }

    public void OnAllBulletsDestroyed()
    {
        NextPhase();
    }

    public void OnShipFired()
    {
        NextPhase();
    }

    public List<ShipShooter> GetAllPlayerShips()
    {
        return Players.Values.Where(x => x != null).ToList();
    }

    public void EnemyDestroyed(EnemyType type)
    {
    }

    public void PlayerDied(ShipShooter player)
    {
    }

    private void InitRound()
    {
        Waver.Instance.Init(URandom.Range(1, Waver.Instance.Levels.Count + 1));
        StartTurn();
        // randomize wave
        InitShips();
        Hud.Instance.SelectPlayer(PlayerInControl);
        foreach (var player in Players.Values) player.FocusCamera = false;
        Players[PlayerInControl].FocusCamera = true;
        TurnPhase = TurnPhaseType.PlayerMove;
    }

    private Dictionary<int, float> GenerateSpawnPoints()
    {
        return new Dictionary<int, float>()
        {
            { 1 , -URandom.Range(SpawnMinX, SpawnMaxX) },
            { 2 , URandom.Range(SpawnMinX, SpawnMaxX) }
        };
    }

    private void InitPlayers(bool withAi = false)
    {
        foreach (var player in Players)
        {
            GameManager.Instance.DestroyRelay(player.Value.gameObject);
        }
        Players.Clear();
        for (int i = 0; i < NumberOfPlayers; i++)
        {
            var player = new Player(i+1);
            Players.Add(player, null);
            Hud.Instance.UpdateScores(player.index, player.Wins, player.Loses);
        }
        if (withAi) Players.Keys.Last().IsAi = true;
        PlayerInControl = Mathf.RoundToInt(URandom.Range(1, Players.Count));
    }

    private void InitShips()
    {
        var spawnPoints = GenerateSpawnPoints();
        
        foreach(var player in Players.Where(x => x.Value == null).ToArray())
        {
            var spawnable = GameManager.Spawnables.PlayerShip;
            if (player.Key.IsAi) spawnable = GameManager.Spawnables.AiShip;
            var ship = GameManager.Instance.InstantiateRelay(spawnable).GetComponent<ShipShooter>();
            Players[player.Key] = ship;
            Hud.Instance.SelectWeapon(player.Key.index, ship.Weapon);
            ship.gameObject.transform.position = new Vector3(spawnPoints[player.Key], Waver.Instance.GetY(spawnPoints[player.Key]));
            ship.gameObject.SetActive(true);
            ship.ArmageddonShot = 1;
        }

        UpdateCurrentDrawing();
    }

    private void UpdateCurrentDrawing()
    {
        foreach (var player in Players)
        {
            player.Value.SetCurrent(PlayerInControl == player.Key);
        }
    }

    private void StartTurn()
    {
        GameManager.Instance.RandomizeWind();
        Waver.Instance.OnNewTurn();
    }

    private void NextTurn()
    {
        _turnCounter++;
        StartTurn();
    }

    private void NextPlayer()
    {
        Players[PlayerInControl].FocusCamera = false;
        var tempPlayerIndex = PlayerInControl;
        var maxIndex = Players.Max(x => x.Key.index);
        var minIndex = Players.Min(x => x.Key.index);
        while(true)
        {
            tempPlayerIndex = (tempPlayerIndex + 1);
            if (tempPlayerIndex > maxIndex) tempPlayerIndex = minIndex;
            if (Players.ContainsKey(tempPlayerIndex) && Players[tempPlayerIndex] != null)
            {
                PlayerInControl = tempPlayerIndex;
                Players[PlayerInControl].FocusCamera = true;
                Hud.Instance.SelectPlayer(PlayerInControl);
                break;
            }
        };
        UpdateCurrentDrawing();
    }


    public void NextPhase()
    {
        switch (TurnPhase)
        {
            case TurnPhaseType.PlayerMove:
                TurnPhase = TurnPhaseType.BulletMove;
                break;
            case TurnPhaseType.BulletMove:
                TurnPhase = TurnPhaseType.EndOfTurn;
                timer = TurnEndDelay;
                break;
            case TurnPhaseType.EndOfTurn:
                if (Players.Count(x => x.Value._isDead == false) <= 1)
                {
                    //end of turn
                    foreach (var player in Players.Where(x => x.Value._isDead == false))
                    {
                        player.Key.Wins++;
                        GameManager.Instance.ShowWinMessage(player.Key.Name, RoundEndDelay);
                        Hud.Instance.UpdateScores(player.Key.index, player.Key.Wins, player.Key.Loses);
                    }
                    foreach (var player in Players.Where(x => x.Value._isDead))
                    {
                        player.Key.Loses++;
                        Hud.Instance.UpdateScores(player.Key.index, player.Key.Wins, player.Key.Loses);
                    }

                    TurnPhase = TurnPhaseType.EndOfRound;
                    timer = RoundEndDelay;
                }
                else
                {
                    NextPlayer();
                    // all players played, next round
                    if (PlayerInControl == StartingPlayer)
                    {
                        NextTurn();
                    }
                    // change round

                    TurnPhase = TurnPhaseType.PlayerMove;
                }
                break;
            case TurnPhaseType.EndOfRound:
                InitRound();
                break;
        }
    }
}
