using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Inventory의 분할 Func 및 분할 Window
/// </summary>
public class DivisionProcess : MonoBehaviour
{
    public static bool DivisionActivated = false;

    public InputField DivisionInputField;
    public GameObject Division_Button;
    public GameObject DivisionCancel_Button;

    public void TryOpenDivision()
    {
        if (Inventory.inventoryActivated)
        {
            DivisionActivated = !DivisionActivated;
            if (DivisionActivated) OpenDivision();
            else CloseDivision();
        }
        else return;
    }

    private void OpenDivision()
    {
        gameObject.SetActive(true);
    }
    private void CloseDivision()
    {
        gameObject.SetActive(false);
    }

    //Button
    public void Button_Division()
    {
        if (Inventory.inventoryActivated)
        {
            if (DivisionActivated)
            {
                Division();
                gameObject.SetActive(false);
                DivisionActivated = !DivisionActivated;
                Inventory.Instance.curSlot = null;
            }
        }
    }

    public void Button_DivisionCancel()
    {
        if (Inventory.inventoryActivated)
        {
            if (DivisionActivated)
            {
                DivisionCancel();
                gameObject.SetActive(false);
                DivisionActivated = !DivisionActivated;
                Inventory.Instance.curSlot = null;
            }
        }
    }
    public void Division()
    {
        if (Inventory.Instance.curSlot != null)
        {
            if (DivisionInputField.text != "")
            {
                int divisionCount = int.Parse(DivisionInputField.text);
                if (divisionCount > 0 && divisionCount < Inventory.Instance.curSlot.itemCount)
                {
                    if (DivisionInputField.text != "") DivisionInputField.text = "";

                    if (Inventory.Instance.DivisionItemIn(Inventory.Instance.curSlot.item, divisionCount))
                    {
                        Inventory.Instance.curSlot.SetSlotCount(-divisionCount);
                        if (Inventory.Instance.curSlot.isQuick)
                        {
                            Inventory.Instance.curSlot.curRegisterQuickSlot.SetSlotCount_q(-divisionCount);

                            //if (Inventory.Instance.curSlot.item.itemType == Enums.ItemType.Production_Item)
                            //{
                            //    UiManager.Instance.quickSlot1.SetSlotCount_q(-divisionCount);
                            //}
                            //else if (Inventory.Instance.curSlot.item.itemType == Enums.ItemType.supply_Item)
                            //{
                            //    UiManager.Instance.quickSlot2.SetSlotCount_q(-divisionCount);
                            //}
                        }
                    }
                    else
                    {
                        Debug.Log("인벤토리 칸이 부족합니다!!");
                        return;
                    }
                }
                else
                {
                    if (DivisionInputField.text != "") DivisionInputField.text = "";
                    return;
                }
            }
        }
    }

    public void DivisionCancel()
    {
        if (DivisionInputField.text != "")
        {
            DivisionInputField.text = "";
        }
    }

}
