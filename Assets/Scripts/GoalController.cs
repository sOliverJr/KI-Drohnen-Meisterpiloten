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
        float randomY = Random.Range(floor.transform.position.y + 10, ceiling.transform.position.y - 10);
        //float randomY = Random.Range(2f, 13.5f);
        //float randomY = Random.Range(-13.5f, 13.5f);
        float randomZ = Random.Range(front.transform.position.z-10, rear.transform.position.z + 10);

        goalCenter.transform.position = new Vector3(randomX, 30, randomZ);
        //goalCenter.transform.position = new Vector3(randomX, randomY, randomZ);


        // currentGoal = Instantiate(goal, goal.transform.position,  Quaternion.Euler(randomX, randomY, 0));
    }

    public void SetGoal()
    {
      //  if (MainController.Instance.goalCollider && MainController.Instance.rotatingGoal)
        //{
            
            RandomiseGoalPosition();
       // }
    }

    private void SetGoalBoxCollider(bool boolean)
    {
        goal.GetComponent<Collider>().enabled = boolean;
    }
}