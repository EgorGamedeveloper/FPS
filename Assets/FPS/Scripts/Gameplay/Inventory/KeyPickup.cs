using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class KeyPickup : Pickup
    {
        protected override void OnPicked(PlayerCharacterController playerController)
        {
            base.OnPicked(playerController);

            if (PlayerMissionInventory.Instance != null)
            {
                PlayerMissionInventory.Instance.GiveKey();
            }

            Destroy(gameObject);
        }
    }
}
