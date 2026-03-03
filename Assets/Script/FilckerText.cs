using UnityEngine;
using TMPro;

public class FlickerText : MonoBehaviour
{
    TextMeshProUGUI text;
    float timer;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > Random.Range(0.1f, 0.5f))
        {
            text.enabled = !text.enabled;
            timer = 0;
        }
    }
}