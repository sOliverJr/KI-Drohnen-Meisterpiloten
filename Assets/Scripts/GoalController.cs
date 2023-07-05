using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GoalController : MonoBehaviour
{
    public GameObject goal;
    public GameObject goalCenter;
    public GameObject floor;
    public GameObject ceiling;
    public GameObject front;
    public GameObject rear;
    public GameObject left;
    public GameObject right;


    
    private void Start()
    {
        SetStaticGoalPosition();
    }

    private void FixedUpdate()
    {
        SetGoalBoxCollider(MainController.Instance.goalCollider);
    }

    private void SetStaticGoalPosition()
    {
        goal.transform.localPosition = MainController.Instance.goalStartPosition;
    }

    private void RandomiseGoalPosition()
    {
        
        float randomX = Random.Range(right.transform.position.x -10, left.transform.position.x +10) ;
       
        float randomZ = Random.Range(front.transform.position.z-10, rear.transform.position.z + 10);

        goalCenter.transform.position = new Vector3(randomX, 30, randomZ);
       
    }

    public void SetGoal()
    {           
            RandomiseGoalPosition();
    }

    private void SetGoalBoxCollider(bool boolean)
    {
        goal.GetComponent<Collider>().enabled = boolean;
    }
}