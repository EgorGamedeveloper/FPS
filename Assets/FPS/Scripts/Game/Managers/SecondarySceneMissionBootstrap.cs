using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.FPS.Game
{
    public class SecondarySceneMissionBootstrap : MonoBehaviour
    {
        const string TargetSceneName = "SecondaryScene";
        const string BootstrapMarkerName = "__SecondarySceneMissionBootstrap";
        const string GameplayAssemblyName = "fps.Gameplay";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void EnsureSecondarySceneMission()
        {
            if (SceneManager.GetActiveScene().name != TargetSceneName)
                return;

            if (GameObject.Find(BootstrapMarkerName) != null)
                return;

            var marker = new GameObject(BootstrapMarkerName);
            marker.AddComponent<SecondarySceneMissionBootstrap>();
        }

        void Start() => SetupMission();

        void SetupMission()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;

            EnsureComponent(player, "Unity.FPS.Gameplay.PlayerMissionInventory");

            Vector3 forward = player.transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.01f)
                forward = Vector3.forward;
            forward.Normalize();
            Vector3 right = Vector3.Cross(Vector3.up, forward);

            GameObject objectiveRoot = new GameObject("MissionObjectives");
            Component keyObjective = EnsureComponent(objectiveRoot, "Unity.FPS.Gameplay.ObjectiveFindKey");
            SetStringField(keyObjective, "Title", "Найдите ключ");

            Component deliveryObjective = EnsureComponent(objectiveRoot, "Unity.FPS.Gameplay.ObjectiveCollectAndDeliverResource");
            SetStringField(deliveryObjective, "Title", "Соберите и доставьте ресурсы");
            SetIntField(deliveryObjective, "ResourceToDeliver", 3);

            CreatePickup("MissionKeyPickup", PrimitiveType.Cube, player.transform.position + forward * 8f + Vector3.up,
                new Vector3(0.6f, 0.2f, 0.6f), "Unity.FPS.Gameplay.KeyPickup");
            CreatePickup("MissionResourcePickupA", PrimitiveType.Sphere, player.transform.position + forward * 14f + right * 2f + Vector3.up,
                Vector3.one * 0.5f, "Unity.FPS.Gameplay.ResourcePickup", 1);
            CreatePickup("MissionResourcePickupB", PrimitiveType.Sphere, player.transform.position + forward * 16f - right * 2f + Vector3.up,
                Vector3.one * 0.5f, "Unity.FPS.Gameplay.ResourcePickup", 1);
            CreatePickup("MissionResourcePickupC", PrimitiveType.Sphere, player.transform.position + forward * 18f + Vector3.up,
                Vector3.one * 0.5f, "Unity.FPS.Gameplay.ResourcePickup", 1);

            GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            zone.name = "ResourceDeliveryZone";
            zone.transform.position = player.transform.position + forward * 5f;
            zone.transform.localScale = new Vector3(3f, 0.1f, 3f);
            Collider zoneCollider = zone.GetComponent<Collider>();
            zoneCollider.isTrigger = true;

            Component zoneBehavior = EnsureComponent(zone, "Unity.FPS.Gameplay.ObjectiveDeliveryZone");
            SetObjectField(zoneBehavior, "TargetObjective", deliveryObjective);
            SetObjectField(deliveryObjective, "DeliveryZone", zoneCollider);
        }

        static Component EnsureComponent(GameObject target, string typeName)
        {
            Type type = Type.GetType(typeName + ", " + GameplayAssemblyName);
            if (type == null)
                return null;

            Component existing = target.GetComponent(type);
            return existing != null ? existing : target.AddComponent(type);
        }

        void CreatePickup(string name, PrimitiveType primitiveType, Vector3 position, Vector3 scale, string pickupTypeName,
            int amount = 0)
        {
            GameObject pickup = GameObject.CreatePrimitive(primitiveType);
            pickup.name = name;
            pickup.transform.position = position;
            pickup.transform.localScale = scale;
            pickup.AddComponent<Rigidbody>();

            Component pickupComponent = EnsureComponent(pickup, pickupTypeName);
            if (amount > 0)
                SetIntField(pickupComponent, "Amount", amount);
        }

        static void SetStringField(Component component, string fieldName, string value)
        {
            if (component == null)
                return;

            var field = component.GetType().GetField(fieldName);
            if (field != null && field.FieldType == typeof(string))
                field.SetValue(component, value);
        }

        static void SetIntField(Component component, string fieldName, int value)
        {
            if (component == null)
                return;

            var field = component.GetType().GetField(fieldName);
            if (field != null && field.FieldType == typeof(int))
                field.SetValue(component, value);
        }

        static void SetObjectField(Component component, string fieldName, object value)
        {
            if (component == null || value == null)
                return;

            var field = component.GetType().GetField(fieldName);
            if (field != null && field.FieldType.IsInstanceOfType(value))
                field.SetValue(component, value);
        }
    }
}
