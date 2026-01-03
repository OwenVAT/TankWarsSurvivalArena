using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public PickupType pickupType;
    public float amount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TankController tank = other.GetComponentInParent<TankController>();
        if (tank == null) 
            return;
        switch (pickupType)
        {
            case PickupType.Heal:
                tank.Heal(amount); 
                break;
            case PickupType.Armor:
                tank.AddArmor(amount);
                break;
            //case PickupType.Coin:
            //    tank.AddCoin(amount);
        }
        Destroy(gameObject);
    }
}
