using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class PointsScreenScript : MonoBehaviour
{
    [SerializeField]
    private FirstPersonController fPC;
    [SerializeField]
    private Text pointsText;
    [SerializeField]
    private GameMasterScript gMS;

    private void OnEnable()
    { // On enable, show the score from the player for a little while, then go to the settings screen
        pointsText.text = "" + fPC.GetScore();
        fPC.ResetPoints();
        StartCoroutine(ContinueToSettings());
    }

    private IEnumerator ContinueToSettings()
    {
        yield return new WaitForSeconds(5f);
        gMS.ToSettingsScreen();
    }
}
