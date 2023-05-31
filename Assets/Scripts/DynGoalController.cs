using UnityEngine;

public class DynGoalController : MonoBehaviour
{
    public GameObject goal;
    public GameObject goalCenter;
    public GameObject currentGoal;

    public bool useStaticGoal;
    public GameObject staticGoal;
    
    void Start()
    {
        /* 
        if (useStaticGoal)
            currentGoal = staticGoal;
        else
        {
            RandomiseGoalPosition();
            // goal.SetActive(false);
        }
        */
        
        RandomiseGoalPosition();
        currentGoal = goal;
    }

    public void RandomiseGoalPosition()
    {
        // int randomHeight = Random.Range(15, 30);
        // int randomX = Random.Range(-25, 25);
        // int randomZ = Random.Range(-25, 25);
        int randomX = Random.Range(-45, 45);
        int randomY = Random.Range(0, 359);

        goalCenter.transform.rotation = Quaternion.Euler(randomX, randomY, 0f);
        // currentGoal = Instantiate(goal, goal.transform.position,  Quaternion.Euler(randomX, randomY, 0));
    }
    
    
    public void DeleteGoal()
    {
        Destroy(currentGoal);
    }
}
