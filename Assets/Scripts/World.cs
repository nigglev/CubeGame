using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CWorld
{
    static CWorld world;

    PlayerController _player;
    CameraController _camera;
    CameraController2 _camera2;
    //CameraController3 _camera3;
    CameraController4 _camera4;
    EnemyController _enemy;
    List<CoinController> _dead_coins;
    CFileManager _fm;

    bool _isPlayerFell;
    float _score = 0;
    float _high_score = 0;
    CHUDManager _hudmanager;

   

    SRVector _player_start_position;
    public SRVector PlayerStartPosition { get { return _player_start_position; } }

    SRVector _enemy_start_position;
    public SRVector EnemyStartPosition { get { return _enemy_start_position; } }

    public float Score { get { return _score; } }
    public CHUDManager HUD { get { return _hudmanager; } }

    public event EventHandler OnNewPlayer;
   
    CWorld()
    {
        _dead_coins = new List<CoinController>();
        _hudmanager = new CHUDManager();
        _fm = new CFileManager();
    }
    
    

    public static CWorld Instance
    {
        get
        {
            if (world == null)
                world = new CWorld();
            return world;
        }
    }

    public void SetPlayer(PlayerController in_player)
    {
        _player = in_player;
        NewPlayerAlert();
    }

    public void SetCamera(CameraController in_camera)
    {
        _camera = in_camera;
    }

    public void SetCamera2(CameraController2 in_camera)
    {
        _camera2 = in_camera;
    }

    //public void SetCamera3(CameraController3 in_camera)
    //{
    //    _camera3 = in_camera;
    //}

    public void SetCamera4(CameraController4 in_camera)
    {
        _camera4 = in_camera;
    }

    public CameraController GetCamera()
    {
        return _camera;
    }

    public CameraController2 GetCamera2()
    {
        return _camera2;
    }

    //public CameraController3 GetCamera3()
    //{
    //    return _camera3;
    //}

    public CameraController4 GetCamera4()
    {
        return _camera4;
    }

    protected virtual void NewPlayerAlert()
    {
        OnNewPlayer?.Invoke(this, EventArgs.Empty);
    }

    public void SetPlayerStartPosition(SRVector in_pos)
    {
        _player_start_position = in_pos;
    }

    public void SetEnemy(EnemyController in_enemy)
    {
        _enemy = in_enemy;
    }

    public void SetEnemyStartPosition(SRVector in_pos)
    {
        _enemy_start_position = in_pos;
    }

    public Vector3 GetPlayerPosition()
    {
        return _player.GetPosition();
    }

    public Transform GetPlayer()
    {
        if (_player == null)
            return null;
        return _player.GetTransform();
    }

    public Vector3 GetPlayerVelocity()
    {
        return _player.GetVelocity();
    }

    public float GetPlayerMaxVelocity()
    {
        return _player.GetMaxVelocity();
    }

    public void AddToDeadCoins(CoinController in_dead_coin)
    {   
        _dead_coins.Add(in_dead_coin);
        
    }

    public void Restart()
    {
        _player.transform.position = PlayerStartPosition.position;
        _player.transform.rotation = PlayerStartPosition.rotation;
        _player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        for (int i = 0; i < _dead_coins.Count; i++)
        {
            _dead_coins[i].OnRestartGame();
        }

        _dead_coins.Clear();
    }

    public void AddScore(float in_amount)
    {
        _score += in_amount;

        Debug.Log("New Score: " + _score.ToString());

        if (_score > _high_score)
        {
            _high_score = _score;
            Debug.Log("New high score: " + _high_score);
        }

        _hudmanager.Refresh();
    }
    
    public Transform GetNearestEnemy(Vector3 in_pos)
    {
        return GameObject.Find("Enemy").transform;
    }

}

