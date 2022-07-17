using System;
using System.Collections;
using System.Collections.Generic;
using Managers.Audio_Manager;
using UnityEngine;
using WeaponSystem;

public class Bullet : Projectile
{
    
    protected override void Execute()
    {
        if(!_isStoped)
            transform.position += _direction * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_launcher == collision.gameObject)
            return;

        if (collision.TryGetComponent<Projectile>(out var projectile))
            return;

        if (collision.TryGetComponent<IDamageable>(out var target))
        {
            if (target.CharacterType == _targetType && _canDamage)
            {
                target.TakeDamage(_damage);
                _canDamage = false;
            }
        }
        _isStoped = true;
        Destroy(gameObject, _destroyTime);
        OnDestroy.Invoke();
    }
    
}
