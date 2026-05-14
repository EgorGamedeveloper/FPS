using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveUnlockDoor : Objective
    {
        [Tooltip("Only doors with this id can complete the objective. Leave empty to accept any locked door.")]
        public string TargetDoorId;

        protected override void Start()
        {
            base.Start();
            LockedDoorController.OnDoorUnlocked += OnDoorUnlocked;
            UpdateObjective("Найдите спрятанный ключ и откройте дверь клавишей E", "0 / 1", string.Empty);
        }

        void OnDoorUnlocked(LockedDoorController door)
        {
            if (IsCompleted || door == null)
                return;

            if (!string.IsNullOrEmpty(TargetDoorId) && door.DoorId != TargetDoorId)
                return;

            CompleteObjective(string.Empty, "1 / 1", "Дверь открыта");
            Destroy(this);
        }

        void OnDestroy()
        {
            LockedDoorController.OnDoorUnlocked -= OnDoorUnlocked;
        }
    }
}
