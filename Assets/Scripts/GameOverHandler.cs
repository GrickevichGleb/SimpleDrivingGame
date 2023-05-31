using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] private GameObject player = null;
    [SerializeField] private ScoreSystem scoreSystem = null;
    [SerializeField] private Button continueButton = null;
    [SerializeField] private TMP_Text gameOverText = null;
    [SerializeField] private Canvas gameOverTab = null;

    [SerializeField] private TMP_Text addDebugText = null;
    
    // Singleton AddManager !

    public void EndGame()
    {
        scoreSystem.StopScoreSystem();
        gameOverText.text = $"Game Over\nYour score: {scoreSystem.GetScore()}";
        gameOverTab.gameObject.SetActive(true);
    }
    
    public void PlayAgain()
    {
        SceneManager.LoadScene(1); //Game Scene
    }
    
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0); //MainMenu Scene
    }
    
    
    public void ContinueButton()
    {
        AddManager.Instance.ShowAdd(this);
        continueButton.interactable = false;

        StartCoroutine(GetAddDebugData());
    }

    public void ContinueGame()
    {
        gameOverTab.gameObject.SetActive(false);
        player.GetComponent<CarGameplay>().ContinueGame();
    }

    private IEnumerator GetAddDebugData()
    {
        yield return new WaitForSeconds(5f);

        //addDebugText.text = AddManager.Instance.debugText;
    }
}
