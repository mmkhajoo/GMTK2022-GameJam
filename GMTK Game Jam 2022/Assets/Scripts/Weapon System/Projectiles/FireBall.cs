using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

public class FireBall : Projectile
{
    protected override void Execute()
    {
        if (!_isStoped)
            transform.position += _direction * _speed * Time.deltaTime;
    }

    public override void Setup(Vector3 target, Weapon weapon, CharacterType targetType, GameObject launcher)
    {
        base.Setup(target, weapon, targetType, launcher);

        var angle = MathF.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        if (angle < 0)
            angle += 360;

        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_launcher == collision.gameObject || collision.gameObject == gameObject)
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
