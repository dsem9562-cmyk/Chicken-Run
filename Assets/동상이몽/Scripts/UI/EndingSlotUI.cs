using UnityEngine;
using UnityEngine.UI;

public class EndingSlotManager : MonoBehaviour
{
    public Image[] slotImages;          // 슬롯 4개
    public Sprite[] lockedSprites;      // 잠김 이미지 4개
    public Sprite[] unlockedSprites;    // 해금 이미지 4개

    void Awake()
    {
        SetupSlotButtons();
    }

    void OnEnable()
    {
        RefreshSlots();
    }

    void SetupSlotButtons()
    {
        if (slotImages == null) return;

        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i] == null) continue;

            var button = slotImages[i].GetComponent<Button>();
            if (button == null)
                button = slotImages[i].gameObject.AddComponent<Button>();

            button.targetGraphic = slotImages[i];
            button.transition = Selectable.Transition.None;
            button.onClick.RemoveAllListeners();

            int index = i;
            button.onClick.AddListener(() => OnSlotClicked(index));
        }
    }

    public void OnSlotClicked(int index)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[EndingSlotManager] GameManager.Instance가 null입니다.");
            return;
        }

        if (index < 0 || index >= slotImages.Length)
            return;

        var type = (GameManager.EndingType)index;
        if (!GameManager.Instance.IsEndingUnlocked(type))
        {
            Debug.Log("[EndingSlotManager] 아직 해금되지 않은 엔딩입니다: " + type);
            return;
        }

        GameManager.Instance.PlayEndingFromGallery(type);
    }

    public void RefreshSlots()
    {
        if (slotImages == null) return;

        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i] == null) continue;

            bool unlocked = IsSlotUnlocked(i);
            var image = slotImages[i];

            if (unlocked &&
                unlockedSprites != null &&
                i < unlockedSprites.Length &&
                unlockedSprites[i] != null)
            {
                image.sprite = unlockedSprites[i];
                image.color = Color.white;
            }
            else if (lockedSprites != null &&
                     i < lockedSprites.Length &&
                     lockedSprites[i] != null)
            {
                image.sprite = lockedSprites[i];
                image.color = Color.white;
            }
            else
            {
                image.color = new Color(1f, 1f, 1f, 0f);
            }

            var button = image.GetComponent<Button>();
            if (button != null)
                button.interactable = unlocked;
        }
    }

    bool IsSlotUnlocked(int index)
    {
        var type = (GameManager.EndingType)index;

        if (GameManager.Instance != null)
            return GameManager.Instance.IsEndingUnlocked(type);

        return PlayerPrefs.GetInt("Ending_" + type.ToString(), 0) == 1;
    }
}