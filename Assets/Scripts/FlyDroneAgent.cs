using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Security;

public class FlyDroneAgent : Agent
{
    public GoalController goalController;

    [SerializeField] private MeshRenderer floor;
    [SerializeField] public DroneController drone;
    [SerializeField] public Vector3 startPosition;
    private Vector3 goalstartposition;
    private Vector3 dronestarposition;
    private float _lastDistance;

    private Material _winMaterial;
    private Material _crashLooseMaterial;
    private Material _flipLooseMaterial;
    private int resetGoalCounter;
    private float startTime;


    private int goalcounter;


    //test
    private float minup = 0;
    private float maxup = 0;
    private float minforward = 0;
    private float maxforward = 0;
    private float lastmax = 0;
    private float lastmin = 0;

    private float maxoutput = 0;
    private float minoutput = 0;

    private float rewarddistance = 0;
    private float rewardrotationgoal = 0;
    private float rewardrotationforward = 0;
    private float rewardvelocity = 0;
    private float rewardangvelocity = 0;
    private float rewardtime = 0;
    private List<float> rewardgoalreached = new List<float>();

    private float velocitymin = 0;
    private float velocitymax = 0;
    private float angvelocitymin = 0;
    private float angvelocitymax = 0;

    private List<List<float>> rewards = new List<List<float>>();

    private float maxtime = 0;

    public void Start()
    {
        goalstartposition = goalController.goal.transform.position;
        dronestarposition = transform.position;
        resetGoalCounter = 0;
        _winMaterial = (Material)Resources.Load("Materials/winMaterial", typeof(Material));
        _crashLooseMaterial = (Material)Resources.Load("Materials/crashLooseMaterial", typeof(Material));
        _flipLooseMaterial = (Material)Resources.Load("Materials/flipLooseMaterial", typeof(Material));
        goalController.SetGoal();


        for (int i = 0; i < 10; i++)
        {
            List<float> row = new List<float>();

            rewards.Add(row);
        }

    }

    public override void OnEpisodeBegin()
    {
        goalcounter = 0;
        drone.ResetVelocity();
        transform.localPosition = startPosition;

        startTime = Time.time;

        transform.rotation = Quaternion.identity;
        print("newEpisode");



    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Convert.ToInt32(Input.GetKey(KeyCode.Q));
        continuousActions[1] = Convert.ToInt32(Input.GetKey(KeyCode.W));
        continuousActions[2] = Convert.ToInt32(Input.GetKey(KeyCode.A));
        continuousActions[3] = Convert.ToInt32(Input.GetKey(KeyCode.S));



    }

    public override void CollectObservations(VectorSensor sensor)
    {


        float distanceX = goalController.goalCenter.transform.position.x - transform.position.x;
        float distanceY = goalController.goalCenter.transform.position.y - transform.position.y;
        float distanceZ = goalController.goalCenter.transform.position.z - transform.position.z;

        sensor.AddObservation(distanceX);
        sensor.AddObservation(distanceY);
        sensor.AddObservation(distanceZ);


        sensor.AddObservation(transform.rotation.x);
        sensor.AddObservation(transform.rotation.y);
        sensor.AddObservation(transform.rotation.z);
        sensor.AddObservation(transform.rotation.w);

        Rigidbody rb = drone.GetComponent<Rigidbody>();
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);
        sensor.AddObservation(rb.velocity.z);

        sensor.AddObservation(rb.angularVelocity.x);
        sensor.AddObservation(rb.angularVelocity.y);
        sensor.AddObservation(rb.angularVelocity.z);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        float rotorOneActive = actions.ContinuousActions[0] / 2 + 0.5f;
        float rotorTwoActive = actions.ContinuousActions[1] / 2 + 0.5f;
        float rotorThreeActive = actions.ContinuousActions[2] / 2 + 0.5f;
        float rotorFourActive = actions.ContinuousActions[3] / 2 + 0.5f;


        if (rotorOneActive < minoutput)
        {
            minoutput = rotorOneActive;
        }

        if (rotorOneActive > maxoutput)
        {
            maxoutput = rotorOneActive;
        }

