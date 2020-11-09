using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Will be attached to each fish to make it swim
public class Fish : MonoBehaviour
{

    [Tooltip("The swim speed")]
    public float fishSpeed;

    private float randomizedSpeed = 0f; // Slightly altered speed that we will change randomly each time a new swim destination is picked
    private float nextActionTime = -1f; // Trigger the selection of a new swim destination
    private Vector3 targetPosition;     // position of the destination the fish is swimming toward

    /// <summary>
    /// Called every timestep
    /// </summary>
    private void FixedUpdate()
    {
        if (fishSpeed > 0f)
        {
            Swim();
        }
    }

    /// <summary>
    /// Swim between random positions
    /// </summary>
    private void Swim()
    {
        // If it's time for the next action, pick a new speed and destination
        // Else, swim toward the destination
        if (Time.fixedTime >= nextActionTime)
        {
            // Randomize speed
            randomizedSpeed = fishSpeed * UnityEngine.Random.Range(.5f, 1.5f);

            // Pick a random target
            targetPosition = PenguinArea.ChooseRandomPosition(transform.parent.position, 100f, 260f, 2f, 13f);

            // Rotate toward the target
            transform.rotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);

            // Calculate the time to get there
            float timetoGetThere = Vector3.Distance(transform.position, targetPosition) / randomizedSpeed;
            nextActionTime = Time.fixedTime + timetoGetThere;

        } else {

            // Make sure that the fish does not swim past the target
            Vector3 moveVector = randomizedSpeed * transform.forward * Time.fixedDeltaTime;

            if (moveVector.magnitude <= Vector3.Distance(transform.position, targetPosition))
            {
                transform.position += moveVector;

            } else
            {
                transform.position = targetPosition;
                nextActionTime = Time.fixedTime;
            }

        }
    }

}
