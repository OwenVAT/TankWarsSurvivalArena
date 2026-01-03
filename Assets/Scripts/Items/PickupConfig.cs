using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="PickupConfig",menuName ="Pickup/PickupConfig")]
public class PickupConfig : ScriptableObject
{
    public PickupType pickupType;
    public string pickupName;
    public int amount;
    public GameObject pickupPrefab;
}
