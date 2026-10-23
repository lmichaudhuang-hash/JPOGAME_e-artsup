using System;
using UnityEditor;

namespace JPO
{
    public static class EventBus
    {
        /// <summary>
        /// Event invoked when the player dies, add callbacks to this to react to the player death
        /// </summary>
        public static event Action playerDies;

        /// <summary>
        /// Event invoked when the player collects the key to unlock the exit of the level
        /// </summary>
        public static event Action keyCollected;

        /// <summary>
        /// Event invoked when the game is paused or resumed
        /// </summary>
        public static event Action<bool> gamePauseChanged;

        public static event Action<int> candyCollected;

        /// <summary>
        /// Invoke the playerDies event
        /// </summary>
        public static void OnPlayerDies()
        {
            playerDies?.Invoke();
        }

        /// <summary>
        /// Invoke the keyCollected event
        /// </summary>
        public static void OnKeyCollected()
        {
            keyCollected?.Invoke();
        }

        /// <summary>
        /// Invoke the gamePauseChanged event
        /// </summary>
        public static void OnGamePauseChanged(bool isPaused)
        {
            gamePauseChanged?.Invoke(isPaused);
        }

        public static void OnCandyCollected(int candyCount)
        {
            candyCollected?.Invoke(candyCount);
        }
    }
}