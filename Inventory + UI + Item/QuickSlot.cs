using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum SlotType
{
    QuickSlot,
    EquiptSlot,
    End,
}

public class QuickSlot : MonoBehaviour
{
    public SlotType slotType;
    public Enums.ItemType OnlyType;
    public Item item;
    public Image Item_Image;
    public int itemCount;
    public Text ItemCount_Text;
    public Slot invenSlot;

    public bool isUsable = true;

    public void DragRegister(Slot _invenSlot,Item _item,int _itemCount)
    {
        if (_invenSlot.isEquiptment) return;

        if (_item.itemType == OnlyType)
        {
            AddRegister(_invenSlot,_item, _itemCount, this);
        }
    }
    public void AddRegister(Slot _invenSlot, Item _item, int _itemCount,QuickSlot _quickSlot)
    {
        if (invenSlot != null)
        {
            invenSlot.SetRegister(false);
            invenSlot.curRegisterQuickSlot = null;
        }
        invenSlot = _invenSlot;
        invenSlot.SetRegister(true);
        invenSlot.curRegisterQuickSlot = _quickSlot;
        item = _item;
        Item_Image.sprite = _item.itemImage;
        itemCount = _itemCount;
        if (item.itemType == Enums.ItemType.Defence_Equiptment_Item || item.itemType == Enums.ItemType.weapon_Equiptment_Item)
        {
            ItemCount_Text.text = "";
        }
        else
        {
            ItemCount_Text.text = _itemCount.ToString();
        }
        SetColor_q(1);
    }
    public void DragEquiptment(Slot _invenSlot, Item _item, int _itemCount)
    {
        if (_invenSlot.item == item) return;

        if (_item.itemType == OnlyType)
        {
            AddEquiptment(_invenSlot, _item, _itemCount, (EquiptSlot)this);
        }
    }
    public void AddEquiptment(Slot _invenSlot, Item _item, int _itemCount, EquiptSlot _equiptSlot)
    {
        if (invenSlot != null)
        {
            Debug.Log(invenSlot.name);
            invenSlot.SetEquiptment(false);
            if (invenSlot.isQuick) Inventory.Instance.SelectionParent.Deregisteration(invenSlot);
            //invenSlot.curRegisterQuickSlot = null;
            //invenSlot.SetRegister(true);
            //invenSlot.curRegisterQuickSlot = _equiptSlot.invenSlot.curRegisterQuickSlot;

        }
        invenSlot = _invenSlot;
        invenSlot.SetEquiptment(true);
        invenSlot.curRegisterQuickSlot = _equiptSlot;
        item = _item;
        Debug.Log("�ٲ� ������ : " + item.name);
        Item_Image.sprite = _item.itemImage;
        itemCount = _itemCount;
        if (item.itemType == Enums.ItemType.Defence_Equiptment_Item || item.itemType == Enums.ItemType.weapon_Equiptment_Item)
        {
            ItemCount_Text.text = "";
        }
        else
        {
            ItemCount_Text.text = _itemCount.ToString();
        }
        SetColor_q(1);
        _equiptSlot.matchEquiptmentSlot_Q();
    }


    public void SetColor_q(float _alpha)
    {
        Color color = Item_Image.color;
        color.a = _alpha;
        Item_Image.color = color;
    }
    public void SetSlotCount_q(int _count)
    {
        itemCount += _count;
        if (item.itemType == Enums.ItemType.Defence_Equiptment_Item || item.itemType == Enums.ItemType.weapon_Equiptment_Item)
        {
            ItemCount_Text.text = "";
        }
        else
        {
            ItemCount_Text.text = itemCount.ToString();
        }
        if (itemCount <= 0) ClearSlot_q();
    }

