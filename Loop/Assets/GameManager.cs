using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    
    [Header("Manager References")]
    public static GameManager instance;
    public SoundManager soundManager;
    public UIManager UIManager;
    public AdManager adManager;
    public RectTransform canvas;

    [Header("Game References")]
    public Generator generator;
    public Player playerControl;

    [Header("Data")]
    public SaveData saveData;
    private bool colorCheck;
    private int score;
    public int Score
    {
        get {
            return score;
        }
        set {
            score = value;
            if (instance.IsDivisble(score, 10))
            {
                instance.generator.Speed += 0.02f;

                if(changeColor)
                {
                    instance.ChangeColors();
                    colorCheck = true; 
                }
            }
        }
    }
    public int gemsFound;
    public int adCounter;

    public bool gameStarted;
    public bool restarting;

    public bool changeColor;
    public Palette[] palettes;
    public int curPalette;
    public Color lerpedColor;

    [Header("Challenges")]
    public Challenge[] gemChallenges;
    public Challenge[] distanceChallenges;
    public bool gemChallengeComplete, distanceChallengeComplete;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            instance.adCounter = 4;
            instance.soundManager = instance.GetComponent<SoundManager>();
            instance.adManager = instance.GetComponent<AdManager>();
            SceneManager.activeSceneChanged += SceneChanged;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this);
        }
    }

    #region Challenges
    public void CheckChallenges (int distance, int gemsFound)
    {
        // Check distance
        if(distance > distanceChallenges[instance.saveData.curDistanceChallenge].distanceNeeded)
        {
            distanceChallengeComplete = true;
        }

        // Check gem
        if(gemsFound >= gemChallenges[instance.saveData.curGemChallenge].gemsNeeded)
        {
            gemChallengeComplete = true;
        }
    }

    public void UpdateChallenges () {

        // TODO add check for all challnges completed.

        instance.UIManager.gemChallengeText.text = instance.gemChallenges[instance.saveData.curGemChallenge].challengeTitle;
        instance.UIManager.gemChallengeReward.text = instance.gemChallenges[instance.saveData.curGemChallenge].rewardGems.ToString();

        instance.UIManager.distChallengeText.text = instance.distanceChallenges[instance.saveData.curDistanceChallenge].challengeTitle;
        instance.UIManager.distChallengeReward.text = instance.distanceChallenges[instance.saveData.curDistanceChallenge].rewardGems.ToString();
    }

    public IEnumerator DistanceChallengeComplete ()
    {
        yield return new WaitForSeconds(1);
        instance.soundManager.PlayClip(instance.soundManager.rewardSound, 0.5f);
        instance.UIManager.distChallengeComplete.gameObject.SetActive(true);
        instance.saveData.gems += instance.distanceChallenges[instance.saveData.curDistanceChallenge].rewardGems;
        instance.saveData.curDistanceChallenge += 1;
        SaveLoadManager.Save();
        UpdateChallenges();
        yield return new WaitForSeconds(1);
        instance.distanceChallengeComplete = false;
        instance.UIManager.distChallengeComplete.gameObject.SetActive(false);
        yield break;
    }

    public IEnumerator GemChallengeComplete()
    {
        yield return new WaitForSeconds(1);
        instance.soundManager.PlayClip(instance.soundManager.rewardSound, 0.5f);
        instance.UIManager.gemChallengeComplete.gameObject.SetActive(true);
        instance.saveData.gems += instance.gemChallenges[instance.saveData.curGemChallenge].rewardGems;
        instance.saveData.curGemChallenge += 1;
        SaveLoadManager.Save();
        UpdateChallenges();
        yield return new WaitForSeconds(1);
        instance.gemChallengeComplete = false;
        instance.UIManager.gemChallengeComplete.gameObject.SetActive(false);
        yield break;
    }



