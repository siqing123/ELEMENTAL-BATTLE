﻿using System.Collections;
using UnityEngine;

public class HeroStats : MonoBehaviour
{
    public event System.Action<GameObject> onDebuffActivated;
    public event System.Action<GameObject> onDebuffDeActivated;
    public event System.Action OnShieldRecovered;
    public event System.Action<float,float> OnShake;
    

    private AnimationEvents _animationEvent;
    private Animator _animator;
    private HeroActions _heroActions;
    private Guard _guard;
    private HeroMovement _heroMovement;
    [SerializeField] private ParticleSystem _bloodSplatterEffect;
    [SerializeField] private ParticleSystem _burningEffect;
    [SerializeField] private GameObject _deathEffect;

    public enum TeamSetting
    {
        Team1,
        Team2,
        FFA
    };

    public TeamSetting Team = TeamSetting.FFA;

    // Basic Stats    
    [SerializeField] private float _meleeAttack = 10f;
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth = 100f;
    [SerializeField] private float _coolDown = 3f;
    [SerializeField] private float _knockBackXAmount = 10f;
    [SerializeField] private float _knockBackYAmount = 5f;
    [SerializeField] private float _knockBackLength = 0.2f;
    [SerializeField] private float _hitStun = 1f;
    [SerializeField] private float _shakePower = 1;
    [SerializeField] private float _shakeLength = 1;

    private bool _isHealing = false;
    private float _tempCoolDownTime;
    private bool _isCoolDownFinished;

