using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveFindKey : Objective
    {
        protected override void Start()
        {
            base.Start();
            EventManager.AddListener<PickupEvent>(OnPickupEvent);
            UpdateObjective("", "0 / 1", string.Empty);
        }

        void OnPickupEvent(PickupEvent evt)
        {
            if (IsCompleted)
                return;

            if (evt.Pickup != null && evt.Pickup.GetComponent<KeyPickup>() != null)
            {
                CompleteObjective(string.Empty, "1 / 1", "Objective complete : " + Title);
                Destroy(this);
            }
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<PickupEvent>(OnPickupEvent);
        }
    }
}
