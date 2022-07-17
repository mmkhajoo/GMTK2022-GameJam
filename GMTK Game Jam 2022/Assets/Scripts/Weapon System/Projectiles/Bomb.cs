using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

public class Bomb : Projectile
{
    private float _explosionRadius;
    private Rigidbody2D _rigidbody2D;

    public override void Setup(Vector3 target, Weapon weapon, CharacterType targetType, GameObject launcher)
    {
        base.Setup(target, weapon, targetType, launcher);

        _explosionRadius = weapon.explosionRadius;
        _rigidbody2D = GetComponent<Rigidbody2D>();

        _rigidbody2D.AddForce(_direction * _speed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_launcher == collision.gameObject || collision.gameObject == gameObject)
            return;

        if (collision.TryGetComponent<Projectile>(out var projectile))
            return;

        var hits = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);

        foreach (var hit in hits)
        {
            if(hit.TryGetComponent<IDamageable>(out var target))
            {
                if (target.CharacterType == _targetType && _canDamage)
                {
                    target.TakeDamage(_damage);
                    _canDamage = false;
                }
            }
        }

        _isStoped = true;
        Destroy(gameObject, _destroyTime);
        OnDestroy.Invoke();
    }

    protected override void Execute()
    {
        if (_isStoped)
        {
            _rigidbody2D.isKinematic = true;
            _rigidbody2D.velocity = Vector2.zero;
        }
    }
}
