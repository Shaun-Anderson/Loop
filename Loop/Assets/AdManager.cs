using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Advertisements;


public class AdManager : MonoBehaviour {

	public string type;

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                //GameMaster.instance.UIManager.DisplayGameOver();
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                //GameMaster.instance.UIManager.DisplayGameOver();

                break;
            case ShowResult.Failed:
                Debug.Log("The ad failed to be shown.");
                break;
        }
    }

    private void HandleRewardResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                //GameMaster.instance.UIManager.DisplayGameOver();
                //
                // YOUR CODE TO REWARD THE GAMER
                GameManager.instance.soundManager.PlayClip(GameManager.instance.soundManager.rewardSound, 0.5f);
                GameManager.instance.gemsFound = GameManager.instance.gemsFound * 2;
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                //GameMaster.instance.UIManager.DisplayGameOver();

                break;
            case ShowResult.Failed:
                Debug.Log("The ad failed to be shown.");
                break;
        }
    }

	public void DisplayInterstitial () {
        if (Advertisement.IsReady("video"))
        {
            GameManager.instance.adCounter = 3;
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("video", options);
        }
	}

    public void DisplayRewardVideo () {
        if(Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleRewardResult };
            Advertisement.Show("rewardVideo", options);
        }
    }
}
