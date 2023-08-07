using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    private static ScoreSystem Instance;
    [SerializeField]
    private GameObject deathVFX;

    [SerializeField]
    private AudioSource Death;

    public int currentScore = 0;
    public int highScore = 0;                             //remove later since we will take this val from save sys 
    public HUDdata_SO scoreUpdate;
    public SaveData saveData;
    //HUD           #Start

    public TextMeshProUGUI scoreBar;
    public GameObject lifeBar;

    public GameObject DeathStateOverlay;
    public TextMeshProUGUI DeathTextBox;
    public TextMeshProUGUI DeathScoreBox;

    public GameObject followCamera;

    public GameObject RollOfTheDice;
    public GameObject Pause;
    public GameObject InputBar;
    //HUD           #End



    private void Awake()
    {
        scoreUpdate.currentScore = 0;
        scoreUpdate.livesLeft = 3;
        Instance = this;

        DeathStateOverlay.SetActive(false);
    }
    private void Start()
    {
        UpdateScores(0);
        highScore = saveData.highScore;
    }
    public static ScoreSystem GetInstance()
    {
        return Instance;
    }

    public void UpdateScores(int scoreUpdated)
    {
        currentScore += scoreUpdated;
        scoreUpdate.currentScore = currentScore;
        scoreBar.text = scoreUpdate.currentScore.ToString();
    }

    internal bool visualLeft = false;
    internal bool notAFall = false;
    public void UpdateLife(int life)
    {
        scoreUpdate.livesLeft -= life;

        if (scoreUpdate.livesLeft == 2)
        {
            lifeBar.transform.GetChild(2).gameObject.SetActive(false);
            notAFall = false;
        }
        else if (scoreUpdate.livesLeft == 1)
        {
            lifeBar.transform.GetChild(2).gameObject.SetActive(false);
            lifeBar.transform.GetChild(1).gameObject.SetActive(false);
            EventSystem.GetInstance().RollAvailable = true;
            notAFall = false;
        }
        else if (scoreUpdate.livesLeft <= 0)
        {
            StartCoroutine(RemoveHealth());

            if (visualLeft || notAFall)
            {
                StartCoroutine(DeathScreen(highScore));
            }
        }
    }

    private IEnumerator RemoveHealth()
    {
        lifeBar.transform.GetChild(2).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        lifeBar.transform.GetChild(1).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        lifeBar.transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.05f);
    }

    public IEnumerator DeathScreen(int highScore)
    {
        Pause.SetActive(false);
        RollOfTheDice.SetActive(false);
        InputBar.SetActive(false);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = false;

        GameObject DeathVFX = Instantiate(deathVFX);
        DeathVFX.transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
        Death.Play();
        yield return new WaitUntil(() => !DeathVFX.GetComponent<ParticleSystem>().isPlaying);

        DeathStateOverlay.SetActive(true);
        yield return new WaitForSeconds(0.05f);

        if(currentScore>highScore)
            saveData.highScore = currentScore;

        if (currentScore > highScore)
        {
            StartCoroutine(TypeWordByWord(0.1f, "New High Score :"));
            StartCoroutine(TypeIntByInt(0.1f, currentScore));
        }
        else if (currentScore <= highScore)
        {
            StartCoroutine(TypeWordByWord(0.01f, "Your Score :"));
            StartCoroutine(TypeIntByInt(0.001f, currentScore));
        }
    }

    private IEnumerator TypeWordByWord(float timeGap, string output)
    {
        DeathTextBox.text = "";
        for (int i = 0; i < output.Length; i++)
        {
            DeathTextBox.text += output[i];
            yield return new WaitForSeconds(timeGap);
        }
    }

    private IEnumerator TypeIntByInt(float timeGap, int output)
    {
        DeathScoreBox.text = "";
        for (int i = 0; i <= output; i++)
        {
            DeathScoreBox.text = i.ToString();
            yield return null;
        }
    }
}
