using UnityEngine;

public class DynGoalController : MonoBehaviour
{
    public static DynGoalController Instance;
    public GameObject goal;
    private GameObject _currentGoal;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        SpawnNewGoal();
    }

    public void SpawnNewGoal()
    {
        // int randomHeight = Random.Range(15, 30);
        // int randomX = Random.Range(-25, 25);
        // int randomZ = Random.Range(-25, 25);
        int randomHeight = Random.Range(18, 25);
        int randomX = Random.Range(-4, 4);
        int randomZ = Random.Range(-4, 4);

        _currentGoal = Instantiate(goal, new Vector3(randomX, randomHeight, randomZ), new Quaternion(0, 0, 0, 1));
    }
    
    
    public void DeleteGoal()
    {
        Destroy(_currentGoal);
    }
}
