using System;
using GamePush;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.FPS.Game
{
    public static class LevelProgressionManager
    {
        public const string CompletedLevelsFieldKey = "completedLevels";

        public static readonly string[] LevelSceneNames =
        {
            "MainScene",
            "SecondaryScene",
            "ThirdScene"
        };

        public static int LevelCount => LevelSceneNames.Length;

        public static int GetCompletedLevels()
        {
            int completedLevels = Mathf.Max(0, GP_Player.GetInt(CompletedLevelsFieldKey));
            return Mathf.Clamp(completedLevels, 0, LevelCount);
        }

        public static int GetLevelNumber(string sceneName)
        {
            int levelIndex = GetLevelIndex(sceneName);
            return levelIndex >= 0 ? levelIndex + 1 : 0;
        }

        public static bool IsLevelUnlocked(int levelIndex)
        {
            return levelIndex >= 0 && levelIndex < LevelCount && levelIndex <= GetCompletedLevels();
        }

        public static void StartBestAvailableLevel()
        {
            int levelIndex = Mathf.Clamp(GetCompletedLevels(), 0, LevelCount - 1);
            LoadLevel(levelIndex);
        }

        public static void StartLevel(int levelNumber)
        {
            LoadLevel(levelNumber - 1);
        }

        public static void LoadLevel(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= LevelCount)
            {
                Debug.LogWarning($"Level index {levelIndex} is outside configured levels.");
                return;
            }

            if (!IsLevelUnlocked(levelIndex))
            {
                Debug.Log($"Level {levelIndex + 1} is locked. Completed levels: {GetCompletedLevels()}.");
                return;
            }

            SceneManager.LoadScene(LevelSceneNames[levelIndex]);
        }

        public static string CompleteCurrentLevelAndGetNextScene(string fallbackWinSceneName)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            int currentLevelIndex = GetLevelIndex(currentSceneName);

            if (currentLevelIndex < 0)
                return fallbackWinSceneName;

            SaveCompletedLevels(Mathf.Max(GetCompletedLevels(), currentLevelIndex + 1));

            int nextLevelIndex = currentLevelIndex + 1;
            if (nextLevelIndex < LevelCount)
                return LevelSceneNames[nextLevelIndex];

            return fallbackWinSceneName;
        }

        public static void SaveCompletedLevels(int completedLevels)
        {
            int clampedCompletedLevels = Mathf.Clamp(completedLevels, 0, LevelCount);
            GP_Player.Set(CompletedLevelsFieldKey, clampedCompletedLevels);
            GP_Player.Sync(SyncStorageType.preferred);
        }

        public static int GetLevelIndex(string sceneName)
        {
            return Array.IndexOf(LevelSceneNames, sceneName);
        }
    }
}
