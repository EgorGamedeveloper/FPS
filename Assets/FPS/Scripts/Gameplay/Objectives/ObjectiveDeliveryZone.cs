using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveDeliveryZone : MonoBehaviour
    {
        public ObjectiveCollectAndDeliverResource TargetObjective;

        void OnTriggerEnter(Collider other)
        {
            if (TargetObjective != null && other.GetComponent<PlayerCharacterController>() != null)
                TargetObjective.SetPlayerInDeliveryZone(true);
        }

        void OnTriggerExit(Collider other)
        {
            if (TargetObjective != null && other.GetComponent<PlayerCharacterController>() != null)
                TargetObjective.SetPlayerInDeliveryZone(false);
        }
    }
}
