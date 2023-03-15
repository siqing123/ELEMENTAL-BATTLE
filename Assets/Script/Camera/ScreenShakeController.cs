﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeController : MonoBehaviour
{        
     private float _shakeTimeRemaning;
     private float _shakeFadeTime;
     private float _shakeRotation;
    [SerializeField] private float _shakeDeathPower = 2f;
    [SerializeField] private float _shakeDeathLength = 2f;

    [SerializeField] private float _shakeHitPower = 0.5f;
    [SerializeField] private float _shakeHitLength = 1f;

    [SerializeField] private float rotationMultiplier = 1f;
    
    private PlayerManager _playerManager;
    private bool canShake;

    private void Awake()
    {
        GameLoader.CallOnComplete(Initialize);
        
    }

    private void Initialize()
    {
        _playerManager = ServiceLocator.Get<PlayerManager>();
    }

    private void Start()
    {
        for (int i = 0; i < _playerManager.mPlayersList.Count; i++)
        {
            _playerManager.mPlayersList[i].GetComponent<HeroStats>().OnShake += ScreenShakeController_OnDeath;            
        }
    }

    private void ScreenShakeController_OnDeath(float power, float length)
    {
        StartShake(power, length);
    }


    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        if(_shakeTimeRemaning > 0)
        {
            _shakeTimeRemaning -= Time.deltaTime;

            float xAmount = Random.Range(-1f, 1f) * _shakeDeathPower;
            float yAmount = Random.Range(-1f, 1f) * _shakeDeathPower;
            transform.position += new Vector3(xAmount, yAmount, 0f);

            _shakeDeathPower = Mathf.MoveTowards(_shakeDeathPower, 0f, _shakeFadeTime * Time.deltaTime);
            _shakeRotation = Mathf.MoveTowards(_shakeRotation, 0f, _shakeFadeTime * rotationMultiplier);
        }
        transform.rotation = Quaternion.Euler(0f, 0f, _shakeRotation * Random.Range(-1f, 1f));
    }

    public void StartShake(float length, float power)
    {
        _shakeTimeRemaning = length;
        _shakeDeathPower = power;
        _shakeFadeTime = power / length;
        _shakeRotation = power * rotationMultiplier;
    }
}
