using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.FPS.Game
{
    public class SecondarySceneMissionBootstrap : MonoBehaviour
    {
        const string TargetSceneName = "SecondaryScene";
        const string BootstrapMarkerName = "__SecondarySceneMissionBootstrap";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void EnsureSecondarySceneMission()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name != TargetSceneName)
                return;

            if (GameObject.Find(BootstrapMarkerName) != null)
                return;

            var marker = new GameObject(BootstrapMarkerName);
            marker.AddComponent<SecondarySceneMissionBootstrap>();
        }

        void Start()
        {
            SetupMission();
        }

        void SetupMission()
        {
            PlayerCharacterController player = FindObjectOfType<PlayerCharacterController>();
            if (player == null)
                return;

            if (player.GetComponent<PlayerMissionInventory>() == null)
                player.gameObject.AddComponent<PlayerMissionInventory>();

            Vector3 forward = player.transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.01f)
                forward = Vector3.forward;
            forward.Normalize();
            Vector3 right = Vector3.Cross(Vector3.up, forward);

            var objectiveRoot = new GameObject("MissionObjectives");

            var keyObjective = objectiveRoot.AddComponent<ObjectiveFindKey>();
            keyObjective.Title = "Найдите ключ";

            var deliveryObjective = objectiveRoot.AddComponent<ObjectiveCollectAndDeliverResource>();
            deliveryObjective.Title = "Соберите и доставьте ресурсы";
            deliveryObjective.ResourceToDeliver = 3;

            CreateKeyPickup(player.transform.position + forward * 8f + Vector3.up);
            CreateResourcePickup(player.transform.position + forward * 14f + right * 2f + Vector3.up, 1);
            CreateResourcePickup(player.transform.position + forward * 16f - right * 2f + Vector3.up, 1);
            CreateResourcePickup(player.transform.position + forward * 18f + Vector3.up, 1);

            GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            zone.name = "ResourceDeliveryZone";
            zone.transform.position = player.transform.position + forward * 5f;
            zone.transform.localScale = new Vector3(3f, 0.1f, 3f);
            var zoneCollider = zone.GetComponent<Collider>();
            zoneCollider.isTrigger = true;

            var zoneBehaviour = zone.AddComponent<ObjectiveDeliveryZone>();
            zoneBehaviour.TargetObjective = deliveryObjective;
            deliveryObjective.DeliveryZone = zoneCollider;
        }

        void CreateKeyPickup(Vector3 position)
        {
            var key = GameObject.CreatePrimitive(PrimitiveType.Cube);
            key.name = "MissionKeyPickup";
            key.transform.position = position;
            key.transform.localScale = new Vector3(0.6f, 0.2f, 0.6f);
            key.AddComponent<Rigidbody>();
            key.AddComponent<KeyPickup>();
        }

        void CreateResourcePickup(Vector3 position, int amount)
        {
            var resource = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            resource.name = "MissionResourcePickup";
            resource.transform.position = position;
            resource.transform.localScale = Vector3.one * 0.5f;
            resource.AddComponent<Rigidbody>();
            var pickup = resource.AddComponent<ResourcePickup>();
            pickup.Amount = amount;
        }
    }
}