#endregion

    void SceneChanged(Scene fromScene, Scene toScene)
    {
        switch (toScene.name)
        {
            case "Game":
                // Set Variables
                //instance.SetInitalValues();
                instance.UIManager = GameObject.Find("LevelManager").GetComponent<UIManager>();
                instance.generator = GameObject.Find("Generator").GetComponent<Generator>();
                instance.playerControl = GameObject.Find("Player").GetComponent<Player>();
                instance.canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();

                SaveLoadManager.Load();
                UIManager.SetPlayerLook();
                UpdateChallenges();

                if(restarting)
                {
                    UIManager.fadePanel.gameObject.SetActive(true);
                    StartCoroutine(RestartAnimation());
                    restarting = false;
                }

                StartCoroutine(StartAnimation());
                break;
        }
    }

    public IEnumerator StartAnimation ()
    {
        foreach(RectTransform rect in UIManager.bottomButtons)
        {
            StartCoroutine(UIAnimation.MoveRect(rect, new Vector2(rect.anchoredPosition.x, 0), 1000));
            yield return new WaitForSeconds(0.1f);
        }

        yield return StartCoroutine(UIAnimation.Fade(UIManager.ChallengeParent, 0, 1, 2));

        foreach (CanvasGroup g in UIManager.challengeTransform)
        {
            switch (g.name)
            {
                case "GemChallenges":
                    if(saveData.curGemChallenge < instance.gemChallenges.Length)
                    {
                        StartCoroutine(UIAnimation.Fade(g, 0, 1, 2));
                    }
                    break;
                case "DistanceChallenges":
                    if (saveData.curDistanceChallenge < instance.distanceChallenges.Length)
                    {
                        StartCoroutine(UIAnimation.Fade(g, 0, 1, 2));
                    }
                    break;
            }
            yield return new WaitForSeconds(0.5f);
        }

        if (gemChallengeComplete)
        {
            StartCoroutine(GemChallengeComplete());
        }

        if (distanceChallengeComplete)
        {
            StartCoroutine(DistanceChallengeComplete());
        }
        yield break;
    }

    public void ChangeColors () {
        StopCoroutine(_ChangeColors());
        curPalette = Random.Range(0, palettes.Length);
        StartCoroutine(_ChangeColors());
    }

    IEnumerator _ChangeColors () {
        //Color camColor = Camera.main.backgroundColor;
        Color curColor = Camera.main.backgroundColor;
        Color curLineColor = generator.lineRenderer.startColor;
        while (true)
        {
            if(Camera.main.backgroundColor != instance.palettes[curPalette].backgroundColor)
            {
                curColor = Color.Lerp(curColor, instance.palettes[curPalette].backgroundColor, 2 * Time.deltaTime);
                curLineColor = Color.Lerp(curLineColor, instance.palettes[curPalette].lineColor, 2 * Time.deltaTime);


                //Debug.Log("CHANGE COLOR from: " + camColor + " To: " + instance.palettes[curPalette].backgroundColor + " || " +  lerpedColor);
                Camera.main.backgroundColor = curColor;
                instance.generator.lineRenderer.startColor = curLineColor;
                instance.generator.lineRenderer.endColor = curLineColor;
                yield return true;
            }
            else
            {
                Debug.Log("COLOR Done");

                yield break;
            }
        }
    }

    IEnumerator DisplayTutorial () {
        Debug.Log(UIManager.tutorialPanel.localPosition + "  ||  " + UIManager.tutorialPanel.anchoredPosition);
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(UIAnimation.MoveRect(UIManager.tutorialPanel, new Vector3(UIManager.tutorialPanel.transform.localPosition.x, 0), 20000 * Time.deltaTime));
        yield return new WaitForSeconds(3);
        yield return StartCoroutine(UIAnimation.MoveRect(UIManager.tutorialPanel, new Vector3(UIManager.tutorialPanel.transform.localPosition.x, 300), 20000 * Time.deltaTime));
        yield break;
    }

    IEnumerator RestartAnimation ()
    {
        yield return StartCoroutine(UIAnimation.Fade(UIManager.fadePanel.GetComponent<CanvasGroup>(), 1, 0, 2));
        UIManager.fadePanel.gameObject.SetActive(false);
        yield break;
    }

    public bool IsDivisble(int x, int n)
    {
        if(x == 0)
        {
            return false;
        }
        return (x % n) == 0;
    }

    public void GameStart() {
        
        gameStarted = true;

        generator.Speed = 9;
        generator.enabled = true;

        playerControl.calibrateAccelerometer();
        playerControl.started = true;
        playerControl.topTrail.active = true;
        playerControl.bottomTrail.active = true;

        if(!instance.saveData.tutorialCompleted) {
            instance.saveData.tutorialCompleted = true;
            StartCoroutine(DisplayTutorial());
        }

        UIManager.GameStart();

    }

    public void SetInitalValues()
    {
        if (instance.saveData.color == 8)
        {
            GameManager.instance.playerControl.frontSRend.material = GameManager.instance.playerControl.rainbowMat;
            GameManager.instance.playerControl.backSRend.material = GameManager.instance.playerControl.rainbowMat;
            instance.playerControl.hueShifter.enabled = true;
            return;
        }

        instance.playerControl.frontSRend.color = GameManager.instance.UIManager.colorUnlocks[GameManager.instance.saveData.color].color;
        instance.playerControl.backSRend.color = GameManager.instance.UIManager.colorUnlocks[GameManager.instance.saveData.color].color;

        GameManager.instance.playerControl.frontSRend.sprite = instance.UIManager.skinUnlocks[instance.saveData.skin].frontSkin;
        GameManager.instance.playerControl.backSRend.sprite = instance.UIManager.skinUnlocks[instance.saveData.skin].backSkin;
    }
    
    public void PlayerLoss () {
        generator.enabled = false;
        playerControl.enabled = false;
        gameStarted = false;
        instance.adCounter -= 1;

        instance.StartCoroutine(instance._PlayerLoss());

    }

    public IEnumerator _PlayerLoss () {
        yield return new WaitForSeconds(1);

        if (instance.adCounter <= 0)
        {
            adManager.DisplayInterstitial();
            instance.adCounter = 3;
        }

        UIManager.ShowEndScreen();

        yield break;
    }
}
