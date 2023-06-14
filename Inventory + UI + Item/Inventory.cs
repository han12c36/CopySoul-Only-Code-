using System.Collections.Generic;
using UnityEngine;

enum itemNameIndex
{
    DefaultWeapon = 5,
    DefaultDefence = 6,
    End,
}

/// <summary>
/// UX를 기반으로 제작한 Inventory
/// 분할, 합침, 버리기, 사용 , 드래그 앤 드롭 기능 구현
/// 위 기능들은 모두 선택된 현재 Slot을 기준으로 동작
/// 인벤 생성시 Pool에 있는 원본 Prefab을 받아옴
/// </summary>

public class Inventory : MonoBehaviour
{
    static public Inventory Instance;
    public static bool inventoryActivated = false;

    private Dictionary<string, Item> Dic_items = new Dictionary<string, Item>();

    public Slot curSlot;

    [SerializeField]
    public GameObject InventoryBase;
    [SerializeField]
    private GameObject SlotParent;
    public SelectionProcess SelectionParent;
    public ThrowingProcess ThrowingParent;
    public DivisionProcess DivisionParent;

    [SerializeField]
    private Slot[] slots;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }
    void Start()
    {
        slots = SlotParent.GetComponentsInChildren<Slot>();
        SetAll_Items();
        GetItems();
    }
    public void TryOpenInventory()
    {
        inventoryActivated = !inventoryActivated;

        if (inventoryActivated)
        {
            OpenInventory();
            UiManager.UIActivated = true;
        }
        else
        {
            CloseInventory();
        }
    }
    private void OpenInventory()
    {
        if (!DivisionProcess.DivisionActivated && !ThrowingProcess.ThrowingActivated)
        {
            InventoryBase.SetActive(true);
            UiManager.Instance.WindowProcedure(true, GetComponent<Canvas>());
        }
    }
    private void CloseInventory()
    {
        if (!DivisionProcess.DivisionActivated && !ThrowingProcess.ThrowingActivated)
        {
            if (SelectionProcess.SelectionActivated) SelectionParent.CloseSelection();
            InventoryBase.SetActive(false);
            inventoryActivated = false;
            UiManager.Instance.WindowProcedure(false, GetComponent<Canvas>());
        }
    }

    public bool ItemIn(Item _item, int _count = 1)
    {
        if (_item.itemType == Enums.ItemType.Production_Item || _item.itemType == Enums.ItemType.supply_Item)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null && slots[i].itemCount < slots[i].MaxCount)
                {
                    if (slots[i].item.objName == _item.objName)
                    {
                        if (slots[i].isQuick) slots[i].curRegisterQuickSlot.SetSlotCount_q(_count);
                        slots[i].SetSlotCount(_count);
                        return true;
                    }
                }
            }
        }
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return true;
            }
        }
        return false;
    }

    public bool DivisionItemIn(Item _item, int _count)
    {
        if (_item.itemType == Enums.ItemType.Production_Item || _item.itemType == Enums.ItemType.supply_Item)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null)
                {
                    slots[i].AddItem(_item, _count);
                    return true;
                }
            }
        }
        return false;
    }

    public void Button_InventoryExit()
    {
        if (!DivisionProcess.DivisionActivated)
        {
            inventoryActivated = false;
            InventoryBase.SetActive(false);
        }
    }

    public void GetItem(Item _item)
    {
        ItemIn(_item);
    }
    public void GetItem(string _itemName,Vector3 pos, int count = 1)
    {
        Debug.Log(_itemName);
        GameObject itemObj = ObjectPoolingCenter.Instance.LentalObj(_itemName, count);

        if (itemObj.tag == "Item")
        {
            Item item = itemObj.GetComponent<Item>();
            if(item.itemType == Enums.ItemType.supply_Item) item.Count = count;
        }
        itemObj.transform.position = pos;
    }
    public void GetItem(string _itemName,int count = 1)
    {
        ItemIn(Dic_items[_itemName], count);
    }

    public void GetItems()
    {
        GetItem("Shield0_Item"); // 60

        GetItem("Shield1_Item"); // 40
        GetItem("Shield2_Item"); // 20
        GetItem("Shield3_Item"); // 10

        GetItem("Sword0_Item");  // 60
        GetItem("Sword1_Item");  // 40
        GetItem("Sword2_Item");  // 20
        GetItem("Sword3_Item");  // 10

        GetItem("Potion_Item", 10);  //70
    }
    public void Routing(Vector3 pos)
    {
        Vector3 Pos1 = new Vector3(pos.x,pos.y + 1f,pos.z);
        int index = Random.Range(0, 11);
        switch (index)
        {
            case 0: Draw("Shield0_Item", 100, Pos1);  //Lv 0     25
                break;
            case 1: Draw("Sword0_Item", 100, Pos1);   //Lv 0     25      
                break;
            case 2: Draw("Shield1_Item", 100, Pos1);  //Lv 1     20  
                break;
            case 3: Draw("Sword1_Item", 100, Pos1);   //Lv 1     20
                break;
            case 4: Draw("Shield2_Item", 100, Pos1);  //Lv 2     15
                break;
            case 5: Draw("Sword2_Item", 100, Pos1);   //Lv 2     15
                break;
            case 6: Draw("Shield3_Item", 100, Pos1);  //Lv 3      5
                break;
            case 7: Draw("Sword3_Item", 100, Pos1);   //Lv 3      5
                break;
            case 8:
            case 9:
            case 10:
                Draw("Potion_Item", 100, pos);       //potion        85
                break;
            default:
                break;
        }
    }

    private bool Draw<T>(T itemname,float probability,Vector3 pos)
    {
        if(RandDraw(probability))
        {
            GameObject item = ObjectPoolingCenter.Instance.LentalObj(itemname.ToString());
            item.transform.position = pos;
            return true;
        }
        return false;
    }
    private bool RandDraw(float probability)
    {
        bool Success = false;
        float Value;
        float total = Dic_items.Count;
        if (probability == 0) Value = -1;
        else Value = (probability * total) / 100f;
        int Rand = Random.Range(0, (int)total);
        if(Rand <= Value) Success = true;
        return Success;
    }

    private void SetAll_Items()
    {
        foreach (GameObject item in ObjectPoolingCenter.Instance.prefabs)
        {
            if (item.GetComponent<Item>() != null)
            {
                Dic_items.Add(item.name, item.GetComponent<Item>());
            }
        }
    }

    

}
