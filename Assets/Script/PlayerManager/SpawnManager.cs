﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnManager : MonoBehaviour
{
    private PlayerManager _playerManager;
    [SerializeField] private List<Transform> _startSpawnPoints = new List<Transform>();
    [SerializeField] private List<Transform> _respawnPoints = new List<Transform>();



    private PlayerInputManager _playerInputManager;
    private void Awake()
    {
        GameLoader.CallOnComplete(Initialize);
        _respawnPoints = _startSpawnPoints;
    }

    private void Initialize()
    {
        _playerManager = ServiceLocator.Get<PlayerManager>();
        _playerManager.TeamOne.Clear();
        _playerManager.TeamTwo.Clear();
        _playerManager.TeamThree.Clear();
        _playerManager.TeamFour.Clear();

        if (_playerManager.FireHero.GetComponent<HeroMovement>().ControllerInput != HeroMovement.Controller.None)
        {
            GameObject fireHero = Instantiate(_playerManager.FireHero);    
            //fireHero.GetComponent<HeroMovement>().PlayerInput = fireHero.GetComponent<PlayerInput>().KeyboardMouseScheme;
            fireHero.SetActive(true);
            _playerManager.mPlayersList[0] = fireHero;
            //_playerInputManager.playerPrefab = _playerManager.mPlayersList[0];
            if (_playerManager.mPlayersList[0].tag == "Team1")
            {
                _playerManager.TeamOne.Add(_playerManager.mPlayersList[0]);
            }
            if (_playerManager.mPlayersList[0].tag == "Team2")
            {
                _playerManager.TeamTwo.Add(_playerManager.mPlayersList[0]);
            }
            if (_playerManager.mPlayersList[0].tag == "Team3")
            {
                _playerManager.TeamTwo.Add(_playerManager.mPlayersList[0]);
            }
            if (_playerManager.mPlayersList[0].tag == "Team4")
            {
                _playerManager.TeamTwo.Add(_playerManager.mPlayersList[0]);
            }
            RandomizeSpawn(fireHero);

        }
        if (_playerManager.WaterHero.GetComponent<HeroMovement>().ControllerInput != HeroMovement.Controller.None)
        {
            GameObject waterHero = Instantiate(_playerManager.WaterHero);
            waterHero.SetActive(true);
            _playerManager.mPlayersList[1] = waterHero;
            if (_playerManager.mPlayersList[1].tag == "Team1")
            {
                _playerManager.TeamOne.Add(_playerManager.mPlayersList[1]);
            }
            if (_playerManager.mPlayersList[1].tag == "Team2")
            {
                _playerManager.TeamTwo.Add(_playerManager.mPlayersList[1]);
            }
            if (_playerManager.mPlayersList[1].tag == "Team3")
            {
                _playerManager.TeamTwo.Add(_playerManager.mPlayersList[1]);
            }
            if (_playerManager.mPlayersList[1].tag == "Team4")
            {
                _playerManager.TeamTwo.Add(_playerManager.mPlayersList[1]);
            }
            RandomizeSpawn(waterHero);

        }
        if (_playerManager.EarthHero.GetComponent<HeroMovement>().ControllerInput != HeroMovement.Controller.None)
        {
            GameObject earthHero = Instantiate(_playerManager.EarthHero);
            earthHero.SetActive(true);
            _playerManager.mPlayersList[3] = earthHero;
            if (_playerManager.mPlayersList[3].tag == "Team1")
            {
                _playerManager.TeamOne.Add(_playerManager.mPlayersList[3]);
            }
            if (_playerManager.mPlayersList[3].tag == "Team2")
            {
                _playerManager.TeamTwo.Add(_playerManager.mPlayersList[3]);
            }
            if (_playerManager.mPlayersList[3].tag == "Team3")
            {
                _playerManager.TeamTwo.Add(_playerManager.mPlayersList[3]);
            }
            if (_playerManager.mPlayersList[3].tag == "Team4")
            {
                _playerManager.TeamTwo.Add(_playerManager.mPlayersList[3]);
            }
            RandomizeSpawn(earthHero);

        }
        if (_playerManager.AirHero.GetComponent<HeroMovement>().ControllerInput != HeroMovement.Controller.None)
        {
            GameObject airHero = Instantiate(_playerManager.AirHero);
            airHero.SetActive(true);
            _playerManager.mPlayersList[2] = airHero;
            if (_playerManager.mPlayersList[2].tag == "Team1")
            {
                _playerManager.TeamOne.Add(_playerManager.mPlayersList[2]);
            }
            if (_playerManager.mPlayersList[2].tag == "Team2")
            {
                _playerManager.TeamTwo.Add(_playerManager.mPlayersList[2]);
            }
            if (_playerManager.mPlayersList[2].tag == "Team3")
            {
                _playerManager.TeamTwo.Add(_playerManager.mPlayersList[2]);
            }
            if (_playerManager.mPlayersList[2].tag == "Team4")
            {
                _playerManager.TeamTwo.Add(_playerManager.mPlayersList[2]);
            }
            RandomizeSpawn(airHero);
        }
    }

    private void RandomizeSpawn(GameObject player)
    {
        int randIndex = Random.Range(0, _startSpawnPoints.Count);
        player.transform.position = _startSpawnPoints[randIndex].position;
        _startSpawnPoints.RemoveAt(randIndex);
    }

    public void RespawnPlayer(GameObject player)
    {
        int randIndex = Random.Range(0, _respawnPoints.Count);
        player.transform.position = _respawnPoints[randIndex].position;
    }

}
