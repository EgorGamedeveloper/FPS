using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.FPS.Gameplay
{
    public class LockedDoorController : MonoBehaviour
    {
        public static event Action<LockedDoorController> OnDoorUnlocked;

        public string DoorId;
        public Transform DoorVisual;
        public Vector3 OpenOffset = new Vector3(0f, 3f, 0f);
        public float OpenSpeed = 3f;

        Vector3 m_ClosedPosition;
        Vector3 m_OpenPosition;
        bool m_PlayerInRange;
        bool m_IsOpen;
        bool m_HasBroadcastUnlock;

        void Start()
        {
            if (DoorVisual == null)
                DoorVisual = transform;

            m_ClosedPosition = DoorVisual.position;
            m_OpenPosition = m_ClosedPosition + OpenOffset;
        }

        void Update()
        {
            if (!m_IsOpen && m_PlayerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (PlayerMissionInventory.Instance != null && PlayerMissionInventory.Instance.TryConsumeKey())
                {
                    UnlockDoor();
                }
            }

            Vector3 target = m_IsOpen ? m_OpenPosition : m_ClosedPosition;
            DoorVisual.position = Vector3.Lerp(DoorVisual.position, target, Time.deltaTime * OpenSpeed);
        }

        void UnlockDoor()
        {
            if (m_IsOpen)
                return;

            m_IsOpen = true;
            if (!m_HasBroadcastUnlock)
            {
                m_HasBroadcastUnlock = true;
                OnDoorUnlocked?.Invoke(this);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerCharacterController>() != null)
                m_PlayerInRange = true;
        }

        void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<PlayerCharacterController>() != null)
                m_PlayerInRange = false;
        }
    }
}
