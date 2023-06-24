using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using TMPro;
using UnityEngine.Serialization;

public class DroneArea : MonoBehaviour
{
    [Tooltip("Drone")] public DroneAgent droneAgent;

    [Tooltip("Checkpunkt")] public Checkpoint checkpoint;

    /// <summary>
    /// Wird beim Start aufgerufen.
    /// </summary>
    private void Start()
    {
        //ResetArea();
    }
    
    /// <summary>
    /// Wird jeden Frame aufgerufen.
    /// </summary>
    private void Update()
    {
        
    }
    
    /// <summary>
    /// Setzt die Umgebung zurück.
    /// </summary>
    public void ResetArea()
    {
        //RemoveAllCheckpoints();
        //droneAgentOld.Reset();
        //SpawnCheckpoint();
    }

    /// <summary>
    /// Entfernt einen spezifischen Checkpunkt.
    /// </summary>
    /// <param name="checkpointObject">Checkpoint der entfernt werden soll.</param>
    private void RemoveSpecificCheckpoint(GameObject checkpointObject)
    {
        //Destroy(checkpointObject);
    }

    /// <summary>
    /// Entfernt alle Checkpunkte.
    /// </summary>
    private void RemoveAllCheckpoints()
    {
        //Destroy(GameObject.FindGameObjectWithTag("Checkpoint"));
    }

    /// <summary>
    /// Plaziert den Checkpunkt.
    /// </summary>
    private void SpawnCheckpoint()
    {
        //GameObject checkpointObject = Instantiate<GameObject>(checkpoint.gameObject);
        //checkpointObject.transform.localPosition = new Vector3(0f, 5f, 0f);

        // TODO: Verstehen
        //checkpointObject.transform.SetParent(transform);
    }

    /// <summary>
    /// Behandelt das Erreichen eines Checkpunktes.
    /// </summary>
    /// <param name="checkpoint">Erreichter Checkpunkt</param>
    public void CheckpointReached(GameObject checkpoint)
    {
        //RemoveSpecificCheckpoint(checkpoint);
    }
}