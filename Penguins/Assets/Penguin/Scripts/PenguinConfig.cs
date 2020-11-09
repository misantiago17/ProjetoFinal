using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All the info about the pinguim stats
public class PenguinConfig : MonoBehaviour
{
    // show if the penguin is agent or player
    private bool isAgent = false;

    private PenguinArea penguinArea;

    private int fishFeed = 0;

    public int numFishFeed {
        get { return fishFeed; }
        set { fishFeed = value; }
    }

    private bool isFull = false; // If true, penguin has a full stomach
    public bool isPenguinFull {

        get { return isFull; }
        set { isFull = value; }
    }


    [Tooltip("How fast the agent moves forward")]
    public float moveSpeed = 5f;

    [Tooltip("How fast the agent turns")]
    public float turnSpeed = 180f;

    [Tooltip("Prefab of the heart that appears when the baby is fed")]
    public GameObject heartPrefab;

    [Tooltip("Prefab of the regurgitated fish that appears when the baby is fed")]
    public GameObject regurgitatedFishPrefab;

    [Tooltip("The baby that the penguin will feed")]
    public GameObject Baby;              

    private void Awake() {

        // check if the penguin is the agent or the player
        if (this.GetComponent<PenguinAgent>())
            isAgent = true;

        penguinArea = GetComponentInParent<PenguinArea>();

        if (!isAgent && GameManager.gameMode == mode.simulation) {
            this.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate() {

        // Test if the agent is close enought to feed the baby
        if (Vector3.Distance(transform.position, Baby.transform.position) < penguinArea.FeedRadius) {    
            // Close enought, try to feed the baby
            RegurgitateFish();
        }

    }

    /// <summary>
    /// Check if agent is full, if not, eat the fish and get a reward
    /// </summary>
    /// <param name="fishObject">The fish to eat</param>
    public void EatFish(GameObject fishObject) {

        if (isFull) return; // Can't eat another fish while full
        isFull = true;

        penguinArea.RemoveSpecificFish(fishObject);

        if (isAgent) {
            this.GetComponent<PenguinAgent>().AddReward(1f);
        }
    }

    /// <summary>
    /// Check if agent is full, if yes, feed the baby
    /// </summary>
    public void RegurgitateFish() {

        if (!isFull) return; // Nothing to regurgitate
        isFull = false;

        // Spawn regurgitated fish for 4 seconds
        GameObject regurgitatedFish = Instantiate<GameObject>(regurgitatedFishPrefab);
        regurgitatedFish.transform.parent = transform.parent;
        regurgitatedFish.transform.position = Baby.transform.position;
        Destroy(regurgitatedFish, 4f);

        // Spawn heart for 4 seconds
        GameObject heart = Instantiate<GameObject>(heartPrefab);
        heart.transform.parent = transform.parent;
        heart.transform.position = Baby.transform.position + Vector3.up;
        Destroy(heart, 4f);

        if (isAgent) {
            this.GetComponent<PenguinAgent>().AddReward(1f);
        }

        fishFeed += 1;

    }


    /// <summary>
    /// When the agent collides with something, take action
    /// </summary>
    /// <param name="collision">The collision info</param>
    private void OnCollisionEnter(Collision collision) {

        if (collision.transform.CompareTag("fish")) {
            EatFish(collision.gameObject);

        } else if (collision.transform.CompareTag("baby")) {
            RegurgitateFish();
        }
    }

}
