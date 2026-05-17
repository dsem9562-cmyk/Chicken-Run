using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public int currentHP = 200;

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log("현재 체력: " + currentHP);
    }
}