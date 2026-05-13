using GamePush;
using TMPro;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class MenuNavigation : MonoBehaviour
    {
        public Selectable DefaultSelection;

        private InputAction m_SubmitAction;
        private InputAction m_NavigateAction;
        private bool m_LevelButtonsCreated;
        
        async void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            EventSystem.current.SetSelectedGameObject(null);

            m_SubmitAction = InputSystem.actions.FindAction("UI/Submit");
            m_NavigateAction  = InputSystem.actions.FindAction("UI/Navigate");

            if (SceneManager.GetActiveScene().name == "IntroMenu")
            {
                await GP_Init.Ready;
                SetupMainMenuLevelSelection();
            }
        }

        void LateUpdate()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (m_SubmitAction.WasPressedThisFrame()
                    || m_NavigateAction.ReadValue<Vector2>().sqrMagnitude != 0 )
                {
                    EventSystem.current.SetSelectedGameObject(DefaultSelection.gameObject);
                }
            }
        }

        void SetupMainMenuLevelSelection()
        {
            if (m_LevelButtonsCreated || DefaultSelection == null)
                return;

            m_LevelButtonsCreated = true;

            ConfigureButton(DefaultSelection, "Continue", LevelProgressionManager.StartBestAvailableLevel);
            SetButtonPosition(DefaultSelection.GetComponent<RectTransform>(), 0f, 25f, 380f, 60f);

            RectTransform templateRect = DefaultSelection.GetComponent<RectTransform>();
            Transform parent = templateRect.parent;

            for (int i = 0; i < LevelProgressionManager.LevelCount; i++)
            {
                int levelNumber = i + 1;
                Selectable levelButton = Instantiate(DefaultSelection, parent);
                levelButton.name = $"Level{levelNumber}Button";
                ConfigureButton(levelButton, $"Level {levelNumber}", () => LevelProgressionManager.StartLevel(levelNumber));
                levelButton.interactable = LevelProgressionManager.IsLevelUnlocked(i);
                SetButtonPosition(levelButton.GetComponent<RectTransform>(), -100f + i * 200f, 95f, 180f, 45f);
            }

            MoveControlsButton(parent);
        }

        static void ConfigureButton(Selectable selectable, string text, UnityEngine.Events.UnityAction onClick)
        {
            TMP_Text label = selectable.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
            {
                label.text = text;
            }

            LoadSceneButton loadSceneButton = selectable.GetComponent<LoadSceneButton>();
            if (loadSceneButton != null)
            {
                loadSceneButton.enabled = false;
            }

            Button button = selectable as Button;
            if (button == null)
                return;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(onClick);
        }

        static void SetButtonPosition(RectTransform rectTransform, float x, float y, float width, float height)
        {
            if (rectTransform == null)
                return;

            rectTransform.anchoredPosition = new Vector2(x, y);
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        static void MoveControlsButton(Transform parent)
        {
            Transform controlsButton = parent.Find("ControlsButton");
            if (controlsButton == null)
                return;

            SetButtonPosition(controlsButton.GetComponent<RectTransform>(), 0f, 155f, 380f, 30f);
        }
    }
}
