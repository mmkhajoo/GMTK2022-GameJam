using System;
using System.Collections;
using System.Collections.Generic;
using Managers.Audio_Manager;
using UnityEngine;
using UnityEngine.Events;
using WeaponSystem;

public abstract class Projectile : MonoBehaviour
{
    public UnityEvent OnDestroy;

    [SerializeField] protected float _destroyTime;
    protected bool _isStoped;

    protected int _damage;
    protected float _speed;
    protected bool _canDamage;
    protected Vector3 _direction;
    protected CharacterType _targetType;
    protected GameObject _launcher;

    [SerializeField] private AudioSource _audioSource;
    public virtual void Setup(Vector3 target, Weapon weapon, CharacterType targetType, GameObject launcher)
    {
        _direction = (target - transform.position).normalized;
        _speed = weapon.projectileSpeed;
        _damage = weapon.weaponDamage;
        _launcher = launcher;
        _canDamage = true;
        _isStoped = false;
    }

    protected abstract void Execute();

    private void Update()
    {
        Execute();
    }
    
    public void PlaySound(string str)
    {
        AudioManager.instance.PlaySoundEffect(_audioSource,Enum.Parse<AudioTypes>(str));
    }
    public void StopSound()
    {
        AudioManager.instance.StopSoundEffect(_audioSource);
    }
}