    public void ClearSlot_q()
    {
        invenSlot.isQuick = false;
        invenSlot.isEquiptment = false;
        invenSlot.SetRegister(false);
        invenSlot.SetEquiptment(false);
        invenSlot = null;
        item = null;
        itemCount = 0;
        Item_Image.sprite = null;
        SetColor_q(0);
        ItemCount_Text.text = "";
    }
    public void QuickSlotUse()
    {
        if (item != null)
        {
            Inventory.Instance.SelectionParent.Use(invenSlot);
        }
    }
    public void QuickSlotEquipt(QuickSlot _quickSlot, EquiptSlot _equiptSlot)
    {
        if (isUsable == false) return;
        isUsable = false;

        //| �ϳ��� ���� �ڿ������� Ȯ��
        if (Player.instance.curState_e == Enums.ePlayerState.Idle | Player.instance.curState_e == Enums.ePlayerState.Move)
        {
            if (_equiptSlot.invenSlot != null && invenSlot != null)
            {
                Slot temp = invenSlot;
                Item tempItem = _equiptSlot.item;

                if (_equiptSlot.invenSlot.isEquiptment)
                {
                    if (_equiptSlot.invenSlot.isEquiptment)
                    {
                        //��� ���� ��Ű��
                        _equiptSlot.invenSlot.SetEquiptment(false);
                        _equiptSlot.invenSlot.SetRegister(true);

                        _equiptSlot.item = _quickSlot.item;

                        //�����â�� ����
                        Item_Image.sprite = _equiptSlot.invenSlot.item_Image.sprite;
                        _equiptSlot.Item_Image.sprite = null;
                        _equiptSlot.SetColor_q(0);
                        _equiptSlot.matchEquiptmentSlot_Q();
                    }
                }
                //����ϵ� ����
                if (invenSlot.isQuick)
                {
                    invenSlot.SetRegister(false);
                    invenSlot.SetEquiptment(true);

                    _quickSlot.item = tempItem;

                    _equiptSlot.Item_Image.sprite = invenSlot.item_Image.sprite;
                    _equiptSlot.SetColor_q(1);
                    _equiptSlot.matchEquiptmentSlot_Q();
                    //��ϵ� �κ� ���� ��ü�������
                    invenSlot = _equiptSlot.invenSlot;
                    _equiptSlot.invenSlot = temp;

                    CoolTimeChangeWeapon(_equiptSlot, _quickSlot);
                }

                //���⿣ ���� ����
                if (_equiptSlot.item.GetComponent<Player_Weapon>().type == eWeaponType.Melee)
                {
                    print(_equiptSlot.item.name);
                    _equiptSlot.item.GetComponent<Item_Weapon>().SetAsMainWeapon();
                }
                else if (invenSlot.item.GetComponent<Player_Weapon>().type == eWeaponType.Sheild)
                {
                    _equiptSlot.item.GetComponent<Item_Weapon>().SetAsSubWeapon();
                }
            }
            else if (_equiptSlot.invenSlot == null && invenSlot != null)
            {
                Slot temp = _quickSlot.invenSlot;

                Debug.Log("�ָԿ��� ����� ��ü : " + temp.name);
                //����� ��ü
                if (invenSlot.isQuick)
                {
                    invenSlot.SetRegister(false);
                    invenSlot.SetEquiptment(true);
                    _equiptSlot.item = _quickSlot.item;
                    //��񽽷����� �ٲ������
                    invenSlot.curRegisterQuickSlot = _equiptSlot;
                    _equiptSlot.Item_Image.sprite = invenSlot.item_Image.sprite;
                    _equiptSlot.SetColor_q(1);
                    _equiptSlot.invenSlot = _quickSlot.invenSlot;
                    _equiptSlot.matchEquiptmentSlot_Q();
                    //��ϵ� �κ� ���� ��ü�������
                    //invenSlot = null;
                    // = _quickSlot.invenSlot.curRegisterQuickSlot;
                    _quickSlot.item = null;
                    _quickSlot.Item_Image.sprite = null;
                    _quickSlot.invenSlot = null;
                    _quickSlot.SetColor_q(0);

                    CoolTimeChangeWeapon(_equiptSlot, _quickSlot);

                    //�ָԿ��� �����
                    if (_equiptSlot.item.GetComponent<Player_Weapon>().type == eWeaponType.Melee)
                    {
                        _equiptSlot.item.GetComponent<Item_Weapon>().SetAsMainWeapon();
                    }
                    else if (_equiptSlot.item.GetComponent<Player_Weapon>().type == eWeaponType.Sheild)
                    {
                        _equiptSlot.item.GetComponent<Item_Weapon>().SetAsSubWeapon();
                    }
                }
            }
            else if (_equiptSlot.invenSlot != null && invenSlot == null)
            {
                Debug.Log("���⿡�� �ָ����� ��ü");

                if (_equiptSlot.invenSlot.isEquiptment)
                {
                    //�ָ����� ��ü
                    if (_equiptSlot.invenSlot.isEquiptment)
                    {

                        // �÷��̾� ���� ���� ����
                        if (_equiptSlot.item.GetComponent<Player_Weapon>().type == eWeaponType.Melee)
                        {
                            Player.instance.status.mainWeapon.GetComponent<Item_Weapon>().DeselectMainWeapon();
                        }
                        else if (_equiptSlot.item.GetComponent<Player_Weapon>().type == eWeaponType.Sheild)
                        {
                            Player.instance.status.subWeapon.GetComponent<Item_Weapon>().DeselectSubWeapon();
                        }

                        //��� ���� ��Ű��
                        _equiptSlot.invenSlot.SetEquiptment(false);
                        _equiptSlot.invenSlot.SetRegister(true);
                        _quickSlot.item = _equiptSlot.item;
                        //���������� �ٲ������
                        _equiptSlot.invenSlot.curRegisterQuickSlot = _quickSlot;
                        _quickSlot.invenSlot = _equiptSlot.invenSlot;
                        //�����â�� ����
                        Item_Image.sprite = _equiptSlot.Item_Image.sprite;
                        SetColor_q(1);

                        _equiptSlot.item = null;
                        _equiptSlot.Item_Image.sprite = null;
                        _equiptSlot.SetColor_q(0);
                        _equiptSlot.invenSlot = null;
                        _equiptSlot.matchEquiptmentSlot_Q();

                        CoolTimeChangeWeapon(_equiptSlot, _quickSlot);

                    }
                }
            }
            else return;
        }
    }

