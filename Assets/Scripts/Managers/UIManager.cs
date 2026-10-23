using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
namespace JPO
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Black Screen")]
        [SerializeField] private Image blackScreen;
        [SerializeField] private float screenFadeDuration = 0.5f;
        public float ScreenFadeDuration => screenFadeDuration;

        [Header("Welcome Text")]
        [SerializeField] private TMP_Text welcomeText;
        [SerializeField] private float delayBeforeTextFade = 0.5f;
        [SerializeField] private float textFadeDuration = 0.5f;

        [Header("Pause Menu")]
        [SerializeField] private CanvasGroup pauseMenu;
        [SerializeField] private GameObject pauseFirstButton;
        [SerializeField] private string mainMenuSceneName = "MenuScene";

        private Tween _screenFadeTween;
        private Sequence _textSeq;

        private void Awake()
        {
            Instance = this;
            EventBus.gamePauseChanged += HandleGamePauseChanged;
            EventBus.candyCollected += UpdateCandyText;
        }

        private void Start()
        {
            ClosePauseMenu();
            HideBlackScreen();
            FadeWelcomeText();
            UpdateCandyText(0);
        }

        private void OnDestroy()
        {
            _screenFadeTween?.Kill();
            _textSeq?.Kill();
            EventBus.gamePauseChanged -= HandleGamePauseChanged;
            EventBus.candyCollected -= UpdateCandyText;
        }

        #region Black Screen
        /// <summary>
        /// Show the black screen by fading it in
        /// </summary>
        public void ShowBlackScreen()
        {
            // Force image alpha to 0
            blackScreen.SetAlpha(0f);
            // Enable the image and fade it to 1
            blackScreen.gameObject.SetActive(true);
            _screenFadeTween = blackScreen.DOFade(1f, screenFadeDuration).SetEase(Ease.Linear);
        }

        /// <summary>
        /// Hide the black screen by fading it out
        /// </summary>
        public void HideBlackScreen()
        {
            // Force image alpha to 1
            blackScreen.SetAlpha(1f);
            blackScreen.gameObject.SetActive(true);
            // Fade image to 0 then disable its object
            _screenFadeTween = blackScreen.DOFade(0f, screenFadeDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => blackScreen.gameObject.SetActive(false));
        }
        #endregion

        #region Welcome Text
        /// <summary>
        /// Fade out the welcome text
        /// </summary>
        private void FadeWelcomeText()
        {
            welcomeText.SetAlpha(1f);
            welcomeText.gameObject.SetActive(true);
            _textSeq?.Kill();
            _textSeq = DOTween.Sequence().SetEase(Ease.Linear);
            _textSeq.AppendInterval(delayBeforeTextFade);
            _textSeq.Append(welcomeText.DOFade(0f, textFadeDuration));
            _textSeq.OnComplete(() => welcomeText.gameObject.SetActive(false));
            _textSeq.Play();
        }
        #endregion

        #region Pause Menu
        /// <summary>
        /// Handles the game paused change
        /// </summary>
        private void HandleGamePauseChanged(bool isPaused)
        {
            if (isPaused) OpenPauseMenu();
            else ClosePauseMenu();
        }

        /// <summary>
        /// Open the pause menu
        /// </summary>
        private void OpenPauseMenu()
        {
            // Show the pause menu UI
            pauseMenu.gameObject.SetActive(true);
            pauseMenu.interactable = true;

            // Select the first button
            EventSystem.current.SetSelectedGameObject(pauseFirstButton);
        }

        /// <summary>
        /// Close the pause menu
        /// </summary>
        private void ClosePauseMenu()
        {
            pauseMenu.gameObject.SetActive(false);
            pauseMenu.interactable = false;
        }

        /// <summary>
        /// Resume Game
        /// </summary>
        public void ResumeGame()
        {
            GameManager.Instance.ResumeGame();
        }

        /// <summary>
        /// Back to main menu
        /// </summary>
        public void BackToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }
        #endregion

        #region Bonbons
       
        public TMP_Text candyText;

        private void UpdateCandyText(int candyCount)
        {
            candyText.text = $"BONBONS: {candyCount:00}";
        }
      


        #endregion
    }
}