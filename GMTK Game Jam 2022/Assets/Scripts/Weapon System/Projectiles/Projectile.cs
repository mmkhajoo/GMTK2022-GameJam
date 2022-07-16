using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WeaponSystem;

public abstract class Projectile : MonoBehaviour
{
    public UnityEvent OnDestroy;

    [SerializeField]
    private float _destroyTime;

    protected int _damage;
    protected float _speed;
    protected Vector3 _direction;
    protected CharacterType _targetType;
    protected GameObject _launcher;

    public virtual void Setup(Vector3 target, Weapon weapon, CharacterType targetType, GameObject launcher)
    {
        _direction = (target - transform.position).normalized;
        _speed = weapon.projectileSpeed;
        _damage = weapon.weaponDamage;
        _launcher = launcher;

        Destroy(gameObject, _destroyTime);
    }

    protected abstract void Execute();

    private void Update()
    {
        Execute();
    }
}