        drone.FrontLeftRotor(rotorOneActive);
        drone.FrontRightRotor(rotorTwoActive);
        drone.RearLeftRotor(rotorThreeActive);
        drone.RearRightRotor(rotorFourActive);

        Rigidbody rb = drone.GetComponent<Rigidbody>();


        Vector3 currentDirection = transform.position - goalController.goalCenter.transform.position;
        float currentDistance = currentDirection.magnitude;



             float factorvelocity = 0.5f;
                float facotrangvelocity = 2;
                float factorgoal = 1;
                float factordistance = 3;
                float factorup = 1;
/*
        //nach 700000
        float factorvelocity = 0.3f;
        float facotrangvelocity = 0.07f;
        float factorgoal = 1;
        float factordistance = 13;
        float factorup = 3;
*/
        /* float factorvelocity = 0.1f;
        float facotrangvelocity = 0.2f;
        float factorgoal = 2;
        float factordistance = 20;
        float factorup = 5;*/


        if ((_lastDistance - currentDistance) <= 3 && (_lastDistance - currentDistance) >0.5f)
        {
            AddReward(0.01f*factordistance);
            rewarddistance += (0.01f * factordistance);
        }

        else
        {
            AddReward(-0.01f * factordistance);
            rewarddistance += (-0.01f * factordistance);

        }
        /*hiermit werden drohnen extrem langsam
        if ((_lastDistance - currentDistance) <= 0)
        {
            AddReward(-0.1f * factordistance);
            rewarddistance += (-0.1f * factordistance);
        }

        else
        {
            AddReward(0.03f * factordistance);

        }*/

        if (maxforward < _lastDistance - currentDistance)
            maxforward = _lastDistance - currentDistance;
        //drone soll richtig ausgerichtet sein
        //einmal richtung ziel zeigen 
        AddReward(Vector3.Dot(-transform.up, Vector3.Normalize(currentDirection)) / 70 * factorgoal);
        rewardrotationgoal += Vector3.Dot(-transform.up, Vector3.Normalize(currentDirection)) / 70 * factorgoal;

        //nach oben zeigen, um nicht abzustürzen

        AddReward(((Vector3.Dot(transform.up, new Vector3(0, 1, 0))) - 0.75f) * 4 / 70* factorup);
        //print("up " + ((Vector3.Dot(transform.up, new Vector3(0, 1, 0))) - 0.75f) * 4 / 70);
        rewardrotationforward += (((Vector3.Dot(transform.up, new Vector3(0, 1, 0))) - 0.75f) * 4 / 70* factorup);
        //print("up" + ((Vector3.Dot(transform.up, new Vector3(0, 1, 0)) - 0.75f) * 4));
        

        //velocity
        if (rb.velocity.magnitude < 3 && rb.velocity.magnitude >1)
        {
            AddReward((0.01f / factorvelocity));
            rewardvelocity += (0.01f / factorvelocity);
        }
        else if (rb.velocity.magnitude < 5)
        {
            AddReward((0.001f / factorvelocity));
            rewardvelocity += (0.001f / factorvelocity);
        }
        else
        {
            AddReward((-0.01f / factorvelocity));
            rewardvelocity -= (0.01f / factorvelocity);
        }

        //angualr velocity
        if (rb.angularVelocity.magnitude < 0.5)
        {
            AddReward((0.01f / facotrangvelocity));
            rewardangvelocity += (0.01f / facotrangvelocity);
        }


        else if (rb.angularVelocity.magnitude < 1)
        {
            rewardangvelocity += (0.001f / facotrangvelocity);
            AddReward((0.001f / facotrangvelocity));
        }

        else if(rb.angularVelocity.magnitude > 3.5f)
        {
            AddReward(-0.1f / facotrangvelocity);
            rewardangvelocity -= (0.1f / facotrangvelocity);
        }
        else
        {
            AddReward((-0.01f / facotrangvelocity));
            rewardangvelocity -= (0.01f / facotrangvelocity);
        }


        

        if (minup > Vector3.Dot(transform.up, new Vector3(0, 1, 0)))
            minup = Vector3.Dot(transform.up, new Vector3(0, 1, 0));

        if (maxup < Vector3.Dot(transform.up, new Vector3(0, 1, 0)))
            maxup = Vector3.Dot(transform.up, new Vector3(0, 1, 0));

