using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ResourcePickup : Pickup
    {
        [Min(1)]
        public int Amount = 1;

        protected override void OnPicked(PlayerCharacterController playerController)
        {
            base.OnPicked(playerController);

            if (PlayerMissionInventory.Instance != null)
            {
                PlayerMissionInventory.Instance.AddResource(Amount);
            }

            Destroy(gameObject);
        }
    }
}
