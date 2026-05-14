using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    // 当有物体进入这个触发器时执行
    private void OnTriggerEnter(Collider other)
    {
        // 检查碰到门的是不是玩家
        if (other.CompareTag("Player"))
        {
            // 调用 GameManager 里的获胜界面
            GameManager.instance.ShowVictory();
        }
    }
}