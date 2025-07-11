using UnityEngine;

public class BallController : MonoBehaviour
{

    [System.Obsolete]
    void OnCollisionEnter(Collision collision)
    {
        InteractableItem item = collision.gameObject.GetComponent<InteractableItem>();

        if (item != null)
        {

            PlayerMovement player = FindObjectOfType<PlayerMovement>();
            if (player != null)
            {
                item.ApplyEffect(player);
            }

            Destroy(collision.gameObject);
            Destroy(gameObject);
        }

    }
}