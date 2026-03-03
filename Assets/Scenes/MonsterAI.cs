using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MonsterAI : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;

    public float chaseDistance = 20f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < chaseDistance)
        {
            agent.SetDestination(player.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Game Over");

            StartCoroutine(ReturnToMenu());
        }
    }

    IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("StartScene");
    }
}