        if (minforward > Vector3.Dot(transform.up, Vector3.Normalize(new Vector3(currentDirection.x, transform.up.y, currentDirection.z))))
            minforward = Vector3.Dot(transform.up, Vector3.Normalize(new Vector3(currentDirection.x, transform.up.y, currentDirection.z)));

        if (maxforward < Vector3.Dot(transform.up, Vector3.Normalize(new Vector3(currentDirection.x, transform.up.y, currentDirection.z))))
            maxforward = Vector3.Dot(transform.up, Vector3.Normalize(new Vector3(currentDirection.x, transform.up.y, currentDirection.z)));



        _lastDistance = Vector3.Distance(transform.position, goalController.goalCenter.transform.position);

        //print("velocityrb" + rb.velocity.magnitude);

        if(Time.time - startTime > maxtime)
            maxtime = Time.time- startTime;
        //print("Time: " + maxtime + ", " + Time.time + ", " + startTime);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (Time.time - startTime >= 0.01f)
        {
            //print("triggerenter");


            //print("test");

            //make reward dependend on entrance speed???
            //print(other.gameObject.name);
            if (other.gameObject == goalController.goal) // Goal
            {
                Rigidbody rb = transform.gameObject.GetComponent<Rigidbody>();
                goalcounter += 1;

                float factorgoal = 5;

                if (rb.velocity.magnitude < 5)
                {
                    AddReward(factorgoal / (rb.velocity.magnitude * 2) + 1);
                    rewardgoalreached.Add( (factorgoal / (rb.velocity.magnitude * 2) + 1));
                }
                    


               if (rb.angularVelocity.magnitude < 2)
                {
                    AddReward(factorgoal / (rb.angularVelocity.magnitude * 4) + 1);
                    rewardgoalreached.Add((factorgoal / (rb.angularVelocity.magnitude * 4) + 1));
                }
                    

                if (Vector3.Dot(transform.up, new Vector3(0, 1, 0)) > 0.5f)
                {
                    AddReward(factorgoal / ((Vector3.Dot(transform.up, new Vector3(0, -1, 0)) + 2) * 6));
                    rewardgoalreached.Add((factorgoal / ((Vector3.Dot(transform.up, new Vector3(0, -1, 0)) + 2) * 6)));
                }
                  

                AddReward(factorgoal * goalcounter);
                rewardgoalreached.Add(factorgoal * goalcounter);


                floor.material = _winMaterial;
                //print("Goalcounter:" + goalcounter);
                 rewards[goalcounter].Add(GetCumulativeReward());
                goalController.SetGoal();
            }
            if (goalcounter >= 10)
                EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {


        if (Time.time - startTime >= 0.01f)
        {


          //  print("wrong Goal");
            AddReward(-10);
            floor.material = _crashLooseMaterial;
            resetGoalCounter++;
            if (resetGoalCounter > 100)
            {
                resetGoalCounter = 0;
                goalController.SetGoal();
            }


            if (goalcounter == 0)
            {
                 rewards[goalcounter].Add(GetCumulativeReward());
                print("Goalcounter:" + goalcounter);
            }
            List<float> averages = new List<float>();
            foreach (List<float> row in rewards)
            {
                float sum = 0.0f;
                foreach (float value in row)
                {
                    sum += value;
                }
                float average = sum / row.Count;
                averages.Add(average);
            }

            // Display the averages
            string output = "Averages: ";
            foreach (float average in averages)
            {
                output = output + average + "; ";
            }
            //  print(output);


            String goalstring = "";
            foreach(float goal in rewardgoalreached)
            {
                goalstring += ", " + goal;

            }
            //print(GetCumulativeReward());
            print("Reward: Distance: " + rewarddistance + ", Goal: " + rewardrotationgoal + ", up: " + rewardrotationforward + ", velocity" + rewardvelocity + ", angvelocity: " + rewardangvelocity + ", goalreached: " + goalstring);
            rewarddistance = 0;
            rewardrotationgoal = 0;
            rewardrotationforward = 0;
            rewardvelocity = 0;
            rewardangvelocity = 0;
            rewardtime = 0;
            rewardgoalreached = new List<float>();
            EndEpisode();
        }
    }
}