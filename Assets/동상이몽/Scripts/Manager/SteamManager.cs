using UnityEngine;
using Steamworks;

[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
    private static SteamManager m_instance;
    private bool m_bInitialized;

    void Awake()
    {
        if (m_instance != null) { Destroy(gameObject); return; }
        m_instance = this;
        DontDestroyOnLoad(gameObject);

        if (!Packsize.Test())
        {
            Debug.LogError("[Steamworks.NET] Packsize Test failed. 64비트/32비트 라이브러리 혼선이 있을 수 있습니다.");
            return;
        }

        m_bInitialized = SteamAPI.Init();
        if (!m_bInitialized)
        {
            Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed! 스팀 클라이언트가 실행 중인지 확인하세요.");
        }
        else
        {
            Debug.Log("[Steamworks.NET] Steam API 초기화 성공!");
        }
    }

    void Update()
    {
        if (m_bInitialized) SteamAPI.RunCallbacks();
    }

    void OnDestroy()
    {
        if (m_instance != this) return;
        m_instance = null;
        if (m_bInitialized) SteamAPI.Shutdown();
    }
}