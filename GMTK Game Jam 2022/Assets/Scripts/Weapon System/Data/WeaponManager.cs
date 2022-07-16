using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WeaponSystem
{
    [CreateAssetMenu(menuName = "Manager/WepaonManager", fileName = "weaponManage")]
    public class WeaponManager : ScriptableObject
    {
        public List<WeaponData> weaponData;


        public WeaponData Get(CharacterType characterType)
        {
            try
            {
                var weapon = weaponData.Where(a => a.characterType == characterType).FirstOrDefault();
                return weapon;
            }
            catch
            {
                Debug.LogError($"There is no weapon data with character type : {characterType}");
                return null;
            }
        }
    }
}