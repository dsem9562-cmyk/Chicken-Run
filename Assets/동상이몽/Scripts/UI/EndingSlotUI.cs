using UnityEngine;
using UnityEngine.UI;

public class EndingSlotManager : MonoBehaviour
{
    public Image[] slotImages;          // 슬롯 4개
    public Sprite[] lockedSprites;      // 잠김 이미지 4개
    public Sprite[] unlockedSprites;    // 해금 이미지 4개

    void OnEnable()
    {
        RefreshSlots();
    }

    public void RefreshSlots()
    {
        if (GameManager.Instance == null) return;

        for (int i = 0; i < slotImages.Length; i++)
        {
            bool unlocked =
                GameManager.Instance.IsEndingUnlocked((GameManager.EndingType)i);

            if (unlocked)
                slotImages[i].sprite = unlockedSprites[i];
            else
                slotImages[i].sprite = lockedSprites[i];
        }
    }
}