using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    public enum ItemType
    {
        Point1,
        Point2,
        Point5,
        HealthUp,
        HealthDown
    }

    public ItemType type;

    public void ApplyEffect(PlayerMovement player)
    {
        if (player == null) return;

        switch (type)
        {
            case ItemType.Point1:
                player.AddScore(1);
                break;
            case ItemType.Point2:
                player.AddScore(2);
                break;
            case ItemType.Point5:
                player.AddScore(5);
                break;
            case ItemType.HealthUp:
                player.ChangeHealth(1);
                break;
            case ItemType.HealthDown:
                player.ChangeHealth(-1);
                break;
        }
    }
}