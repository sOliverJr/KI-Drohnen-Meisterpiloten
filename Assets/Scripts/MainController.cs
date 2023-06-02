using UnityEngine;

// sealed -> no inheritance
public sealed class MainController : MonoBehaviour
{
    public static MainController Instance;
    // default boolean = false
    public bool goalCollider = true;
    public bool staticGoal = true;
    public bool rotatingGoal; 
    public Vector3 goalStartPosition = new (0, 1.5f, 0);

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (!staticGoal)
        {
            rotatingGoal = true;
        }

        if (rotatingGoal)
        {
            staticGoal = false;
        }
    }
}