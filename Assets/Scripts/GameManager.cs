using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{ 
    public PlayerController player;
    public GameObject pauseMenu;
    public GameObject creaturesMainList;
    public GameObject fullSizeView;
    [HideInInspector]
    public bool releasingAnimationGoing = false;

    public readonly Color32 whiteColor = new Color32(0xff, 0xff, 0xff, 0xff);
    public readonly Color32 redColor = new Color32(0xfd, 0x28, 0x1b, 0xff);
    public readonly Color32 greenColor = new Color32(0x4b, 0xff, 0x47, 0xff);
    public readonly Color32 blueColor = new Color32(0x32, 0x66, 0xff, 0xff);

    public Coroutine curMinigame = null;
    Coroutine showLoseCoroutine = null;
    bool endingPhase = false;

    public GameObject reactionMinigame;
    public Image reactIndicator;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI roundTimeText;
    public TextMeshProUGUI sumTimeText;
    public TextMeshProUGUI reactMessageText;
    public TextMeshProUGUI reactRoundCounterText;
    public int minWait;
    public int maxWait;
    bool reactWaiting = false;
    bool reactReady = false;
    float roundTime = 0;
    float sumTime = 0;
    float maxTime = 0;

    public GameObject memoryMinigame;
    public TextMeshProUGUI memoryMessageText;
    public TextMeshProUGUI memoryRoundCounterText;
    public GameObject grid4x4;
    public GameObject grid5x5;
    public GameObject grid6x6;
    GameObject curGrid;
    int totalButtons = 0;
    int highlightedButtons = 0;
    int guessedButtons = 0;
    bool memoryReady = false;
    List<GameObject> memoryButtons = new List<GameObject>();
    List<GameObject> correctButtons = new List<GameObject>();

    public AudioSource musicSource;
    public AudioSource effectsSource;
    public AudioClip flyMusicClip;
    public AudioClip gladiatorsMusicClip;
    public AudioClip gameBeginsClip;
    public AudioClip countdownTickClip;
    public AudioClip keyAcquiredClip;
    public AudioClip unlockClip;
    public AudioClip memoryFadeClip;
    public AudioClip minigameVictoryClip;
    public AudioClip loseClip;

	private void Start()
	{
        effectsSource.PlayOneShot(gameBeginsClip, 0.3f);
	}

	void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape) && !endingPhase && !releasingAnimationGoing)
		{
            if (curMinigame != null)
            {
                StopCoroutine(curMinigame);
                if (showLoseCoroutine != null)
                {
                    StopCoroutine(showLoseCoroutine);
                    showLoseCoroutine = null;
                }
                reactionMinigame.SetActive(false);
                reactWaiting = false;
                reactReady = false;
                memoryMinigame.SetActive(false);
                memoryReady = false;
                player.playingMinigame = false;
                curMinigame = null;
                player.audioSource.UnPause();
                musicSource.clip = flyMusicClip;
                musicSource.Play();
                player.interactionText.gameObject.SetActive(true);
                player.interactionReady = true;
            }
            else if (fullSizeView.activeSelf)
                CloseFullSizeView();
            else if (creaturesMainList.activeSelf)
                CloseCreaturesMenu();
            else if (!pauseMenu.activeSelf)
                PauseGame(true);
            else if(pauseMenu.activeSelf)
                ResumeGame();
        }

        if (reactWaiting && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            StopCoroutine(curMinigame);
            reactWaiting = false;
            showLoseCoroutine = StartCoroutine(ShowReactionLose(true));
        }

        if (reactReady)
		{
            roundTime += Time.deltaTime;
            roundTimeText.text = (int)roundTime + "." + ((int)(roundTime * 1000) % 1000).ToString("000");
            if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
			{
                sumTime += roundTime;
                sumTimeText.text = (int)sumTime + "." + ((int)(sumTime * 1000) % 1000).ToString("000") + "/"
                    + (int)maxTime + "." + ((int)(maxTime * 1000) % 1000).ToString("000");
                reactIndicator.color = whiteColor;
                reactReady = false;
            }
        }
    }

    public void PauseGame(bool showMenu)
    {
        Time.timeScale = 0;
        player.audioSource.Pause();
        if(showMenu)
            pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        player.audioSource.UnPause();
        pauseMenu.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowCreaturesMenu()
	{
        PauseGame(false);
        pauseMenu.SetActive(false);
        creaturesMainList.SetActive(true);
	}

    public void CloseCreaturesMenu()
	{
        creaturesMainList.SetActive(false);
        ResumeGame();
	}

    public void CloseFullSizeView()
	{
        fullSizeView.SetActive(false);
        creaturesMainList.SetActive(true);
	}

    public void StartReactionMinigame(int difficulty)
	{
		switch (difficulty)
		{
            case 0:
                maxTime = 1.7f;
                break;
            case 1:
                maxTime = 1.45f;
                break;
            case 2:
                maxTime = 1.2f;
                break;
            default:
                Debug.Log("Unknown reaction minigame difficulty");
                break;
		}
        player.audioSource.Pause();
        musicSource.clip = gladiatorsMusicClip;
        musicSource.Play();
        curMinigame = StartCoroutine(ReactionMinigame());
    }

    public void StartMemoryMinigame(int difficulty)
	{
        grid4x4.SetActive(false);
        grid5x5.SetActive(false);
        grid6x6.SetActive(false);
        switch (difficulty)
		{
            case 0:
                totalButtons = 4 * 4;
                curGrid = grid4x4;
                break;
            case 1:
                totalButtons = 5 * 5;
                curGrid = grid5x5;
                break;
            case 2:
                totalButtons = 6 * 6;
                curGrid = grid6x6;
                break;
            default:
                Debug.Log("Unknown memory minigame difficulty");
                break;
        }
        curGrid.SetActive(true);
        player.audioSource.Pause();
        musicSource.clip = gladiatorsMusicClip;
        musicSource.Play();
        curMinigame = StartCoroutine(MemoryMinigame());
    }

    IEnumerator ReactionMinigame()
	{
        reactReady = false;
        reactWaiting = false;
        sumTime = 0;
        sumTimeText.color = whiteColor;
        reactIndicator.color = whiteColor;
        roundTimeText.text = "0.000";
        sumTimeText.text = "0.000/" + (int)maxTime + "." + ((int)(maxTime * 1000) % 1000).ToString("000");
        reactMessageText.text = "Welcome to Reaction!";
        reactRoundCounterText.text = "Round: 0/5";
        countdownText.gameObject.SetActive(false);
        reactionMinigame.SetActive(true);
        yield return new WaitForSeconds(3);
        reactMessageText.text = "Get ready!";

        for (int i = 0; i < 5; i++)
        {
            roundTime = 0;
            reactRoundCounterText.text = "Round: " + (i + 1) + "/5";
            countdownText.gameObject.SetActive(true);
            countdownText.text = "3";
            Animator countdownAnim = countdownText.gameObject.GetComponent<Animator>();
            countdownAnim.Play("Reaction countdown", -1, 0);
            effectsSource.PlayOneShot(countdownTickClip);
            yield return new WaitForSeconds(1);
            countdownText.text = "2";
            countdownAnim.Play("Reaction countdown", -1, 0);
            effectsSource.PlayOneShot(countdownTickClip);
            yield return new WaitForSeconds(1);
            countdownText.text = "1";
            countdownAnim.Play("Reaction countdown", -1, 0);
            effectsSource.PlayOneShot(countdownTickClip);
            yield return new WaitForSeconds(1);
            countdownText.gameObject.SetActive(false);
            reactWaiting = true;
            yield return new WaitForSeconds(Random.Range(minWait / 1000f, maxWait / 1000f));
            reactMessageText.text = "React!";
            reactIndicator.color = blueColor;
            reactWaiting = false;
            reactReady = true;
            yield return new WaitUntil(() => !reactReady);
            if (sumTime > maxTime)
            {
                showLoseCoroutine = StartCoroutine(ShowReactionLose(false));
                yield break;
            }
            if (i < 4)
            {
                reactMessageText.text = "Get ready!";
                yield return new WaitForSeconds(3);
            }
        }

        endingPhase = true;
        reactIndicator.color = greenColor;
        reactMessageText.text = "You won! :)";
        effectsSource.PlayOneShot(minigameVictoryClip, 0.55f);
        yield return new WaitForSeconds(3);
        reactionMinigame.SetActive(false);
        curMinigame = null;
        player.MinigameCompleted();
        endingPhase = false;
    }

    IEnumerator MemoryMinigame()
	{
        memoryMessageText.text = "Welcome to Memory!";
        memoryMessageText.color = whiteColor;
        memoryRoundCounterText.text = "Round: 0/5";
        memoryButtons.Clear();
        memoryMinigame.SetActive(true);
        for(int i = 1; i <= totalButtons; i++)
		{
            memoryButtons.Add(curGrid.transform.Find("Button " + i).gameObject);
            memoryButtons[i - 1].GetComponent<Image>().color = whiteColor;
            memoryButtons[i - 1].GetComponent<Button>().interactable = true;
            memoryButtons[i - 1].GetComponent<Animator>().enabled = false;
        }
        yield return new WaitForSeconds(3);
        for(int i = 0; i < 5; i++)
		{
            memoryRoundCounterText.text = "Round: " + (i + 1) + "/5";
            memoryMessageText.text = "Remember highlighted buttons";
            highlightedButtons = totalButtons / 4 + Mathf.RoundToInt(i / 2f);
            foreach (GameObject button in correctButtons)
            {
                button.GetComponent<Image>().color = whiteColor;
                button.GetComponent<Button>().interactable = true;
            }
            correctButtons.Clear();
            for(int j = 0; j < highlightedButtons; j++)
			{
                GameObject buttonToAdd;
                do
                {
                    buttonToAdd = memoryButtons[Random.Range(0, memoryButtons.Count)];
                } while (correctButtons.Contains(buttonToAdd));
                correctButtons.Add(buttonToAdd);
                buttonToAdd.GetComponent<Image>().color = blueColor;
			}
            yield return new WaitForSeconds(5);
            foreach(GameObject button in correctButtons)
			{
                Animator anim = button.GetComponent<Animator>();
                anim.enabled = true;
                anim.Play("Memory button tint", -1, 0);
			}
            effectsSource.PlayOneShot(memoryFadeClip);
            yield return new WaitForSeconds(3);
            foreach (GameObject button in correctButtons)
                button.GetComponent<Animator>().enabled = false;
            memoryMessageText.text = "Recreate pattern";
            guessedButtons = 0;
            memoryReady = true;
            yield return new WaitUntil(() => !memoryReady);
            if(guessedButtons == highlightedButtons)
			{
                foreach (GameObject button in correctButtons)
                    button.GetComponent<Image>().color = greenColor;
                memoryMessageText.text = "Round complete";
                yield return new WaitForSeconds(3);
            }
            else
			{
                memoryMessageText.text = "You lose... :(";
                memoryMessageText.color = redColor;
                foreach (GameObject button in correctButtons)
                    button.GetComponent<Image>().color = blueColor;
                effectsSource.PlayOneShot(loseClip);
                yield return new WaitForSeconds(3);
                memoryMinigame.SetActive(false);
                player.playingMinigame = false;
                player.audioSource.UnPause();
                musicSource.clip = flyMusicClip;
                musicSource.Play();
                player.interactionText.gameObject.SetActive(true);
                player.interactionReady = true;
                curMinigame = null;
                yield break;
            }
        }
        endingPhase = true;
        memoryMessageText.text = "You won! :)";
        memoryMessageText.color = greenColor;
        effectsSource.PlayOneShot(minigameVictoryClip, 0.55f);
        yield return new WaitForSeconds(3);
        memoryMinigame.SetActive(false);
        curMinigame = null;
        player.MinigameCompleted();
        endingPhase = false;
    }

    IEnumerator ShowReactionLose(bool prefire)
	{
        reactIndicator.color = redColor;
        if(!prefire)
            sumTimeText.color = redColor;
        reactMessageText.text = "You lose... :(";
        effectsSource.PlayOneShot(loseClip);
        yield return new WaitForSeconds(3);
        reactionMinigame.SetActive(false);
        player.playingMinigame = false;
        player.audioSource.UnPause();
        musicSource.clip = flyMusicClip;
        musicSource.Play();
        player.interactionText.gameObject.SetActive(true);
        player.interactionReady = true;
        curMinigame = null;
        showLoseCoroutine = null;
    }

    public void MemoryButtonClicked(int index)
	{
        if (!memoryReady)
            return;

        GameObject clickedButton = null;
        foreach(GameObject button in memoryButtons)
            if(button.name == "Button " + index)
			{
                clickedButton = button;
                break;
			}
        bool correct = correctButtons.Contains(clickedButton);
        if (correct)
        {
            clickedButton.GetComponent<Button>().interactable = false;
            clickedButton.GetComponent<Image>().color = blueColor;
            if (++guessedButtons == highlightedButtons)
                memoryReady = false;
        }
        else
        {
            clickedButton.GetComponent<Image>().color = redColor;
            memoryReady = false;
        }
	}
}
