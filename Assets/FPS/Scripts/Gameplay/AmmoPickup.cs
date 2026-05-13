using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class AmmoPickup : Pickup
    {
        [Tooltip("Optional weapon prefab whose ammo type should be refilled")]
        public WeaponController Weapon;

        [Tooltip("Ammo type to refill when no weapon prefab is assigned")]
        public string AmmoType = "Default";

        [Tooltip("Number of rounds added to the player's carried ammo reserve")]
        public int BulletCount = 30;

        protected override void OnPicked(PlayerCharacterController byPlayer)
        {
            PlayerWeaponsManager playerWeaponsManager = byPlayer.GetComponent<PlayerWeaponsManager>();
            if (playerWeaponsManager == null)
                return;

            string ammoType = Weapon != null ? Weapon.AmmoType : AmmoType;
            int ammoAdded = playerWeaponsManager.AddAmmo(ammoType, BulletCount);
            if (ammoAdded <= 0)
                return;

            WeaponController eventWeapon = Weapon != null ? playerWeaponsManager.HasWeapon(Weapon) : FindWeaponForAmmo(playerWeaponsManager, ammoType);
            AmmoPickupEvent evt = Events.AmmoPickupEvent;
            evt.Weapon = eventWeapon;
            evt.AmmoType = ammoType;
            evt.Amount = ammoAdded;
            EventManager.Broadcast(evt);

            PlayPickupFeedback();
            Destroy(gameObject);
        }

        WeaponController FindWeaponForAmmo(PlayerWeaponsManager playerWeaponsManager, string ammoType)
        {
            for (int i = 0; i < 9; i++)
            {
                WeaponController weapon = playerWeaponsManager.GetWeaponAtSlotIndex(i);
                if (weapon != null && weapon.AmmoMatches(ammoType))
                    return weapon;
            }

            return null;
        }
    }
}
