using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Transform objectPoolParent;
    public Text ScoreText;
    private float score;

    public Text highscoreText;


    [Header("Start Panel")]
    public Transform StartUIPanel;
    public Text startText;
    public RectTransform[] bottomButtons;
    public CanvasGroup ChallengeParent;
    public CanvasGroup[] challengeTransform;

    [Header("End Panel")]
    public Transform endPanel;
    public Text endScoreText;
    public Image newHighscoreImage;
    public Text newHighscoreText;
    public Text gemText;
    public Text totalGemText;

    public Image fadePanel;

    public Text gemTrackerText;
    public Image gemTrackerImage;

    public Transform deathzone;

    private int screenshotCount;

    [Header("Store Panel")]
    public Transform storePanel;
    [Space(10)]
    public ColorProfile[] colorUnlocks;
    public GameObject colorGrid;
    public Transform colorPickParent;
    [Space(10)]
    public SkinProfile[] skinUnlocks;
    public GameObject skinGrid;
    public Transform skinPickParent;
    [Space(10)]

    public GameObject buyButtonPrefab;
    public Text store_GemText;
    public Button prevButton;



    [Header("Options Panel")]
    public Transform optionsMenu;

    public Image volumeImage;
    public Text volumeText;
    public Sprite musicOnImage;
    public Sprite musicOffImage;

    public Image controlImage;
    public Text controlText;
    public Sprite controlDragImage;
    public Sprite controlTiltImage;

    [Header("Tutorial Panel")]
    public RectTransform tutorialPanel;
    public Text tutorialText;

    public Transform[] startUpLayout;

    [Header("Challenge UI")]
    public Text gemChallengeText;
    public Text gemChallengeReward;
    public Transform gemChallengeComplete;
    [Space(10)]
    public Text distChallengeText;
    public Text distChallengeReward;
    public Transform distChallengeComplete;

    // Use this for initialization
    void Start()
    {

        highscoreText.text = GameManager.instance.saveData.highscore.ToString();
        if (GameManager.instance.saveData.volumeOn)
        {
            volumeImage.sprite = musicOnImage;
            volumeText.text = "Music on";
        }
        else
        {
            volumeImage.sprite = musicOffImage;
            volumeText.text = "Music off";
        }

        if (GameManager.instance.saveData.accelEnabled)
        {
            controlImage.sprite = controlTiltImage;
            controlText.text = "Control: Tilt";
        }
        else
        {
            controlImage.sprite = controlDragImage;
            controlText.text = "Control: Drag";
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            ScreenCapture.CaptureScreenshot(screenshotCount + ".png");
            screenshotCount += 1;
        }

        if (GameManager.instance.gameStarted)
        {
            score += Time.deltaTime;
            GameManager.instance.Score = (int)score;
            ScoreText.text = GameManager.instance.Score.ToString();

            if (score > GameManager.instance.saveData.highscore)
            {
                ScoreText.color = Color.yellow;
            }

        }
    }

    public void GameStart()
    {
        StartUIPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        StartCoroutine(UIAnimation.Fade(StartUIPanel.GetComponent<CanvasGroup>(), 1, 0, 1));
        StartCoroutine(DeathzoneStart());
        ScoreText.gameObject.SetActive(true);
        gemTrackerText.gameObject.SetActive(true);
        gemTrackerImage.gameObject.SetActive(true);
    }

    IEnumerator DeathzoneStart()
    {
        RectTransform dRect = deathzone.GetComponent<RectTransform>();
        yield return StartCoroutine(UIAnimation.ChangeRectSize(dRect, new Vector2(dRect.sizeDelta.x, 250), 10 * Time.deltaTime));
        //deathzone.GetComponent<Pulse>().enabled = true;
    }

    // Button methods

    public void StartButtonPressed()
    {
        GameManager.instance.GameStart();
    }

    public void QuitButtonPressed()
    {
        Application.Quit();
    }

    public void BackButtonPressed()
    {

    }

    public void RestartButtonPressed()
    {
        GameManager.instance.soundManager.PlayClip(GameManager.instance.soundManager.buttonPressSound, 0.5f);
        GameManager.instance.restarting = true;
        GameManager.instance.gemsFound = 0;
        fadePanel.gameObject.SetActive(true);
        StartCoroutine(RestartAnimation());
    }

    public IEnumerator RestartAnimation()
    {
        yield return StartCoroutine(UIAnimation.Fade(fadePanel.GetComponent<CanvasGroup>(), 0, 1, 2f));
        SceneManager.LoadScene(0);
        yield break;
    }

    public void GemBonusButtonPressed()
    {
        GameManager.instance.soundManager.PlayClip(GameManager.instance.soundManager.buttonPressSound, 0.5f);
        Debug.Log("RewardButtonPressed");
        GameManager.instance.adManager.DisplayRewardVideo();
    }

    public void RateButtonPressed()
    {
        GameManager.instance.soundManager.PlayClip(GameManager.instance.soundManager.buttonPressSound, 0.5f);
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=com.Ando.Loop");
#elif UNITY_IPHONE
         Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
#endif
    }

    // Displays end screen and checks highscore
    public void ShowEndScreen()
    {
        GameManager.instance.saveData.gems += GameManager.instance.gemsFound;

        endPanel.gameObject.SetActive(true);
        // Score
        endScoreText.text = GameManager.instance.Score.ToString();
        // Highscore Check
        if (score > GameManager.instance.saveData.highscore)
        {
            // Highscore is higher.
            newHighscoreImage.gameObject.SetActive(true);
            newHighscoreText.gameObject.SetActive(true);
            GameManager.instance.saveData.highscore = (int)score;
        }
        else
        {
            newHighscoreText.gameObject.SetActive(true);
            newHighscoreText.text = "Highscore: " + GameManager.instance.saveData.highscore.ToString();
        }
        // Gem
        gemText.text = GameManager.instance.gemsFound.ToString();
        totalGemText.text = "Total Gems: " + GameManager.instance.saveData.gems.ToString();
        GameManager.instance.CheckChallenges((int)score, GameManager.instance.gemsFound);
        SaveLoadManager.Save();
    }

    #region Store
    public void DisplayStore()
    {
        GameManager.instance.soundManager.PlayClip(GameManager.instance.soundManager.buttonPressSound, 0.5f);
        storePanel.gameObject.SetActive(true);
        store_GemText.text = GameManager.instance.saveData.gems.ToString();
        RefreshColorPicker();
    }

    public void ChangeStoreTab (Button button)
    {
        prevButton.GetComponent<Image>().color = Color.white;
        prevButton.GetComponentInChildren<Text>().color = Color.black;

        button.GetComponent<Image>().color = Color.black;
        var text = button.GetComponentInChildren<Text>();
        text.color = Color.white;
        switch (text.text)
        {
            case "Color":
                colorGrid.SetActive(true);
                skinGrid.SetActive(false);
                RefreshColorPicker();
                break;
            case "Skin":
                colorGrid.SetActive(false);
                skinGrid.SetActive(true);
                RefreshSkinPicker();
                break;
        }

        prevButton = button;
    }

    public int GetPrice(string type, int index)
    {
        switch(type)
        {
            case "Color":
                return colorUnlocks[index].cost;
            case "Skin":
                return skinUnlocks[index].cost;
        }
        return 0;
    }

    // Color Functionaility

    public void BuyColor (Button button)
    {
        int index = button.transform.parent.GetSiblingIndex();
        if(GameManager.instance.saveData.gems >= GetPrice("Color", index))
        {
            GameManager.instance.saveData.gems -= GetPrice("Color", index);
            GameManager.instance.saveData.colorsUnlocked.Add(index);
            SaveLoadManager.Save();
            RefreshColorPicker();
            store_GemText.text = GameManager.instance.saveData.gems.ToString();
        }
        else
        {
            Debug.Log("Not Enough Gems");
        }
    }
    public void ChangeColor(Button button)
    {
        if(button.GetComponentInChildren<Text>().text == "Rainbow")
        {
            GameManager.instance.playerControl.frontSRend.color = colorUnlocks[button.transform.parent.GetComponent<Transform>().GetSiblingIndex()].color;
            GameManager.instance.playerControl.backSRend.color = colorUnlocks[button.transform.parent.GetComponent<Transform>().GetSiblingIndex()].color;
            GameManager.instance.playerControl.frontSRend.material = GameManager.instance.playerControl.rainbowMat;
            GameManager.instance.playerControl.backSRend.material = GameManager.instance.playerControl.rainbowMat;
            GameManager.instance.playerControl.hueShifter.enabled = true;
            GameManager.instance.soundManager.PlayClip(GameManager.instance.soundManager.buttonPressSound, 0.5f);
            GameManager.instance.saveData.color = button.transform.parent.GetComponent<Transform>().GetSiblingIndex();
            RefreshColorPicker();
            SaveLoadManager.Save();
            return;
        }


        GameManager.instance.playerControl.hueShifter.enabled = false;

        GameManager.instance.soundManager.PlayClip(GameManager.instance.soundManager.buttonPressSound, 0.5f);

        GameManager.instance.playerControl.frontSRend.color = button.GetComponent<Image>().color;
        GameManager.instance.playerControl.backSRend.color = button.GetComponent<Image>().color;
        GameManager.instance.playerControl.frontSRend.material = GameManager.instance.playerControl.normalMat;
        GameManager.instance.playerControl.backSRend.material = GameManager.instance.playerControl.normalMat;

        GameManager.instance.saveData.color = button.transform.parent.GetComponent<Transform>().GetSiblingIndex();
        RefreshColorPicker();
        SaveLoadManager.Save();
    }
    public void RefreshColorPicker () {
        Button[] buttons = colorPickParent.GetComponentsInChildren<Button>();

        for (int i = 0; i < colorUnlocks.Length; i++)
        {
            if (GameManager.instance.saveData.colorsUnlocked.Contains(i))
            {

                buttons[i].GetComponentInChildren<Text>().text = colorUnlocks[i].colorName;
                if(colorUnlocks[i].colorName == "Rainbow")
                {
                    buttons[i].GetComponentsInParent<Image>()[0].color = colorUnlocks[i].color;
                    buttons[i].transform.parent.GetComponentsInChildren<Transform>()[5].GetComponent<CanvasGroup>().alpha = 0;
                    buttons[i].transform.parent.GetComponentsInChildren<Transform>()[5].GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
                else
                {
                    buttons[i].GetComponentsInParent<Image>()[0].color = colorUnlocks[i].color;
                    buttons[i].transform.parent.GetComponentsInChildren<Transform>()[4].GetComponent<CanvasGroup>().alpha = 0;
                    buttons[i].transform.parent.GetComponentsInChildren<Transform>()[4].GetComponent<CanvasGroup>().blocksRaycasts = false;
                }



                //GameObject newButton = Instantiate(colorButtonPrefab, colorPickParent);
                if (i == GameManager.instance.saveData.color)
                {
                    buttons[i].GetComponentsInParent<Image>()[1].color = Color.grey;
                }
                else
                {
                    buttons[i].GetComponentsInParent<Image>()[1].color = Color.black;

                }
            }
            else
            {
                buttons[i].transform.parent.GetComponentsInChildren<Text>()[1].text = colorUnlocks[i].cost.ToString();

            }
        }
    }

    // Skin functionaility.

    public void BuySkin(Button button)
    {
        int index = button.transform.parent.GetSiblingIndex();

        if (GameManager.instance.saveData.gems >= GetPrice("Skin", index))
        {
            GameManager.instance.saveData.gems -= GetPrice("Skin", index);
            GameManager.instance.saveData.skinsUnlocked.Add(index);
            SaveLoadManager.Save();
            RefreshSkinPicker();
            store_GemText.text = GameManager.instance.saveData.gems.ToString();
        }
        else
        {
            Debug.Log("Not Enough Gems");
        }
    }
    public void ChangeSkin(Button button)
    {
        GameManager.instance.soundManager.PlayClip(GameManager.instance.soundManager.buttonPressSound, 0.5f);

        GameManager.instance.playerControl.frontSRend.sprite = skinUnlocks[button.transform.parent.GetComponent<Transform>().GetSiblingIndex()].frontSkin;
        GameManager.instance.playerControl.backSRend.sprite = skinUnlocks[button.transform.parent.GetComponent<Transform>().GetSiblingIndex()].backSkin;

        GameManager.instance.saveData.skin = button.transform.parent.GetComponent<Transform>().GetSiblingIndex();
        RefreshSkinPicker();
        SaveLoadManager.Save();
    }
    public void RefreshSkinPicker()
    {
        Button[] buttons = skinPickParent.GetComponentsInChildren<Button>();

        for (int i = 0; i < skinUnlocks.Length; i++)
        {
            Debug.Log(skinUnlocks.Length);

            if (GameManager.instance.saveData.skinsUnlocked.Contains(i))
            {
                
                buttons[i].GetComponentsInChildren<Image>()[1].sprite = skinUnlocks[i].icon;
                buttons[i].GetComponentInChildren<Text>().text = skinUnlocks[i].skinName;

                buttons[i].transform.parent.GetComponentsInChildren<Transform>()[5].GetComponent<CanvasGroup>().alpha = 0;
                buttons[i].transform.parent.GetComponentsInChildren<Transform>()[5].GetComponent<CanvasGroup>().blocksRaycasts = false;

                if (i == GameManager.instance.saveData.skin)
                {
                    buttons[i].GetComponentsInParent<Image>()[1].color = Color.grey;
                }
                else
                {
                    buttons[i].GetComponentsInParent<Image>()[1].color = Color.black;
                }
            }
            else
            {
                Debug.Log(buttons[i].transform.parent.GetComponentsInChildren<Transform>()[0].name);
                buttons[i].transform.parent.GetComponentsInChildren<Text>()[1].text = skinUnlocks[i].cost.ToString();

            }
        }
    }

    public void SetPlayerLook ()
    {
        //GameManager.instance.playerControl.frontSRend.sprite = skinUnlocks[GameManager.instance.saveData.skin].frontSkin;
        //GameManager.instance.playerControl.backSRend.sprite = skinUnlocks[GameManager.instance.saveData.skin].backSkin;
        //GameManager.instance.playerControl.frontSRend.color = colorUnlocks[GameManager.instance.saveData.color].color;
        //GameManager.instance.playerControl.backSRend.color = colorUnlocks[GameManager.instance.saveData.color].color;
    }

    #endregion

    #region Options

    public void ChangeVolumeButton()
    {
        GameManager.instance.saveData.volumeOn = !GameManager.instance.saveData.volumeOn; 
        if(GameManager.instance.saveData.volumeOn)
        {
            volumeImage.sprite = musicOnImage;
            volumeText.text = "Music on";
        }
        else
        {
            volumeImage.sprite = musicOffImage;
            volumeText.text = "Music off";
        }
        SaveLoadManager.Save();
    }

    public void ControlChanged () {
        GameManager.instance.saveData.accelEnabled = !GameManager.instance.saveData.accelEnabled; 
        if (GameManager.instance.saveData.accelEnabled)
        {
            controlImage.sprite = controlTiltImage;
            controlText.text = "Control: Tilt";
        }
        else
        {
            controlImage.sprite = controlDragImage;
            controlText.text = "Control: Drag";
        }
        SaveLoadManager.Save();
    }

    public void DisplayOptionsMenu () {
        optionsMenu.gameObject.SetActive(!optionsMenu.gameObject.activeInHierarchy);
    }

    #endregion

    public void Mute(UnityEngine.Events.UnityEventBase ev)
    {
        int count = ev.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            ev.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
        }
    }

    public void Unmute(UnityEngine.Events.UnityEventBase ev)
    {
        int count = ev.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            ev.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        }
    }

    public void ButtonPressed () {
        GameManager.instance.soundManager.PlayClip(GameManager.instance.soundManager.buttonPressSound, 0.5f);
    }
}

[System.Serializable]
public struct ColorProfile {
    public string colorName;
    public Color color;
    public int cost;
}

[System.Serializable]
public struct SkinProfile {
    public string skinName;
    public Sprite icon;
    public Sprite frontSkin;
    public Sprite backSkin;
    public int cost;
}