    void EnableThis()
    {
        isUsable = true;
    }

    private void CoolTimeChangeWeapon(EquiptSlot _equiptSlot,QuickSlot _quickSlot)
    {
        Color originCol0 = _equiptSlot.equalSlot.Item_Image.color;
        Color originCol1 = _quickSlot.Item_Image.color;

        if (_equiptSlot == null | _quickSlot == null)
        {
            EnableThis();
            return;
        }

            //���Ⱑ ����
        _equiptSlot.equalSlot.Item_Image.color = Color.black;
        _equiptSlot.equalSlot.Item_Image.DOColor(originCol0, 0.5f).SetEase(Ease.OutCirc);
        _quickSlot.Item_Image.color = Color.black;
        _quickSlot.Item_Image.DOColor(originCol1, 0.5f).SetEase(Ease.OutCirc);

        //���Ⱑ �ִϸ��̼�

        _equiptSlot.equalSlot.Item_Image.type = Image.Type.Filled;
        _equiptSlot.equalSlot.Item_Image.fillAmount = 0f;
        _equiptSlot.equalSlot.Item_Image.DOFillAmount(1f, 0.7f).SetEase(Ease.OutCirc);

        _quickSlot.Item_Image.type = Image.Type.Filled;
        _quickSlot.Item_Image.fillAmount = 0f;
        _quickSlot.Item_Image.DOFillAmount(1f, 0.7f).SetEase(Ease.OutCirc)
        .OnComplete(() => { EnableThis(); });
    }
}