    // Getters & Setters 
    public HeroMovement HeroMovement { get => _heroMovement; set => _heroMovement = value; }
    public HeroActions HeroActions { get => _heroActions; }
    public Guard Guard { get => _guard; }
    public bool CDFinished { get { return _isCoolDownFinished; } set { _isCoolDownFinished = value; } }
    public float CDTime { get { return _tempCoolDownTime; } set { _tempCoolDownTime = value; } }
    public float CoolDown { get { return _coolDown; } }
    public float CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; } }
    public float MaxHealth { get { return _maxHealth; } }
    public float AttackDamage { get { return _meleeAttack; } }
    public float KnockBackXAmount { get { return _knockBackXAmount; } }
    public float KnockBackYAmount { get { return _knockBackYAmount; } }
    public float KnockBackLength { get { return _knockBackLength; } }
    public float HitStun { get { return _hitStun; } }


    //Elemental Type
    [SerializeField] private Elements.ElementalAttribute _elementalType;
    public Elements.ElementalAttribute GetElement { get => _elementalType; } 

    //Buff & Debuff Effects
    [SerializeField] private StatusEffects.PositiveEffects _positiveEffect = StatusEffects.PositiveEffects.None;
    [SerializeField] private StatusEffects.NegativeEffects _negativeEffect = StatusEffects.NegativeEffects.None;
    public StatusEffects.PositiveEffects Buff { get => _positiveEffect;  set => _positiveEffect = value; } 
    public StatusEffects.NegativeEffects DeBuff { get => _negativeEffect;  set => _negativeEffect = value; }

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _animationEvent = GetComponentInChildren<AnimationEvents>();
        _currentHealth = _maxHealth;
        _tempCoolDownTime = 0;
        _guard = GetComponent<Guard>();
        _heroMovement = GetComponent<HeroMovement>();
        _heroActions = GetComponent<HeroActions>();
    }

    private void Start()
    {
        _guard.OnShieldRecover += RestoreShield;
    }

    private void FixedUpdate()
    {
        if (_tempCoolDownTime <= 0.0f)
        {
            _tempCoolDownTime = 0.0f;
            _isCoolDownFinished = true;
        }
        if (_tempCoolDownTime > 0.0f)
        {
            _tempCoolDownTime -= Time.deltaTime;
        }

        if (_currentHealth <= 0)
        {
            HeroDie();
        }
    }

    public void TakeDamageFromProjectile(float damage)
    {
        if (!_animationEvent.DashProjectileInvincibility)
        {
            _isHealing = false;
            _currentHealth -= damage;
            OnShake.Invoke(_shakePower * 0.5f,_shakeLength * 0.5f);
            switch (_negativeEffect)
            {
                case StatusEffects.NegativeEffects.OnFire:
                    break;
                case StatusEffects.NegativeEffects.Slowed:
                    onDebuffActivated?.Invoke(gameObject);
                    break;
                case StatusEffects.NegativeEffects.Stunned:
                    break;
                case StatusEffects.NegativeEffects.None:
                    break;
                default:
                    break;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        _isHealing = false;
        if (_currentHealth <= 0)
        {
            HeroDie();
        }
        else
        {
            _currentHealth -= damage;
        }

        BloodEffect();
        _animator.SetTrigger("HurtTrigger");
    }

    public void FallOutOfMap()
    {
        _currentHealth -= MaxHealth / 2f;
        if(_currentHealth <= 0)
        {
            HeroDie();
        }
        else
        {
            SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
            spawnManager.RespawnPlayer(gameObject);
        }
    }

    public void RestoreHealthOverTime(float duration, float amount, float maxAmount)
    {
        _isHealing = true;
        StartCoroutine(HealOverTimeCoroutine(duration, amount, maxAmount));
    }

    private IEnumerator HealOverTimeCoroutine(float duration, float amount, float maxAmount)
    {
        float amountHealed = 0;
            while (amountHealed <= maxAmount && _isHealing)
            {
                if (CurrentHealth >= MaxHealth)
                {
                    CurrentHealth = MaxHealth;
                    break;
                }
                amountHealed += amount;
                _currentHealth += amount;
                yield return new WaitForSeconds(duration);
            }
    }

    private void RestoreShield()
    {
        if (!_guard.IsShieldDisabled)
        {
            StartCoroutine(RestoreShieldOverTimeCoroutine(_guard.ShieldRecoveryAmount, _guard.ShieldRecoveryTick));
        }
        else
        {
            StartCoroutine(RestoreBrokenShield(_guard.ShieldRecoveryTime));
        }
    }

    private IEnumerator RestoreShieldOverTimeCoroutine(float restoreAmount, float restoreTick)
    {        
        while ((_guard.ShieldEnergy < _guard.ShieldMaxEnergy) && !_guard.Guarding)
        {
            _guard.ShieldEnergy += restoreAmount;
            yield return new WaitForSeconds(restoreTick);
        }
    }

    private IEnumerator RestoreBrokenShield(float shieldRecoveryTime)
    {
        yield return new WaitForSeconds(shieldRecoveryTime);
        OnShieldRecovered?.Invoke();

    }

    public void DamageOverTime(float damageAmount, float damageDuration)
    {
       
        StartCoroutine(DamageOverTimeCoroutine(damageAmount, damageDuration));
    }

    private IEnumerator DamageOverTimeCoroutine(float damageAmount, float duration)
    {
        float amountDamaged = 0;
        float damagePerloop = damageAmount / duration;
        while (amountDamaged < damageAmount)
        {
            _burningEffect.Play();
            _currentHealth -= damagePerloop;
            if (_currentHealth <= 0)
            {
                HeroDie();
            }
            Debug.Log(_elementalType.ToString() + "Hero Current Health" + _currentHealth);
            amountDamaged += damagePerloop;
            yield return new WaitForSeconds(1f);
        }
        _negativeEffect = StatusEffects.NegativeEffects.None;
        _burningEffect.Stop();
    }

    public void SlowMovement(float mSlowAmount, float mSlowDuration)
    {
        StartCoroutine(SlowEffectCoroutine(mSlowAmount, mSlowDuration));
    }

    private IEnumerator SlowEffectCoroutine(float slowAmount, float duration)
    {
        HeroMovement heromovement = GetComponent<HeroMovement>();
        float maxSpeed = heromovement.Speed;
        heromovement.Speed = slowAmount;
        float slowPerLoop = slowAmount / duration;
        while (heromovement.Speed < maxSpeed)
        {
            heromovement.Speed += slowPerLoop;
            Debug.Log(_elementalType.ToString() + "Hero Current Speed" + heromovement.Speed);
            yield return new WaitForSeconds(1f);
        }
        _negativeEffect = StatusEffects.NegativeEffects.None;
        onDebuffDeActivated?.Invoke(gameObject);
    }

    private void HeroDie()
    {
        PlayerManager playermanager = ServiceLocator.Get<PlayerManager>();
        Instantiate(_deathEffect, transform.position, Quaternion.identity);
        if (playermanager.TeamOne.Contains(gameObject))
        {
            playermanager.TeamOne.Remove(gameObject);
        }
        if (playermanager.TeamTwo.Contains(gameObject))
        {
            playermanager.TeamTwo.Remove(gameObject);
        }
        OnShake?.Invoke(_shakePower * 2f, _shakeLength * 2f);
        this.gameObject.SetActive(false);
    }

    private void BloodEffect()
    {
        _bloodSplatterEffect.Play();
    }
}
