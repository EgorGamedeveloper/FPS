using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveCollectAndDeliverResource : Objective
    {
        [Min(1)]
        public int ResourceToDeliver = 5;

        [Tooltip("Zone where player should deliver resources")] public Collider DeliveryZone;

        bool m_PlayerInDeliveryZone;
        int m_DeliveredAmount;

        protected override void Start()
        {
            base.Start();

            if (string.IsNullOrEmpty(Title))
                Title = "Соберите и принесите ресурсы";

            UpdateObjective(string.Empty, GetCounterText(), string.Empty);

            if (PlayerMissionInventory.Instance != null)
            {
                PlayerMissionInventory.Instance.OnResourceCountChanged += OnResourceCountChanged;
            }
        }

        void Update()
        {
            if (IsCompleted || !m_PlayerInDeliveryZone || Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame)
                return;

            if (PlayerMissionInventory.Instance == null)
                return;

            int remaining = ResourceToDeliver - m_DeliveredAmount;
            int canDeposit = Mathf.Min(remaining, PlayerMissionInventory.Instance.CarriedResourceCount);
            if (canDeposit <= 0)
                return;

            if (PlayerMissionInventory.Instance.TryRemoveResource(canDeposit))
            {
                m_DeliveredAmount += canDeposit;
                if (m_DeliveredAmount >= ResourceToDeliver)
                {
                    CompleteObjective(string.Empty, GetCounterText(), "Objective complete : " + Title);
                    Destroy(this);
                }
                else
                {
                    UpdateObjective(string.Empty, GetCounterText(), "Ресурсы доставлены: " + GetCounterText());
                }
            }
        }

        void OnResourceCountChanged(int _)
        {
            if (!IsCompleted)
                UpdateObjective(string.Empty, GetCounterText(), string.Empty);
        }

        string GetCounterText() => m_DeliveredAmount + " / " + ResourceToDeliver;

        public void SetPlayerInDeliveryZone(bool inZone)
        {
            m_PlayerInDeliveryZone = inZone;
        }

        void OnDestroy()
        {
            if (PlayerMissionInventory.Instance != null)
                PlayerMissionInventory.Instance.OnResourceCountChanged -= OnResourceCountChanged;
        }
    }
}
