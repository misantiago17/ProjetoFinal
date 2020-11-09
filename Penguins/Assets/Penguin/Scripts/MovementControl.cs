using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Controls the movemento of the player penguin
/// The player moves with the same actions from the agent penguin
/// </summary>
public class MovementControl : MonoBehaviour
{
    private Rigidbody rigidbody;
    private PenguinConfig configurations;

    private void Awake() {

        rigidbody = this.GetComponent<Rigidbody>();
        configurations = this.GetComponent<PenguinConfig>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float forwardAmount = 0f;
        float turnAmount = 0f;
        if (Input.GetKey(KeyCode.W)) {

            // move forward
            forwardAmount = 1f;
        }
        if (Input.GetKey(KeyCode.A)) {

            // turn left
            turnAmount = 1f;

        } else if (Input.GetKey(KeyCode.D)) {

            // turn right
            turnAmount = -1f;
        }

        rigidbody.MovePosition(transform.position + transform.forward * forwardAmount * configurations.moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * configurations.turnSpeed * Time.fixedDeltaTime);

    }
}
