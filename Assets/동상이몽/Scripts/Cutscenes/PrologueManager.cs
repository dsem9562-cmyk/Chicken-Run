using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 게임 시작 시 프롤로그를 표시합니다.
/// - 비디오 프롤로그: VideoClip 재생 후 자동으로 게임 시작
/// - 텍스트 프롤로그: UI 텍스트 표시, 클릭/스페이스로 넘기기
/// 빈 오브젝트에 붙이고 Inspector에서 설정하세요.
/// </summary>
public class PrologueController : MonoBehaviour
{
    public enum PrologueType
    {
        None,       // 프롤로그 없음
        Video,      // 비디오 재생
        Text        // 텍스트 UI (여러 페이지)
    }

    [Header("프롤로그 타입")]
    public PrologueType prologueType = PrologueType.Text;

    [Header("비디오 프롤로그 (Video 선택 시)")]
    [Tooltip("프롤로그 영상 (mp4 등)")]
    public VideoClip prologueVideo;

    [Header("텍스트 프롤로그 (Text 선택 시)")]
    [Tooltip("표시할 텍스트들 (순서대로 한 페이지씩)")]
    [TextArea(3, 6)]
    public string[] prologueTexts = new string[]
    {
        "옛날 옛적, 양계장에 살던 닭 한 마리가 있었습니다.",
        "그 닭은 자유를 꿈꾸며 하늘을 날고 싶어 했죠.",
        "어느 날, 기회가 찾아왔습니다..."
    };
    [Tooltip("텍스트를 표시할 UI Text (없으면 자동 생성)")]
    public Text prologueTextUI;
    [Tooltip("배경 패널 (어두운 오버레이, 없으면 자동 생성)")]
    public Image backgroundPanel;
    [Tooltip("페이지당 표시 시간 (초, 0이면 클릭/스페이스로만 넘김)")]
    public float autoAdvanceDelay = 0f;

    [Header("공통 설정")]
    [Tooltip("프롤로그 중 게임 일시정지")]
    public bool pauseGameDuringPrologue = true;
    [Tooltip("프롤로그 종료 후 시작할 섹션 (1 = 플레이 시작)")]
    public int sectionToStartAfter = 1;

    private bool _isShowing;
    private int _currentTextIndex;
    private GameObject _canvasRoot;
    private static float _savedTimeScale = 1f;

    void Start()
    {
        if (prologueType == PrologueType.None)
        {
            StartGameDirectly();
            return;
        }

        if (prologueType == PrologueType.Video && prologueVideo != null)
        {
            StartCoroutine(PlayVideoPrologue());
            return;
        }

        if (prologueType == PrologueType.Text && prologueTexts != null && prologueTexts.Length > 0)
        {
            _isShowing = true;
            if (pauseGameDuringPrologue)
            {
                _savedTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            SetupTextUI();
            ShowTextPage(0);
            return;
        }

        // 설정이 없으면 바로 시작
        StartGameDirectly();
    }

    void Update()
    {
        if (!_isShowing || prologueType != PrologueType.Text) return;

        // 클릭 또는 스페이스로 다음 페이지
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            NextTextPage();
        }
    }

    IEnumerator PlayVideoPrologue()
    {
        _isShowing = true;
        if (pauseGameDuringPrologue)
        {
            _savedTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        var go = new GameObject("PrologueVideoPlayer");
        var vp = go.AddComponent<VideoPlayer>();
        vp.clip = prologueVideo;
        vp.playOnAwake = false;
        vp.isLooping = false;
        vp.targetCamera = Camera.main;
        vp.renderMode = VideoRenderMode.CameraNearPlane;
        vp.targetCameraAlpha = 1f;

        bool ended = false;
        vp.loopPointReached += _ => ended = true;
        vp.Play();

        while (!ended)
        {
            yield return null;
        }

        Destroy(go);
        EndPrologue();
    }

    void SetupTextUI()
    {
        // Canvas가 없으면 자동 생성
        var canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            _canvasRoot = new GameObject("PrologueCanvas");
            canvas = _canvasRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvasRoot.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _canvasRoot.AddComponent<GraphicRaycaster>();
        }

        // 배경 패널
        if (backgroundPanel == null)
        {
            var panelGo = new GameObject("ProloguePanel");
            panelGo.transform.SetParent(canvas.transform, false);
            backgroundPanel = panelGo.AddComponent<Image>();
            backgroundPanel.color = new Color(0, 0, 0, 0.85f);
            var rect = backgroundPanel.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }

        // 텍스트 UI
        if (prologueTextUI == null)
        {
            var textGo = new GameObject("PrologueText");
            textGo.transform.SetParent(backgroundPanel.transform, false);
            prologueTextUI = textGo.AddComponent<Text>();
            prologueTextUI.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            prologueTextUI.fontSize = 28;
            prologueTextUI.color = Color.white;
            prologueTextUI.alignment = TextAnchor.MiddleCenter;
            prologueTextUI.horizontalOverflow = HorizontalWrapMode.Wrap;
            prologueTextUI.verticalOverflow = VerticalWrapMode.Overflow;
            var textRect = prologueTextUI.rectTransform;
            textRect.anchorMin = new Vector2(0.1f, 0.3f);
            textRect.anchorMax = new Vector2(0.9f, 0.7f);
            textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        }
    }

    void ShowTextPage(int index)
    {
        _currentTextIndex = index;
        if (prologueTextUI != null)
        {
            prologueTextUI.text = prologueTexts[index];
        }

        if (autoAdvanceDelay > 0)
        {
            StartCoroutine(AutoAdvanceAfterDelay());
        }
    }

    IEnumerator AutoAdvanceAfterDelay()
    {
        yield return new WaitForSecondsRealtime(autoAdvanceDelay);
        NextTextPage();
    }

    void NextTextPage()
    {
        StopAllCoroutines();
        if (_currentTextIndex < prologueTexts.Length - 1)
        {
            ShowTextPage(_currentTextIndex + 1);
        }
        else
        {
            EndPrologue();
        }
    }

    void EndPrologue()
    {
        _isShowing = false;
        if (pauseGameDuringPrologue)
        {
            Time.timeScale = _savedTimeScale;
        }

        // UI 제거 (자동 생성한 것만)
        if (_canvasRoot != null)
        {
            Destroy(_canvasRoot);
        }
        else if (backgroundPanel != null && backgroundPanel.gameObject.name == "ProloguePanel")
        {
            Destroy(backgroundPanel.gameObject);
        }

        StartGameDirectly();
    }

    void StartGameDirectly()
    {
        // 프롤로그 종료 시 플레이어를 스폰 위치로 재설정 (씬 기본 위치 충돌 방지)
        var initializer = FindFirstObjectByType<GameStartInitializer>();
        if (initializer != null)
            initializer.ApplySpawnPosition();

        var cam = Camera.main?.GetComponent<CameraController>();
        if (cam != null)
        {
            cam.SetSection(sectionToStartAfter);
        }
    }
}
