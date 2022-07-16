using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    protected override void Execute()
    {
        transform.position += _direction * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_launcher == collision.gameObject)
            return;

        if (collision.TryGetComponent<IDamageable>(out var target))
        {
            if (target.CharacterType == _targetType)
                target.TakeDamage(_damage);

        }

        OnDestroy.Invoke();
        gameObject.SetActive(false);

    }
}
