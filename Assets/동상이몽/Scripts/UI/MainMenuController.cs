using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("패널들 - 여기에 드래그&드롭!")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;
    public GameObject gameEndingsPanel;
    public GameObject gameInfoPanel;

    [Header("게임 시작 시 이동할 씬 이름")]
    public string gameSceneName = "GameScene";
    // 👆 Inspector에서 실제 게임 씬 이름으로 바꾸면 됨
    // 예: "농장", "Stage1" 등

    // ===== 메인 메뉴 버튼들 =====

    public void OnStartGame()
    {
        // 씬 이름이 비어있으면 경고
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogError("게임 씬 이름이 비어있습니다! Inspector에서 설정하세요.");
            return;
        }

        // 씬이 Build Settings에 등록돼 있는지 확인
        if (Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogError($"'{gameSceneName}' 씬이 Build Settings에 추가되지 않았습니다!\n" +
                           "File → Build Settings → Add Open Scenes 로 추가하세요.");
        }
    }

    public void OnSettings()
    {
        HideAll();
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void OnCredits()
    {
        HideAll();
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    public void OnGameEndings()
    {
        HideAll();
        if (gameEndingsPanel != null) gameEndingsPanel.SetActive(true);
    }

    public void OnGameInfo()
    {
        HideAll();
        if (gameInfoPanel != null) gameInfoPanel.SetActive(true);
    }

    public void OnQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ===== 뒤로가기 =====

    public void ShowMainMenu()
    {
        HideAll();
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    void HideAll()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (gameEndingsPanel != null) gameEndingsPanel.SetActive(false);
        if (gameInfoPanel != null) gameInfoPanel.SetActive(false);
    }
}