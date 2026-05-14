using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MonsterAI : MonoBehaviour
{
    // === 新增：AI 状态机枚举 ===
    public enum AIState { Patrol, Chase }

    [Header("AI State")]
    public AIState currentState = AIState.Patrol; // 默认状态为巡逻

    [Header("Patrol Settings")]
    public Transform[] waypoints; // 存放我们讨论的那 3 个巡逻点
    private int currentWaypointIndex = 0;

    [Header("Detection Settings")]
    public float detectionRadius = 10f; // 发现玩家的距离
    public float loseRadius = 15f;      // 玩家逃脱的距离

    [Header("References")]
    public Transform player;
    private NavMeshAgent agent;
    private Animator anim;
    private bool isGameOver = false;

    [Header("Jumpscare Settings")]
    public AudioClip scareSound; // 尖叫声音频
    private AudioSource audioSource; // 播放器

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // 游戏开始，如果有巡逻点，就走向第一个点
        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void Update()
    {
        // === 完全保留你的贴脸杀核心逻辑 ===
        if (isGameOver)
        {
            // 强行夺取摄像机控制权！让玩家看着怪物的脸！
            if (Camera.main != null)
            {
                Vector3 monsterFacePosition = transform.position + Vector3.up * 2.0f;
                Camera.main.transform.LookAt(monsterFacePosition);
            }
            return; // 游戏结束，停止下面的寻路逻辑
        }

        // === 新增：状态机思考逻辑 ===
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case AIState.Patrol:
                PatrolUpdate();
                // 如果玩家进入侦测范围，切换到追击状态！
                if (distanceToPlayer <= detectionRadius)
                {
                    currentState = AIState.Chase;
                }
                break;

            case AIState.Chase:
                ChaseUpdate();
                // 如果玩家逃出丢失范围，切换回巡逻状态
                if (distanceToPlayer > loseRadius)
                {
                    currentState = AIState.Patrol;
                    // 回到巡逻状态时，走向离自己最近的那个巡逻点
                    if (waypoints.Length > 0)
                    {
                        agent.SetDestination(waypoints[currentWaypointIndex].position);
                    }
                }
                break;
        }
    }

    // 巡逻行为
    void PatrolUpdate()
    {
        if (waypoints.Length == 0) return;

        // 如果快要到达当前点，就切换下一个点
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    // 追击行为
    void ChaseUpdate()
    {
        // 正常追击逻辑
        agent.SetDestination(player.position);
    }

    // === 完全保留你原来的碰撞和倒计时逻辑 ===
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isGameOver)
        {
            isGameOver = true;

            // 1. 物理刹车
            if (agent != null)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }

            // 2. 触发攻击动画
            if (anim != null)
            {
                anim.SetTrigger("onAttack");
                anim.SetBool("isGameOver", true);
            }

            // 3. 播放尖叫音效！
            if (audioSource != null && scareSound != null)
            {
                audioSource.PlayOneShot(scareSound);
            }

            // 4. 启动 2 秒后回到主菜单的倒计时 (调用了我们做的 GameManager 死亡界面)
            StartCoroutine(ReturnToMenu());
        }
    }

    IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(2f);
        GameManager.instance.ShowGameOver();
    }
}