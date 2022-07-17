using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WeaponSystem
{
    [CreateAssetMenu(menuName = "Character/Weapon", fileName = "WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public CharacterType characterType;
        public List<Weapon> weapons;

        public Weapon GetWeapon(SubWeaponType subWeaponType)
        {
            return weapons.Where(a => a.subWeaponType == subWeaponType).FirstOrDefault();
        }
    }
}