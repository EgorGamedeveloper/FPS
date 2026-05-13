using System;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class PlayerMissionInventory : MonoBehaviour
    {
        public static PlayerMissionInventory Instance { get; private set; }

        public bool HasKey { get; private set; }
        public int CarriedResourceCount { get; private set; }

        public event Action<bool> OnKeyStateChanged;
        public event Action<int> OnResourceCountChanged;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void GiveKey()
        {
            if (HasKey)
                return;

            HasKey = true;
            OnKeyStateChanged?.Invoke(HasKey);
        }

        public bool TryConsumeKey()
        {
            if (!HasKey)
                return false;

            HasKey = false;
            OnKeyStateChanged?.Invoke(HasKey);
            return true;
        }

        public void AddResource(int amount)
        {
            CarriedResourceCount = Mathf.Max(0, CarriedResourceCount + amount);
            OnResourceCountChanged?.Invoke(CarriedResourceCount);
        }

        public bool TryRemoveResource(int amount)
        {
            if (amount <= 0 || CarriedResourceCount < amount)
                return false;

            CarriedResourceCount -= amount;
            OnResourceCountChanged?.Invoke(CarriedResourceCount);
            return true;
        }
    }
}
