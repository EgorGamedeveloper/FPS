using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.FPS.Gameplay
{
    public class ThirdSceneMissionBootstrap : MonoBehaviour
    {
        const string TargetSceneName = "ThirdScene";
        const string BootstrapMarkerName = "__ThirdSceneMissionBootstrap";
        const string DoorId = "ThirdSceneExitDoor";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void EnsureThirdSceneMission()
        {
            if (SceneManager.GetActiveScene().name != TargetSceneName)
                return;

            if (GameObject.Find(BootstrapMarkerName) != null)
                return;

            var marker = new GameObject(BootstrapMarkerName);
            marker.AddComponent<ThirdSceneMissionBootstrap>();
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
            Vector3 startPosition = player.transform.position;

            var objectiveRoot = new GameObject("ThirdSceneObjectives");

            var keyObjective = objectiveRoot.AddComponent<ObjectiveFindKey>();
            keyObjective.Title = "Найдите спрятанный ключ";
            keyObjective.Description = "Ключ спрятан рядом с дверью.";

            var doorObjective = objectiveRoot.AddComponent<ObjectiveUnlockDoor>();
            doorObjective.Title = "Откройте дверь";
            doorObjective.Description = "Подберите ключ и нажмите E у двери.";
            doorObjective.TargetDoorId = DoorId;

            var resourceObjective = objectiveRoot.AddComponent<ObjectiveCollectAndDeliverResource>();
            resourceObjective.Title = "Соберите ресурсы";
            resourceObjective.Description = "Соберите 5 единиц ресурсов и принесите их в точку спавна.";
            resourceObjective.ResourceToDeliver = 5;

            CreateDoor(startPosition + forward * 12f + Vector3.up * 1.5f, forward);
            CreateHiddenKey(startPosition + forward * 8f + right * 4f + Vector3.up * 0.6f, right);
            CreateResourcePickups(startPosition, forward, right);
            CreateSpawnDeliveryZone(startPosition, resourceObjective);
        }

        void CreateDoor(Vector3 position, Vector3 forward)
        {
            GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = "ThirdSceneLockedDoor";
            door.transform.position = position;
            door.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            door.transform.localScale = new Vector3(3.5f, 3f, 0.35f);
            SetColor(door, new Color(0.35f, 0.18f, 0.08f));

            var doorCollider = door.GetComponent<Collider>();
            if (doorCollider != null)
                doorCollider.isTrigger = false;

            var doorTrigger = new GameObject("ThirdSceneDoorInteractionTrigger");
            doorTrigger.transform.position = position - forward * 1.1f;
            doorTrigger.transform.rotation = door.transform.rotation;
            doorTrigger.transform.localScale = new Vector3(4.5f, 3.25f, 2.25f);
            var triggerCollider = doorTrigger.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;

            var doorController = doorTrigger.AddComponent<LockedDoorController>();
            doorController.DoorId = DoorId;
            doorController.DoorVisual = door.transform;
            doorController.OpenOffset = Vector3.up * 3.5f;
        }

        void CreateHiddenKey(Vector3 position, Vector3 right)
        {
            GameObject hidingCrate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hidingCrate.name = "ThirdSceneKeyHidingCrate";
            hidingCrate.transform.position = position + right * 0.55f + Vector3.up * 0.45f;
            hidingCrate.transform.localScale = new Vector3(1.5f, 0.9f, 1.5f);
            SetColor(hidingCrate, new Color(0.25f, 0.16f, 0.08f));

            GameObject key = GameObject.CreatePrimitive(PrimitiveType.Cube);
            key.name = "ThirdSceneHiddenKeyPickup";
            key.transform.position = position - right * 0.55f;
            key.transform.localScale = new Vector3(0.55f, 0.15f, 0.55f);
            SetColor(key, new Color(1f, 0.82f, 0.12f));
            key.AddComponent<Rigidbody>();
            key.AddComponent<KeyPickup>();
        }

        void CreateResourcePickups(Vector3 startPosition, Vector3 forward, Vector3 right)
        {
            CreateResourcePickup("ThirdSceneResourcePickupA", startPosition + forward * 4f - right * 3f + Vector3.up * 0.6f);
            CreateResourcePickup("ThirdSceneResourcePickupB", startPosition + forward * 6f + right * 2.5f + Vector3.up * 0.6f);
            CreateResourcePickup("ThirdSceneResourcePickupC", startPosition + forward * 9f - right * 2f + Vector3.up * 0.6f);
            CreateResourcePickup("ThirdSceneResourcePickupD", startPosition + forward * 11f + right * 3f + Vector3.up * 0.6f);
            CreateResourcePickup("ThirdSceneResourcePickupE", startPosition + forward * 14f + Vector3.up * 0.6f);
        }

        void CreateResourcePickup(string name, Vector3 position)
        {
            GameObject resource = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            resource.name = name;
            resource.transform.position = position;
            resource.transform.localScale = Vector3.one * 0.55f;
            SetColor(resource, new Color(0.15f, 0.85f, 1f));
            resource.AddComponent<Rigidbody>();
            ResourcePickup pickup = resource.AddComponent<ResourcePickup>();
            pickup.Amount = 1;
        }

        void CreateSpawnDeliveryZone(Vector3 spawnPosition, ObjectiveCollectAndDeliverResource resourceObjective)
        {
            GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            zone.name = "ThirdSceneSpawnResourceDeliveryZone";
            zone.transform.position = spawnPosition + Vector3.up * 0.05f;
            zone.transform.localScale = new Vector3(3.5f, 0.1f, 3.5f);
            SetColor(zone, new Color(0.1f, 0.9f, 0.25f));

            Collider zoneCollider = zone.GetComponent<Collider>();
            zoneCollider.isTrigger = true;

            ObjectiveDeliveryZone deliveryZone = zone.AddComponent<ObjectiveDeliveryZone>();
            deliveryZone.TargetObjective = resourceObjective;
            resourceObjective.DeliveryZone = zoneCollider;
        }

        static void SetColor(GameObject target, Color color)
        {
            var renderer = target.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = color;
        }
    }
}
