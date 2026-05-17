using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public int damage = 60;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHP hp = other.GetComponent<PlayerHP>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

            Destroy(gameObject); // 맞으면 사라짐
        }

        // 바닥에 닿아도 사라지게 하고 싶으면
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
