using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem
{
    [CreateAssetMenu(menuName = "Character/Weapon", fileName = "WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public CharacterType characterType;
        public List<Weapon> weapons;
    }
}