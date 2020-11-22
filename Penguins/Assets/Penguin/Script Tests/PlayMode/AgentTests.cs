using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class AgentTests
    {

        // Check if after getting into a new Episode in Competitive mode:
        // - The agent and the player have a empty stomach
        // - The acummulative reward is 0
        // - The fish counter for the player and the penguin are 0
        // - The timer is 0
        [UnityTest]
        public IEnumerator CompetitiveModeReset()
        {
            GameManager.gameMode = mode.competitive;
            var async = SceneManager.LoadSceneAsync("GameScene");

            yield return new WaitUntil(() => async.isDone);

            var player = GameObject.Find("Penguin Player");

            var agent = GameObject.Find("Penguin Agent");
            var AI = agent.GetComponent<PenguinAgent>();

            var hud = GameObject.Find("HUD").GetComponent<HUDController>();

            hud.ResetGameStats();

            // Check if stomach is full
            Assert.AreEqual(false, agent.GetComponent<PenguinConfig>().isPenguinFull);
            Assert.AreEqual(false, player.GetComponent<PenguinConfig>().isPenguinFull);

            // Check if cumulative reward is 0
            Assert.AreEqual(0, Mathf.Round(agent.GetComponent<PenguinAgent>().GetCumulativeReward()));

            // Check if fish counter is 0
            Assert.AreEqual("0", hud.AgentNumFishRegurgitated.text);
            Assert.AreEqual("0", hud.PlayerNumFishRegurgitated.text);

            // Check if timer is 0
            Assert.AreEqual("Time 0s", hud.TimeTxt.text);

        }

        // Check if after getting into a new Episode in Simulation mode :
        // - The agent have a empty stomach
        // - The acummulative reward is 0
        // - The fish counter for the penguin are 0
        // - The timer is 0
        [UnityTest]
        public IEnumerator SimulationModeReset() {

            GameManager.Simulation();

            var agent = GameObject.Find("Penguin Agent");
            var AI = agent.GetComponent<PenguinAgent>();

            var hud = GameObject.Find("HUD");

            AI.OnEpisodeBegin();

            // Check if stomach is full
            Assert.AreEqual(false, agent.GetComponent<PenguinConfig>().isPenguinFull);

            // Check if cumulative reward is 0
            Assert.AreEqual(0, Mathf.Round(agent.GetComponent<PenguinAgent>().GetCumulativeReward()));

            // Check if fish counter is 0
            Assert.AreEqual("0", hud.GetComponent<HUDController>().AgentNumFishRegurgitated.text);

            // Check if timer is 0
            Assert.AreEqual("Time 0s", hud.GetComponent<HUDController>().TimeTxt.text);

            yield return null;
        }

        // Check if after getting into a new Episode in Training mode :
        // - The agent have a empty stomach
        // - The cummulative reward is 0
        [UnityTest]
        public IEnumerator TrainingModeReset() {

            var async = SceneManager.LoadSceneAsync("TrainingScene");

            yield return new WaitUntil(() => async.isDone);

            var agents = new List<GameObject>();

            foreach (GameObject agent in GameObject.FindGameObjectsWithTag("Agent")) {
                agents.Add(agent);
            }

            agents[0].GetComponent<PenguinAgent>().OnEpisodeBegin();

            foreach (GameObject penguin in agents) {

                // Check if stomach is full
                Assert.AreEqual(false, penguin.GetComponent<PenguinConfig>().isPenguinFull);

                // Check if cumulative reward is 0
                Assert.AreEqual(0, Mathf.Round(penguin.GetComponent<PenguinAgent>().GetCumulativeReward()));
            }
            
            yield return null;
        }


    }
}
