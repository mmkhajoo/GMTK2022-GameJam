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
        if (_launcher == collision.gameObject)
            return;

        var hits = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);

        foreach (var hit in hits)
        {
            if(hit.TryGetComponent<IDamageable>(out var target))
            {
                if (target.CharacterType == _targetType)
                    target.TakeDamage(_damage);
            }
        }

        OnDestroy.Invoke();
        //Particle
        gameObject.SetActive(false);

    }

    protected override void Execute()
    {

    }
}
