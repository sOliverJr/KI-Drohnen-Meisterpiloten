using UnityEngine;
using Random = UnityEngine.Random;

public class GoalController : MonoBehaviour
{
    public GameObject goal;
    public GameObject goalCenter;

    
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
        int randomX = Random.Range(-45, 45);
        int randomY = Random.Range(0, 359);

        goalCenter.transform.rotation = Quaternion.Euler(randomX, randomY, 0f);
        // currentGoal = Instantiate(goal, goal.transform.position,  Quaternion.Euler(randomX, randomY, 0));
    }

    public void SetGoal()
    {
        if (MainController.Instance.goalCollider && MainController.Instance.rotatingGoal)
        {
            RandomiseGoalPosition();
        }
    }

    private void SetGoalBoxCollider(bool boolean)
    {
        goal.GetComponent<Collider>().enabled = boolean;
    }
}