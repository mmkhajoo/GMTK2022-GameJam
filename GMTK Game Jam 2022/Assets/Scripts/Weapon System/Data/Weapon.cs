using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem
{
    [System.Serializable]
    public class Weapon
    {
        public WeaponType weaponType;
        public SubWeaponType subWeaponType;
        public int weaponDamage;
        public float fireRate;
        public float rangeDetect;
        public float projectileSpeed;
        public float explosionRadius;
        public GameObject weaponPrefab;
        public GameObject projectilePrefab;
    }
}