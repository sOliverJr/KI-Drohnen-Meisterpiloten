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
        resetGoalCounter = 0;
        _winMaterial = (Material)Resources.Load("Materials/winMaterial", typeof(Material));
        _crashLooseMaterial = (Material)Resources.Load("Materials/crashLooseMaterial", typeof(Material));

        goalController.SetGoal();


        for (int i = 0; i < 10; i++)
        {
            List<float> row = new List<float>();

            rewards.Add(row);
        }

    }

    //Zurücksetzen der Episode
    public override void OnEpisodeBegin()
    {
        goalcounter = 0;
        drone.ResetVelocity();
        transform.localPosition = startPosition;
        startTime = Time.time;
        transform.rotation = Quaternion.identity;
    }

    //Setze die vier Ausgabeparameter
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Convert.ToInt32(Input.GetKey(KeyCode.Q));
        continuousActions[1] = Convert.ToInt32(Input.GetKey(KeyCode.W));
        continuousActions[2] = Convert.ToInt32(Input.GetKey(KeyCode.A));
        continuousActions[3] = Convert.ToInt32(Input.GetKey(KeyCode.S));
    }

    //Festlegen der Inputpsrameter
    public override void CollectObservations(VectorSensor sensor)
    {

        //Richtung und Abstand von Drohne zum Ziel
        float distanceX = goalController.goalCenter.transform.position.x - transform.position.x;
        float distanceY = goalController.goalCenter.transform.position.y - transform.position.y;
        float distanceZ = goalController.goalCenter.transform.position.z - transform.position.z;

        sensor.AddObservation(distanceX);
        sensor.AddObservation(distanceY);
        sensor.AddObservation(distanceZ);

        //rotation der Drohne
        sensor.AddObservation(transform.rotation.x);
        sensor.AddObservation(transform.rotation.y);
        sensor.AddObservation(transform.rotation.z);
        sensor.AddObservation(transform.rotation.w);

        Rigidbody rb = drone.GetComponent<Rigidbody>();

        //Geschwindigkeit und Richtung der Drohne
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);
        sensor.AddObservation(rb.velocity.z);

        //Geschwindigkeit und Richtung der Drehbewegung der Drohne
        sensor.AddObservation(rb.angularVelocity.x);
        sensor.AddObservation(rb.angularVelocity.y);
        sensor.AddObservation(rb.angularVelocity.z);
    }

    //Gibt den Agenten Belohnungen im Flugverlauf
    public override void OnActionReceived(ActionBuffers actions)
    {
        //Umwandeln der Ausgabewerte ins Intervall von 0 bis 1 um 
        float rotorOneActive = actions.ContinuousActions[0] / 2 + 0.5f;
        float rotorTwoActive = actions.ContinuousActions[1] / 2 + 0.5f;
        float rotorThreeActive = actions.ContinuousActions[2] / 2 + 0.5f;
        float rotorFourActive = actions.ContinuousActions[3] / 2 + 0.5f;

        //ÜBergabe der Ausgabewerte auf die Rotoren
        drone.FrontLeftRotor(rotorOneActive);
        drone.FrontRightRotor(rotorTwoActive);
        drone.RearLeftRotor(rotorThreeActive);
        drone.RearRightRotor(rotorFourActive);

        Rigidbody rb = drone.GetComponent<Rigidbody>();


        Vector3 currentDirection = transform.position - goalController.goalCenter.transform.position;
        float currentDistance = currentDirection.magnitude;


        //Gewichtung der Belohungen, um diese im Trainingsverlauf zu ändern
        float factorvelocity = 1;
        float facotrangvelocity = 1;
        float factorgoal = 1;
        float factordistance = 1;
        float factorup = 1;

        //Belohung: Abstand zwischen Drohne und Ziel wird kleiner
        if ((_lastDistance - currentDistance) <= 3 && (_lastDistance - currentDistance) > 0.5f)
        {
            AddReward(0.01f * factordistance);
            rewarddistance += (0.01f * factordistance);
        }
        else
        {
            AddReward(-0.01f * factordistance);
            rewarddistance += (-0.01f * factordistance);
        }

        //drone soll richtig ausgerichtet sein
        //einmal richtung Ziel zeigen 
        AddReward(Vector3.Dot(-transform.up, Vector3.Normalize(currentDirection)) / 70 * factorgoal);
        rewardrotationgoal += Vector3.Dot(-transform.up, Vector3.Normalize(currentDirection)) / 70 * factorgoal;

        //nach oben zeigen, um nicht abzustürzen
        AddReward(((Vector3.Dot(transform.up, new Vector3(0, 1, 0))) - 0.75f) * 4 / 70 * factorup);
        rewardrotationforward += (((Vector3.Dot(transform.up, new Vector3(0, 1, 0))) - 0.75f) * 4 / 70 * factorup);


        //Belohnung der Geschwindigkeit
        if (rb.velocity.magnitude < 3 && rb.velocity.magnitude > 1)
        {
            AddReward((0.01f * factorvelocity));
            rewardvelocity += (0.01f * factorvelocity);
        }
        else if (rb.velocity.magnitude < 5)
        {
            AddReward((0.001f * factorvelocity));
            rewardvelocity += (0.001f * factorvelocity);
        }
        else
        {
            AddReward((-0.01f * factorvelocity));
            rewardvelocity -= (0.01f * factorvelocity);
        }

        //Belohnung der Rotationsgeschwindigkeit der Drohne
        if (rb.angularVelocity.magnitude < 0.5)
        {
            AddReward((0.01f * facotrangvelocity));
            rewardangvelocity += (0.01f * facotrangvelocity);
        }
        else if (rb.angularVelocity.magnitude < 1)
        {
            rewardangvelocity += (0.001f * facotrangvelocity);
            AddReward((0.001f * facotrangvelocity));
        }
        else if (rb.angularVelocity.magnitude > 3.5f)
        {
            AddReward(-0.1f * facotrangvelocity);
            rewardangvelocity -= (0.1f * facotrangvelocity);
        }
        else
        {
            AddReward((-0.01f * facotrangvelocity));
            rewardangvelocity -= (0.01f * facotrangvelocity);
        }

        //speichere den aktuellen Abstand zwischen Drohne und Ziel zum Vergleich für den nächsten Frame
        _lastDistance = Vector3.Distance(transform.position, goalController.goalCenter.transform.position);
    }

    //Bei eintreten ins Ziel 
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject == goalController.goal)
        {
            Rigidbody rb = transform.gameObject.GetComponent<Rigidbody>();
            goalcounter += 1;

            //Gewichtung Belohung für die Zielerreichung
            float factorgoal = 5;

            //Belohnung für eine möglichst geringe Geschwindigkeit
            if (rb.velocity.magnitude < 5)
            {
                AddReward(factorgoal / (rb.velocity.magnitude * 2) + 1);
                rewardgoalreached.Add((factorgoal / (rb.velocity.magnitude * 2) + 1));
            }


            //Belohnung für eine möglichst geringe Rotationsgeschwindigkeit
            if (rb.angularVelocity.magnitude < 2)
            {
                AddReward(factorgoal / (rb.angularVelocity.magnitude * 4) + 1);
                rewardgoalreached.Add((factorgoal / (rb.angularVelocity.magnitude * 4) + 1));
            }

            //Belohung für die möglichst waagerechte Lage der Drohne
            if (Vector3.Dot(transform.up, new Vector3(0, 1, 0)) > 0.5f)
            {
                AddReward(factorgoal / ((Vector3.Dot(transform.up, new Vector3(0, -1, 0)) + 2) * 6));
                rewardgoalreached.Add((factorgoal / ((Vector3.Dot(transform.up, new Vector3(0, -1, 0)) + 2) * 6)));
            }

            //Belohung fürs erreichen des Ziels
            AddReward(factorgoal * goalcounter);
            rewardgoalreached.Add(factorgoal * goalcounter);

            //Boden grün färben
            floor.material = _winMaterial;


            //gesamtbelohung für das jeweilig erreichte Ziel speichern, sollte fürs Training auskommentiert werden und dient zur Einschätzung
            //des Trainingfortschrittes.
            rewards[goalcounter].Add(GetCumulativeReward());
            goalController.SetGoal();
        }
        //nach dem 10. Ziel die Episode neu starten
        if (goalcounter >= 10)
            EndEpisode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Bestrafung für die Kollision mit der Wand, Boden oder Decke
        AddReward(-10);
        floor.material = _crashLooseMaterial;
        
        //im Training wird die Position des Ziels geändert nach 100 mal verfehlen dieses
        resetGoalCounter++;
        if (resetGoalCounter > 100)
        {
            resetGoalCounter = 0;
            goalController.SetGoal();
        }

        if (goalcounter == 0)
        {
            //gesamtbelohung wenn kein Ziel erreicht wurde
            rewards[goalcounter].Add(GetCumulativeReward());
        }

        //DUrchschnitt für 0,1,2,... erreichte Ziele berechnen. 
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

        // Durchschnitt für 0,1,2,... erreichte Ziele ausgeben
        //sollte fürs training auskommentiert werden und dient zur Einschätzung des Trainingfortschrittes.
        string output = "Averages: ";
        foreach (float average in averages)
        {
            output = output + average + "; ";
        }
        print(output);


        String goalstring = "";
        foreach (float goal in rewardgoalreached)
        {
            goalstring += ", " + goal;

        }

        //ausgeben der jeweiligen Belohungen für die verschiedenen Bewertungsmetriken
        //sollte fürs training auskommentiert werden und dient zur Einschätzung des Trainingfortschrittes.
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