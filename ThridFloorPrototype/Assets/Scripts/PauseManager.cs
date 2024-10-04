using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public Transform targetTransform;
    public Vector3 targetScale = Vector3.one;

    public bool isPaused = false;

    void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        SoundManager.Instance.ClickSound();

        Time.timeScale = 0; 
        pausePanel.SetActive(true);
        targetTransform.localScale = Vector3.zero;
        targetTransform.DOScale(targetScale, 0.1f).SetUpdate(true).OnComplete(() =>
        {
            Debug.Log("Pause animation complete");
        });
    }

    public void ResumeGame()
    {
        SoundManager.Instance.ClickSound();

        Time.timeScale = 1; 
        pausePanel.SetActive(false);
    }
    public void PlayAgain()
    {
        SoundManager.Instance.ClickSound();

        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }

    public void BackToMainMenu()
    {
        SoundManager.Instance.ClickSound();

        Time.timeScale = 1; 
        SceneManager.LoadScene("MainMenu");
    }
    public void BackToMainMenuAfterWinOrLose()
    {
        SoundManager.Instance.ClickSound();

        Time.timeScale = 1;

        AdsManager.Instance.ShowInterstitialAd(() =>
        {
            SceneManager.LoadScene("MainMenu");
        });
    }
}
