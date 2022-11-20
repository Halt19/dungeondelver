using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class GameMasterScript : MonoBehaviour
{
    [SerializeField]
    private LevelBuilder builder;
    [SerializeField]
    private Slider loadSlider;

    [SerializeField]
    private FirstPersonController playerController;

    [SerializeField]
    private Canvas optionsMenuCanvas;
    [SerializeField]
    private Canvas loadScreenCanvas;
    [SerializeField]
    private Canvas gameHUDCanvas;
    [SerializeField]
    private Canvas scoreScreenCanvas;

    [SerializeField]
    private InputField genSizeText;
    [SerializeField]
    private InputField numGenText;
    [SerializeField]
    private Slider enemyFrequencySlider;
    [SerializeField]
    private Slider enemyPowerSlider;
    [SerializeField]
    private Slider enemyHealthSlider;
    [SerializeField]
    private Slider pickupHealthSlider;
    [SerializeField]
    private Slider pickupPointsSlider;
    [SerializeField]
    private Slider pickupFrequencySlider;
    [SerializeField]
    private Slider mutRateSlider;
    [SerializeField]
    private InputField numEliteText;
    [SerializeField]
    private Slider lengthSlider;
    [SerializeField]
    private Slider gunRateSlider;

    private int mode = 0;

    private void Start()
    {
        optionsMenuCanvas.gameObject.SetActive(true);
        loadScreenCanvas.gameObject.SetActive(false);
        gameHUDCanvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (playerController == null)
        {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
        }
       playerController.SetPauseStatus(true);
    }

    /*
    I also hated making this one. Sets the values in the LevelBuilder to the values from the settings screen.
    */
    public void ApplySettings()
    {
        int minEHealth = Mathf.Max(0, (int)enemyHealthSlider.value - 40);
        int minEDamage = Mathf.Max(0, (int)enemyPowerSlider.value - 10);
        int minPHealth = Mathf.Max(0, (int)pickupHealthSlider.value - 20);
        int minPPoints = Mathf.Max(0, (int)pickupPointsSlider.value - 75);
        int genSize = int.Parse(genSizeText.text);
        int numGens = int.Parse(numGenText.text);
        int numElitesInc = int.Parse(numEliteText.text);
        builder.SetGenerationInfo(100, 10, 15, minEHealth, (int)enemyHealthSlider.value+40, minEDamage, (int)enemyPowerSlider.value+10, minPHealth, 
        (int)pickupHealthSlider.value+20, minPPoints, (int)pickupPointsSlider.value+75, (int)enemyFrequencySlider.value, 
        (int)enemyHealthSlider.value*(int)enemyFrequencySlider.value, (int)enemyPowerSlider.value*(int)enemyFrequencySlider.value, (int)pickupFrequencySlider.value,
        (int)pickupHealthSlider.value*(int)pickupFrequencySlider.value, (int)pickupPointsSlider.value*(int)pickupFrequencySlider.value, mutRateSlider.value, 0.75f,
        0.5f, genSize, numElitesInc, numGens, (int)lengthSlider.value, gunRateSlider.value);
    } // ApplySettings

    /*
    Activates the LevelBuilder to generate the level, and brings up/activates the load screen.
    */
    public void GenerateLevel()
    {
        builder.Generate();
        loadSlider.maxValue = builder.NumGenerations;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        optionsMenuCanvas.gameObject.SetActive(false);
        loadScreenCanvas.gameObject.SetActive(true);
        gameHUDCanvas.gameObject.SetActive(false);        
        StartCoroutine(LoadSliderUpdate());
    } // GenerateLevel

    /*
    Ends the game, bringing the player to the score screen, and locking their controls. The score screen script then brings the player back to the settings screen.
    */
    public void EndGame()
    {
        builder.ClearRooms();
        optionsMenuCanvas.gameObject.SetActive(false);
        loadScreenCanvas.gameObject.SetActive(false);
        gameHUDCanvas.gameObject.SetActive(false);
        scoreScreenCanvas.gameObject.SetActive(true);
        playerController.SetPauseStatus(true);
    } // EndGame

    /*
    Brings up the settings screen.
    */
    public void ToSettingsScreen()
    {
        optionsMenuCanvas.gameObject.SetActive(true);
        loadScreenCanvas.gameObject.SetActive(false);
        gameHUDCanvas.gameObject.SetActive(false);
        scoreScreenCanvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    } // ToSettingsScreen

    /*
    Updates the value of the loading progress bar based on the number of generations created by the LevelBuilder. At the end, sets the active canvas to the
    gameplay one and activates player controls.
    */
    private IEnumerator LoadSliderUpdate()
    {
        while (builder.loadProgress < builder.NumGenerations)
        {
            loadSlider.value = builder.loadProgress;
            yield return new WaitForEndOfFrame();
        }
        optionsMenuCanvas.gameObject.SetActive(false);
        loadScreenCanvas.gameObject.SetActive(false);
        gameHUDCanvas.gameObject.SetActive(true);
        playerController.SetPauseStatus(false);
    } // LoadSliderUpdate

    /*
    Quits the game after 2 seconds. Unused.
    */
    private IEnumerator QuitGame()
    {
        yield return new WaitForSeconds(2);
        Application.Quit();
    } // QuitGame
}
