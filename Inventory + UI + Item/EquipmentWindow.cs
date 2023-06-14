using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentWindow : MonoBehaviour
{
    static public EquipmentWindow Instance;

    public static bool EquipmentActivated = false;

    [SerializeField]
    private GameObject EquiptmentWindowPanel;
    [SerializeField]
    private GameObject SlotParent;
    [SerializeField]
    private EquiptSlot[] slots;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }
    void Start()
    {
        slots = SlotParent.GetComponentsInChildren<EquiptSlot>();
    }

    public void TryOpenEquiptment()
    {
        EquipmentActivated = !EquipmentActivated;
        //UiManager.UIActivated = EquipmentActivated;
        if (EquipmentActivated)
        {
            OpenEquiptment();
        }
        else
        {
            CloseEquiptment();
        }
    }
    
    private void OpenEquiptment()
    {
        UiManager.Instance.WindowProcedure(true, GetComponent<Canvas>());
        EquiptmentWindowPanel.SetActive(true);
    }
    private void CloseEquiptment()
    {
        UiManager.Instance.WindowProcedure(false, GetComponent<Canvas>());
        EquiptmentWindowPanel.SetActive(false);
        EquipmentActivated = false;
    }

    public EquiptSlot GetEquiptSlot(Enums.ItemType _itemType)
    {
        if (_itemType == Enums.ItemType.supply_Item || _itemType == Enums.ItemType.Production_Item) return null;
        for(int i = 0; i < slots.Length; i++)
        {
            if(slots[i].OnlyType == _itemType)
            {
                return slots[i];
            }
        }
        return null;
    }

}
