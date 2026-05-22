using UnityEngine;
using UnityEngine.SceneManagement;

public class SuperBossKiller : MonoBehaviour
{
    void Awake()
    {
        Time.timeScale = 1f; // 🔥 강제 초기화
    }

    public GameObject boss;
    public string endingSceneName = "EndingScene";
    public float delay = 1.5f;

    private bool isKilled = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && !isKilled)
        {
            isKilled = true;
            Debug.Log("CHEAT: Boss killed! Forcing ending...");

            if (boss != null)
            {
                Destroy(boss);
            }

            Invoke("LoadEnding", delay);
        }

        if (GameManager.Instance != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) GameManager.Instance.SetItem(GameManager.EndingType.UniverseConqueror);
            if (Input.GetKeyDown(KeyCode.Alpha2)) GameManager.Instance.SetItem(GameManager.EndingType.ChickenGod);
            if (Input.GetKeyDown(KeyCode.Alpha3)) GameManager.Instance.SetItem(GameManager.EndingType.BuddhaChicken);
            if (Input.GetKeyDown(KeyCode.Alpha4)) GameManager.Instance.SetItem(GameManager.EndingType.HotSpicy);
        }
    }

    void LoadEnding()
    {
        Debug.Log("CHEAT: Loading Ending Scene...");
        SceneManager.LoadScene(endingSceneName);
    }
}