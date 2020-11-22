using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{
    public PenguinConfig agent;
    public PenguinConfig player;
    public PenguinArea area;

    // Text related to the agent
    public Text AgentAccumulativeReward;
    public Text AgentNumFishRegurgitated;

    // Text related to the player
    public Image PlayerPanel;
    public Text PlayerText;
    public Text PlayerNumFishRegurgitated;

    // Text related to the time past in the round
    public Text TimeTxt;

    // Win and Lose Panels
    public GameObject WinPanel;
    public GameObject LosePanel;

    public Button SimulationModeButton;
    public Button CompetitiveModeButton;

    public Button HardButton;
    public Button MediumButton;
    public Button EasyButton;

    public Text descriptionText;

    [HideInInspector] public float currentTime;

    private int seconds;

    private Vector3 playerPos;
    private Quaternion playerRot;

    private void Awake() {

        playerPos = player.gameObject.transform.position;
        playerRot = player.gameObject.transform.rotation;

        if (GameManager.gameMode == mode.simulation) {
            PlayerPanel.gameObject.SetActive(false);
            PlayerText.gameObject.SetActive(false);
            PlayerNumFishRegurgitated.gameObject.SetActive(false);
            CompetitiveModeButton.gameObject.SetActive(true);
            SimulationModeButton.gameObject.SetActive(false);
            ChangeDifficulty(0);
        } else {
            PlayerPanel.gameObject.SetActive(true);
            PlayerText.gameObject.SetActive(true);
            PlayerNumFishRegurgitated.gameObject.SetActive(true);
            CompetitiveModeButton.gameObject.SetActive(false);
            SimulationModeButton.gameObject.SetActive(true);
            ChangeDifficulty(0);
        }

    }


    // Update is called once per frame
    private void Update()
    {
        // Update the number os fish feed to the baby penguin
        AgentNumFishRegurgitated.text = agent.numFishFeed.ToString();

        if (GameManager.gameMode == mode.competitive) {
            PlayerNumFishRegurgitated.text = player.numFishFeed.ToString();
        }

        // Update the accumulative reward the agent is receiving during the round
        AgentAccumulativeReward.text = "Accumulative reward: " + agent.gameObject.GetComponent<PenguinAgent>().GetCumulativeReward().ToString("0.00");

        // Update the time past in the round
        currentTime += Time.deltaTime;
        seconds = (int) (currentTime % 60);
        TimeTxt.text = "Time " + seconds.ToString() + "s";

        if (GameManager.difficulty == difficulty.easy) {
            MediumButton.interactable = true;
            HardButton.interactable = true;
            EasyButton.interactable = false;

        } else if (GameManager.difficulty == difficulty.medium) {
            MediumButton.interactable = false;
            HardButton.interactable = true;
            EasyButton.interactable = true;

        } else if (GameManager.difficulty == difficulty.hard) {
            MediumButton.interactable = true;
            HardButton.interactable = false;
            EasyButton.interactable = true;
        }

    }

    // Show Lose or Win Panel - TODO (precisa saber o número de peixes presentes)
    public void NextRoundPanel() {

        Time.timeScale = 0.0f;

        // If the player have more fish feed than the agent, show win panel
        if (player.numFishFeed > agent.numFishFeed) {
            WinPanel.SetActive(true);

        // Else, show lose panel
        } else {
            LosePanel.SetActive(true);
        }
    }

    // Reset game stats every round
    public void ResetGameStats() {

        // Unpause
        Time.timeScale = 1.0f;

        // Restart player position
        player.gameObject.transform.position = playerPos;
        player.gameObject.transform.rotation = playerRot;

        WinPanel.SetActive(false);
        LosePanel.SetActive(false);

        // Update the number os fish feed to the baby penguin
        AgentNumFishRegurgitated.text = "0";

        if (GameManager.gameMode == mode.competitive)
            PlayerNumFishRegurgitated.text = "0";

        // Update the accumulative reward the agent is receiving during the round
        AgentAccumulativeReward.text = "Accumulative reward: " + "0.00";

        //  Update the time past in the round
        TimeTxt.text = "Time " + "0" + "s";
        currentTime = 0;

        area.ResetArea();
    }

    public void ChangeDifficulty (int difficulty) {


        // Easy
        if (difficulty == 0) {
            GameManager.difficulty = global::difficulty.easy;
            descriptionText.text = "PPO + Curriculum + Memory";

        // Medium
        } else if (difficulty == 1) {
            GameManager.difficulty = global::difficulty.medium;
            descriptionText.text = "PPO + Curriculum + Memory + Curiosity";

        // Hard
        } else {
            GameManager.difficulty = global::difficulty.hard;
            descriptionText.text = "PPO + Curriculum";
        }

        agent.gameObject.GetComponent<PenguinAgent>().ChangeModel(GameManager.difficulty);

    }

}
