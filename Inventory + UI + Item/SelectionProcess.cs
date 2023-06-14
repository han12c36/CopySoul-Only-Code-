using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionProcess : MonoBehaviour
{
    public static bool SelectionActivated = false;

    public GameObject Equipt_Button;
    public GameObject UnEquipt_Button;
    public GameObject Use_Button;
    public GameObject Register_Button;
    public GameObject Deregistration_Button;
    public GameObject Throw_Button;

    public void Selection_AllOff()
    {
        SelectionActivated = !SelectionActivated;
        gameObject.SetActive(false);
        Equipt_Button.SetActive(false);
        UnEquipt_Button.SetActive(false);
        Use_Button.SetActive(false);
        Register_Button.SetActive(false);
        Deregistration_Button.SetActive(false);
        Throw_Button.SetActive(false);
    }

    public void TryOpenSelection(Slot curSlot,Enums.ItemType _itemType, Vector3 Vec)
    {
        if (Inventory.inventoryActivated && !DivisionProcess.DivisionActivated && !SelectionActivated)
        {
            OpenSelection(curSlot,_itemType, Vec);
        }
    }

    //이미 장비중이라면 아니라면 

    public void OpenSelection(Slot _curSlot,Enums.ItemType _itemType, Vector3 vec)
    {

        SelectionActivated = !SelectionActivated;
        gameObject.SetActive(true);
        Vector3 vec1 = new Vector3(vec.x + 60f, vec.y - 15f);
        gameObject.transform.position = vec1;
        if (_itemType == Enums.ItemType.supply_Item || _itemType == Enums.ItemType.Production_Item)
        {
            Use_Button.SetActive(true);
        }
        else if ((_itemType == Enums.ItemType.weapon_Equiptment_Item || _itemType == Enums.ItemType.Defence_Equiptment_Item) 
            && !_curSlot.isEquiptment)
        {
            Equipt_Button.SetActive(true);
        }

        if (_curSlot.isQuick) Deregistration_Button.SetActive(true);
        else Register_Button.SetActive(true);

        if (_curSlot.isEquiptment)
        {
            UnEquipt_Button.SetActive(true);
        }
        else
        {
            UnEquipt_Button.SetActive(false);
        }

        Throw_Button.SetActive(true);
    }

    public void CloseSelection()
    {
        gameObject.SetActive(false);
        Selection_AllOff();
    }

    public void Button_Equipt()
    {
        if (Inventory.Instance.curSlot.isQuick) Deregisteration(Inventory.Instance.curSlot);
        Equipt(Inventory.Instance.curSlot);
        Selection_AllOff();
        Inventory.Instance.curSlot = null;
    }
    public void Button_UnEquipt()
    {
        UnEquipt(Inventory.Instance.curSlot);
        Selection_AllOff();
        Inventory.Instance.curSlot = null;
    }

    public void Button_Use()
    {
        Use(Inventory.Instance.curSlot);
        Selection_AllOff();
        Inventory.Instance.curSlot = null;
    }

    public void Button_Register()
    {
        if(Inventory.Instance.curSlot.isEquiptment) UnEquipt(Inventory.Instance.curSlot);
        ButtonRegister(Inventory.Instance.curSlot);
        Selection_AllOff();
        Inventory.Instance.curSlot = null;
    }
    public void Button_Throw()
    {
        if(Inventory.Instance.curSlot.item.itemType == Enums.ItemType.weapon_Equiptment_Item 
            || Inventory.Instance.curSlot.item.itemType == Enums.ItemType.Defence_Equiptment_Item
            || Inventory.Instance.curSlot.item.itemType == Enums.ItemType.Helmet_Equiptment_Item
            || Inventory.Instance.curSlot.item.itemType == Enums.ItemType.Armor_Equiptment_Item)
        {
            Inventory.Instance.ThrowingParent.Throw(Inventory.Instance.curSlot, 1);
            Inventory.Instance.curSlot = null;
        }
        else
        {
            if(Inventory.Instance.curSlot.itemCount <= 1)
            {
                Inventory.Instance.ThrowingParent.Throw(Inventory.Instance.curSlot, 1);
                Inventory.Instance.curSlot = null;
            }
            else Inventory.Instance.ThrowingParent.TryOpenThrow();
        }
        Selection_AllOff();
    }

    public void Button_Deregistration()
    {
        Deregisteration(Inventory.Instance.curSlot);
        Selection_AllOff();
        Inventory.Instance.curSlot = null;
    }

    public void ButtonRegister(Slot _slot)
    {
        //if (_slot.isEquiptment) return;

        if (_slot != null)
        {
            _slot.SetRegister(true);
            if (_slot.item.itemType == Enums.ItemType.Production_Item)
            {
                if (UiManager.Instance.quickSlot1.invenSlot != null)
                {
                    if (UiManager.Instance.quickSlot1.invenSlot != _slot)
                    {
                        UiManager.Instance.quickSlot1.invenSlot.SetRegister(false);
                    }
                }
                UiManager.Instance.quickSlot1.AddRegister(_slot, _slot.item, _slot.itemCount, UiManager.Instance.quickSlot1);
            }
            else if (_slot.item.itemType == Enums.ItemType.weapon_Equiptment_Item)
            {
                if (UiManager.Instance.quickSlot2.invenSlot != null)
                {
                    if (UiManager.Instance.quickSlot2.invenSlot != _slot)
                    {
                        UiManager.Instance.quickSlot2.invenSlot.SetRegister(false);
                    }
                }
                UiManager.Instance.quickSlot2.AddRegister(_slot, _slot.item, _slot.itemCount, UiManager.Instance.quickSlot2);
            }
            else if (_slot.item.itemType == Enums.ItemType.supply_Item)
            {
                if (UiManager.Instance.quickSlot3.invenSlot != null)
                {
                    if (UiManager.Instance.quickSlot3.invenSlot != _slot)
                    {
                        UiManager.Instance.quickSlot3.invenSlot.SetRegister(false);
                    }
                }
                UiManager.Instance.quickSlot3.AddRegister(_slot, _slot.item, _slot.itemCount, UiManager.Instance.quickSlot3);
            }
            else if (_slot.item.itemType == Enums.ItemType.Defence_Equiptment_Item)
            {
                if (UiManager.Instance.quickSlot4.invenSlot != null)
                {
                    if (UiManager.Instance.quickSlot4.invenSlot != _slot)
                    {
                        UiManager.Instance.quickSlot4.invenSlot.SetRegister(false);
                    }
                }
                UiManager.Instance.quickSlot4.AddRegister(_slot, _slot.item, _slot.itemCount, UiManager.Instance.quickSlot4);
            }
        }
    }

    public void Equipt(Slot _curSlot)
    {
        EquiptSlot EquiptSlot =  EquipmentWindow.Instance.GetEquiptSlot(_curSlot.item.itemType);
        if (EquiptSlot)
        {
            EquiptSlot.AddEquiptment(_curSlot, _curSlot.item, 1, EquiptSlot);
        }
        else Debug.Log("EquiptSlot == null");

        if (_curSlot.item.GetComponent<Player_Weapon>().type == eWeaponType.Melee)
        {
            print(_curSlot.item.name + " " + _curSlot.item.gameObject);
            _curSlot.item.GetComponent<Item_Weapon>().SetAsMainWeapon();
        }
        else if (_curSlot.item.GetComponent<Player_Weapon>().type == eWeaponType.Sheild)
        {
            _curSlot.item.GetComponent<Item_Weapon>().SetAsSubWeapon();
        }
    }

    public void Use(Slot _curSlot)
    {
        if (Player.instance.curState_e == Enums.ePlayerState.Idle || Player.instance.curState_e == Enums.ePlayerState.Move)
        {
            if (_curSlot.item.itemType == Enums.ItemType.Production_Item)
            {
                Debug.Log("상호 작용!");
                _curSlot.item.PlayFuncs();
            }
            else if (_curSlot.item.itemType == Enums.ItemType.supply_Item)
            {
                Debug.Log("아이템 사용!");
                _curSlot.item.PlayFuncs();
            }
            if (_curSlot.isQuick) _curSlot.curRegisterQuickSlot.SetSlotCount_q(-1);
            _curSlot.SetSlotCount(-1);
        }
    }

    public void Deregisteration(Slot _curSlot)
    {
        _curSlot.SetRegister(false);
        _curSlot.curRegisterQuickSlot.ClearSlot_q();
        _curSlot.curRegisterQuickSlot = null;
    }

    public void UnEquipt(Slot _curSlot)
    {
        EquiptSlot _equiptSlot = _curSlot.curRegisterQuickSlot.GetComponent<EquiptSlot>();

        if (_equiptSlot == null)
        {
            _curSlot.SetEquiptment(false);

            if (_curSlot.item.GetComponent<Player_Weapon>().type == eWeaponType.Melee)
            {
                print(_curSlot.item.name + " " + _curSlot.item.gameObject);
                _equiptSlot = UiManager.Instance.EquiptSlotQ_Weapon;

                _equiptSlot.ClearSlot_q();
                _equiptSlot.matchEquiptmentSlot_Q();
                _curSlot.item.GetComponent<Item_Weapon>().DeselectMainWeapon();
            }
            else if (_curSlot.item.GetComponent<Player_Weapon>().type == eWeaponType.Sheild)
            {
                _equiptSlot = UiManager.Instance.EquiptSlotQ_Defence;

                _equiptSlot.ClearSlot_q();
                _equiptSlot.matchEquiptmentSlot_Q();
                _curSlot.item.GetComponent<Item_Weapon>().DeselectSubWeapon();
            }
        }
        else
        {
            _curSlot.SetEquiptment(false);
            _equiptSlot.ClearSlot_q();
            _equiptSlot.matchEquiptmentSlot_Q();
            if (_curSlot.item.GetComponent<Player_Weapon>().type == eWeaponType.Melee)
            {
                print(_curSlot.item.name + " " + _curSlot.item.gameObject);
                _curSlot.item.GetComponent<Item_Weapon>().DeselectMainWeapon();
            }
            else if (_curSlot.item.GetComponent<Player_Weapon>().type == eWeaponType.Sheild)
            {
                _curSlot.item.GetComponent<Item_Weapon>().DeselectSubWeapon();
            }
        }
    }
}
