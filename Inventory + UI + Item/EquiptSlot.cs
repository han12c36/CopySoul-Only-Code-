public class EquiptSlot : QuickSlot
{
    public EquiptSlot_Q equalSlot;

    private void Awake()
    {
        slotType = SlotType.EquiptSlot;
        if (OnlyType == Enums.ItemType.weapon_Equiptment_Item)
        {
            if (equalSlot == null && UiManager.Instance.EquiptSlotQ_Weapon)
            {
                equalSlot = UiManager.Instance.EquiptSlotQ_Weapon;
            }
        }
        else if (OnlyType == Enums.ItemType.Defence_Equiptment_Item)
        {
            if (equalSlot == null && UiManager.Instance.EquiptSlotQ_Defence)
            {
                equalSlot = UiManager.Instance.EquiptSlotQ_Defence;
            }
        }
    }
    void Start()
    {
       
    }

    //ó���� ��� �ø��� �ΰ� ����
    public void matchEquiptmentSlot_Q()
    {
        equalSlot.Item_Image.sprite = Item_Image.sprite;
        equalSlot.Item_Image.color = Item_Image.color;
        equalSlot.item = item;
        equalSlot.invenSlot = invenSlot;
    }
    public void EquiptmentInitialize()
    {

    }
}
