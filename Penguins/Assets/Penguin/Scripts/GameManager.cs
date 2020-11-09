using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum mode {simulation, competitive}
public enum difficulty { hard, medium, easy }

public class GameManager : MonoBehaviour
{
    public static mode gameMode = mode.simulation;
    public static difficulty difficulty = difficulty.easy;

    private void Awake() {

        // Avoid having a lot of Game Managers in the scenes
        foreach (GameManager GM in GameObject.FindObjectsOfType<GameManager>()) {
            if (GM.gameObject != this.gameObject) {
                Destroy(GM.gameObject);
                break;
            }
        }

        DontDestroyOnLoad(this.gameObject);

    }

    // Activate the simulation game mode
    public void Simulation() {

        gameMode = mode.simulation;

        SceneManager.LoadScene("GameScene");

    }

    // Activate the competitive game mode
    public void Competitive() {

        gameMode = mode.competitive;

        SceneManager.LoadScene("GameScene");

    }

    // Go to the menu
    public void Menu() {
        SceneManager.LoadScene("Menu");
    }
}
