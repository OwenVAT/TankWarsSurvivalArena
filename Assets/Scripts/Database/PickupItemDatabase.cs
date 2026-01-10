using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItemDatabase : MonoBehaviour
{
    public static PickupItemDatabase Instance;
    [SerializeField] private List<PickupConfig> listPickupItem;
    public Dictionary<PickupType, PickupConfig> dictPickupItem;
    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        dictPickupItem = new Dictionary<PickupType, PickupConfig>();
        foreach (var item in Resources.LoadAll<PickupConfig>("/ScriptableObjects/PickupItem"))
        {
            if (!dictPickupItem.ContainsKey(item.pickupType))
            {
                dictPickupItem[item.pickupType] = item;
            }
        }
    }
    public PickupConfig GetPickupConfig(PickupType type)
    {
        if (dictPickupItem.TryGetValue(type, out PickupConfig cfg))
        {
            return cfg;
        }
        return null;
    }

}
