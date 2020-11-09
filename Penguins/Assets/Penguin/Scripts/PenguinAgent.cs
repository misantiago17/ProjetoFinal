using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.Barracuda;
using Unity.MLAgents.Policies;

public class PenguinAgent: Agent
{
    new private Rigidbody rigidbody;

    public HUDController HUD;

    public NNModel HardModel;
    public NNModel MediumModel;
    public NNModel EasyModel;

    private PenguinConfig configurations;
    private PenguinArea penguinArea;

    private bool fistTimeOnScene = true;

    private void Awake() {
        configurations = this.GetComponent<PenguinConfig>();
        penguinArea = GetComponentInParent<PenguinArea>();
    }


    /// <summary>
    /// Initial setup, called when the agent is enabled
    /// It isn't called every round, just the first time the penguim appears
    /// </summary>
    public override void Initialize() {
        base.Initialize();
        rigidbody = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        // Request a decision every 5 steps. RequestDecision() automatically calls Request Action()
        // but for the steps in between, we need to call it explicitly to take action using the results
        // of the previous decision

        // The decisions are to get a fish or feed the baby
        if (StepCount % 5 == 0) {
            RequestDecision();

        // The action is to choose a direction for the penguin
        } else {
            RequestAction();
        }
    }


    /// <summary>
    /// Reset the agent and area when there are no more fishes in the pond or the episode steps reach it's maximum (5000)
    /// </summary>
    public override void OnEpisodeBegin() {

        configurations.isPenguinFull = false;

        // Show the Win/Lose Panel before going to the next round
        if (GameManager.gameMode == mode.competitive && fistTimeOnScene == false) {
            HUD.NextRoundPanel();

        } else {
            HUD.currentTime = 0;
            penguinArea.ResetArea();
        }

        fistTimeOnScene = false;

    }

    /// <summary>
    /// Perform actions based on a vector of numbers:
    /// vectorAction[0] - 0 stay put or 1 move foward
    /// vectorAction[1] - 0 doesn't rotate, 1 rotate negatively or 2 rotate positively
    /// </summary>
    /// <param name="vectorAction">The list of actions to take</param>
    public override void OnActionReceived(float[] vectorAction)
    {
        // Convert the first action to forward movement
        float forwardAmount = vectorAction[0];

        // Convert the second action to turning left or right
        float turnAmount = 0f;
        if (vectorAction[1] == 1f)
        {
            turnAmount = -1f;
        }
        else if (vectorAction[1] == 2f)
        {
            turnAmount = 1f;
        }

        // Apply movement
        rigidbody.MovePosition(transform.position + transform.forward * forwardAmount * configurations.moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * configurations.turnSpeed * Time.fixedDeltaTime);

        // Apply a tiny negative reward every step to encourage action and to achieve the goal faster
        if (MaxStep > 0) 
            AddReward(-1f / MaxStep);
    }

    /// <summary>
    /// Read inputs from the keyboard and convert them to a list of actions.
    /// This is called only when the player wants to control the agent and has set
    /// Behavior Type to "Heuristic Only" in the Behavior Parameters inspector.
    /// The first action "fowardAction" is by default 0 (don't move) until the player presses W
    /// </summary>
    /// <returns>A vectorAction array of floats that will be passed into <see cref="AgentAction(float[])"/></returns>
    public override void Heuristic(float[] actionsOut)
    {
        float forwardAction = 0f;
        float turnAction = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            // move forward
            forwardAction = 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            // turn left
            turnAction = 1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // turn right
            turnAction = 2f;
        }

        actionsOut[0] = forwardAction;
        actionsOut[1] = turnAction;
    }

    /// <summary>
    /// Collect all non-Raycast observations
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        // Whether the penguin has eaten a fish (1 float = 1 value)
        sensor.AddObservation(configurations.isPenguinFull);
        
        // Distance to the baby (1 float = 1 value)
        sensor.AddObservation(Vector3.Distance(configurations.Baby.transform.position, transform.position));

        // Direction to baby (1 Vector3 = 3 values)
        sensor.AddObservation((configurations.Baby.transform.position - transform.position).normalized);

        // Direction penguin is facing (1 Vector3 = 3 values)
        sensor.AddObservation(transform.forward);

        // 1 + 1 + 3 + 3 = 8 total values

        // The penguin also have raycast observations, they check what objects the penguins is seeing
    }

    /// <summary>
    /// Change the agent's model
    /// </summary>
    public void ChangeModel(difficulty difficulty) {

        if (difficulty == difficulty.easy) {
            this.GetComponent<BehaviorParameters>().Model = EasyModel;
        } else if (difficulty == difficulty.medium) {
            this.GetComponent<BehaviorParameters>().Model = MediumModel;
        } else {
            this.GetComponent<BehaviorParameters>().Model = HardModel;
        }

    }

}


