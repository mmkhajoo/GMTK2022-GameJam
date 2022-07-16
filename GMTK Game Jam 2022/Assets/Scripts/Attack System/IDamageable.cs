using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    CharacterType CharacterType { get; }

    void TakeDamage(int hitDamage);
}
