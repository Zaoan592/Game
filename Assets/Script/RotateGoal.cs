using UnityEngine;

public class RotateGoal : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }
}
