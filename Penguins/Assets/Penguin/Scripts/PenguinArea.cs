using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using UnityEngine.SceneManagement;

public class PenguinArea: MonoBehaviour
{
    private List<GameObject> fishList = new List<GameObject>();
    private float feedRadius = 0f;
    private Transform initialPlayerPos; 

    [Tooltip("The agent inside the area")]
    public GameObject penguinAgent;

    [Tooltip("The player")]
    public GameObject penguinPlayer;

    [Tooltip("Prefab of a live fish")]
    public Fish fishPrefab; 

    /// <summary>
    /// The number of fish remaining
    /// </summary>
    public int FishRemaining {
        get { return fishList.Count; }
    }

    /// <summary>
    /// The feed radius defined by the curriculum learning
    /// </summary>
    public float FeedRadius {
        get { return feedRadius; }
    }


    /// <summary>
    /// Called when the game starts
    /// </summary>
    private void Start() {

        if (SceneManager.GetActiveScene().name == "TrainingScene") {
            GameManager.gameMode = mode.training;
        }

        if (GameManager.gameMode == mode.competitive)
            initialPlayerPos = penguinPlayer.transform;
        ResetArea();
    }


    private void FixedUpdate() {

        // Check if still have fishes in the pond and they are all feed to the baby
        if (FishRemaining <= 0 && !penguinAgent.GetComponent<PenguinConfig>().isPenguinFull) {

            if (GameManager.gameMode == mode.competitive) {

                if (!penguinPlayer.GetComponent<PenguinConfig>().isPenguinFull) {
                    penguinAgent.GetComponent<PenguinAgent>().EndEpisode();
                }
            } else {
                penguinAgent.GetComponent<PenguinAgent>().EndEpisode();
            }

        }
    }

    /// <summary>
    /// Reset the area, including fish and penguin placement
    /// </summary>
    public void ResetArea()
    {
        RemoveAllFish();
        PlacePenguin(penguinAgent);
        if (GameManager.gameMode == mode.competitive)
            PlacePenguin(penguinPlayer);
        PlaceBaby(penguinAgent.GetComponent<PenguinConfig>().Baby);

        // The player could compete with the agent to feed the same baby or their own
        if (GameManager.gameMode == mode.competitive && penguinAgent.GetComponent<PenguinConfig>().Baby != penguinPlayer.GetComponent<PenguinConfig>().Baby) {
            PlaceBaby(penguinPlayer.GetComponent<PenguinConfig>().Baby);
        }

        // Change the fish speed with curriculum learning
        // by default the velocity with curriculum learning active is 0 increasing everytime the agent gets better
        // without curriculum learning the fish speed is 0.5
        SpawnFish(5, Academy.Instance.EnvironmentParameters.GetWithDefault("fish_speed", 0.5f));

        // Check if the default radius changed, the default value is 0 (needs to touch the baby penguin to feed him)
        feedRadius = Academy.Instance.EnvironmentParameters.GetWithDefault("feed_radius", 0f);

        penguinAgent.GetComponent<PenguinConfig>().numFishFeed = 0;
        if (GameManager.gameMode == mode.competitive)
            penguinPlayer.GetComponent<PenguinConfig>().numFishFeed = 0;

    }


    /// <summary>
    /// Remove a specific fish from the area when it is eaten
    /// </summary>
    /// <param name="fishObject">The fish to remove</param>
    public void RemoveSpecificFish(GameObject fishObject)
    {
        fishList.Remove(fishObject);
        Destroy(fishObject);
    }

    /// <summary>
    /// Choose a random position on the X-Z plane within a partial donut shape
    /// </summary>
    /// <param name="center">The center of the donut</param>
    /// <param name="minAngle">Minimum angle of the wedge</param>
    /// <param name="maxAngle">Maximum angle of the wedge</param>
    /// <param name="minRadius">Minimum distance from the center</param>
    /// <param name="maxRadius">Maximum distance from the center</param>
    /// <returns>A position falling within the specified region</returns>
    public static Vector3 ChooseRandomPosition(Vector3 center, float minAngle, float maxAngle, float minRadius, float maxRadius)
    {
        float radius = minRadius;
        float angle = minAngle;

        if (maxRadius > minRadius)
        {
            // Pick a random radius
            radius = UnityEngine.Random.Range(minRadius, maxRadius);
        }

        if (maxAngle > minAngle)
        {
            // Pick a random angle
            angle = UnityEngine.Random.Range(minAngle, maxAngle);
        }

        // Center position + forward vector rotated around the Y axis by "angle" degrees, multiplies by "radius"
        return center + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;
    }


    /// <summary>
    /// Remove all fish from the area
    /// </summary>
    private void RemoveAllFish()
    {
        if (fishList != null)
        {
            for (int i = 0; i < fishList.Count; i++)
            {
                if (fishList[i] != null)
                {
                    Destroy(fishList[i]);
                }
            }
        }

        fishList = new List<GameObject>();
    }


    /// <summary>
    /// Place the penguin in the area
    /// </summary>
    private void PlacePenguin(GameObject penguim)
    {
        Rigidbody rigidbody = penguinAgent.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        penguinAgent.transform.position = ChooseRandomPosition(transform.position, 0f, 360f, 0f, 9f) + Vector3.up * .5f;
        penguinAgent.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
    }

    /// <summary>
    /// Place the baby in the area
    /// </summary>
    private void PlaceBaby(GameObject baby)
    {
        Rigidbody rigidbody = baby.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        baby.transform.position = ChooseRandomPosition(transform.position, -45f, 45f, 4f, 9f) + Vector3.up * .5f;
        baby.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    /// <summary>
    /// Spawn some number of fish in the area and set their swim speed
    /// </summary>
    /// <param name="count">The number to spawn</param>
    /// <param name="fishSpeed">The swim speed</param>
    private void SpawnFish(int count, float fishSpeed)
    {
        for (int i = 0; i < count; i++)
        {
            // Spawn and place the fish
            GameObject fishObject = Instantiate<GameObject>(fishPrefab.gameObject);
            fishObject.transform.position = ChooseRandomPosition(transform.position, 100f, 260f, 2f, 13f) + Vector3.up * .5f;
            fishObject.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);

            // Set the fish's parent to this area's transform
            fishObject.transform.SetParent(transform);

            // Keep track of the fish
            fishList.Add(fishObject);

            // Set the fish speed
            fishObject.GetComponent<Fish>().fishSpeed = fishSpeed;
        }
    }

}
