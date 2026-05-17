using UnityEngine;
using UnityEngine.SceneManagement;

public class BossWatcher : MonoBehaviour
{
    public GameObject boss;
    public string endingSceneName = "EndingScene";
    public float delayBeforeEnding = 1.5f;

    private bool isTriggered = false;

    void Update()
    {
        if (isTriggered) return;
        if (boss == null) return;

        if (!boss.activeInHierarchy)
        {
            isTriggered = true;
            Debug.Log("Boss disappeared! Loading ending scene...");
            Invoke("LoadEnding", delayBeforeEnding);
        }
    }

    void LoadEnding()
    {
        SceneManager.LoadScene(endingSceneName);
    }
}