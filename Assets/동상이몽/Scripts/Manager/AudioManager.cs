using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("BGM Clips")]
    public AudioClip startMenuBGM;
    public AudioClip[] chapterBGMs;   // 7개
    public AudioClip[] endingBGMs;    // 4개

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 넘어가도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ✅ 시작 화면
    public void PlayStartMenu()
    {
        PlayBGM(startMenuBGM);
    }

    // ✅ 챕터 BGM
    public void PlayChapter(int chapterIndex)
    {
        PlayBGM(chapterBGMs[chapterIndex]);
    }

    // ✅ 엔딩 BGM
    public void PlayEnding(int endingIndex)
    {
        PlayBGM(endingBGMs[endingIndex]);
    }

    // ✅ 공통 BGM 재생 함수
    void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip) return;

        bgmSource.clip = clip;
        bgmSource.Play();
    }

    // ✅ 효과음 재생
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}