using UnityEngine;
using UnityEngine.UI;
//���콺 �̺�Ʈ ��� ���
using UnityEngine.EventSystems;

//RayChast üũ ����(���� �����ִ� �̹����� �����ɽ�Ʈ�� �����ϴϱ�)

//IPointerClickHandler : Ŭ������ڵ鷯(�������̽�)
//IBeginDragHandler : �巡�� ���� �̺�Ʈ
//IDragHandler : �巡�� �� �̺�Ʈ
//IEndDragHandler : �巡�� �� �̺�Ʈ
//IDropHandler : ���콺 �� �̺�Ʈ
public class Slot : MonoBehaviour, IPointerClickHandler , IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    //��ĭ�� �� �� �ִ� �ִ� ������ ����
    public int MaxCount;
    //ȹ���� ������
    public Item item;
    //ȹ���� ������ ����
    public int itemCount;
    //������ �̹���
    public Image item_Image;
    //������ ���� �ؽ�Ʈ
    [SerializeField]
    protected Text text_Count;
    //������ ���� �̹���(�������� �������� ���)
    [SerializeField]
    private GameObject go_CountImage;
    [SerializeField]
    private Text Register_Text;

    public bool isEquiptment;
    public bool isQuick;
    public QuickSlot curRegisterQuickSlot;

    void Start()
    {
    }

    //������ ��������Ʈ ���İ� ����
    public void SetColor(float _alpha)
    {
        Color color = item_Image.color;
        color.a = _alpha;
        item_Image.color = color;
    }

    //������ ȹ��
    public void AddItem(Item _item,int _count = 1)
    {
        item = _item;
        itemCount = _count;
        item_Image.sprite = item.itemImage;

        if(_item.itemType == Enums.ItemType.Defence_Equiptment_Item || _item.itemType == Enums.ItemType.weapon_Equiptment_Item)
        {
            if (go_CountImage != null) go_CountImage.SetActive(false);
        }
        else
        {
            if (go_CountImage != null) go_CountImage.SetActive(true);
            text_Count.text = itemCount.ToString();
        }

        SetColor(1);
    }

    //������ ���� ����
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();
        if (itemCount <= 0)
        {
            if (isQuick) SetRegister(false);
            ClearSlot();
        }
    }

    public void SetRegister(bool value)
    {
        isQuick = value;
        if(!value) Register_Text.text = "";
        else Register_Text.text = "R";
        Register_Text.gameObject.SetActive(value);
    }
    public void SetEquiptment(bool value)
    {
        isEquiptment = value;
        if (!value) Register_Text.text = "";
        else Register_Text.text = "E";
        Register_Text.gameObject.SetActive(value);
    }

    public void ChangeRegisterToEquiptment()
    {
        isQuick = false;
        isEquiptment = true;
        Register_Text.text = "E";
    }

    //���� �ʱ�ȭ
    public void ClearSlot()
    {
        isQuick = false;
        item = null;
        itemCount = 0;
        item_Image.sprite = null;
        SetColor(0);
        text_Count.text = "0";
        go_CountImage.SetActive(false);
    }

    

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SelectionProcess.SelectionActivated)
        {
            if (eventData.pointerCurrentRaycast.gameObject != Inventory.Instance.SelectionParent)
            {
                Inventory.Instance.SelectionParent.CloseSelection();
                Inventory.Instance.curSlot = null;
            }
        }

        //��Ŭ����
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {
                Inventory.Instance.curSlot = this;

                //uianimation
                //Inventory_UiAnimation.instance.InteractionSlotAnimation_Tween(true);
                


                if (item.itemType == Enums.ItemType.weapon_Equiptment_Item
                || item.itemType == Enums.ItemType.Defence_Equiptment_Item)
                {
                    //Equipt(rightMouse)
                    //���� â ����(���, �����)
                    Inventory.Instance.SelectionParent.TryOpenSelection(Inventory.Instance.curSlot, item.itemType, eventData.position);
                }
                else
                {
                    //Division(shift + rightMouse)
                    if(Input.GetKey(KeyCode.LeftShift))
                    {
                        if (itemCount < 2) return;
                        else
                        {
                            Inventory.Instance.DivisionParent.TryOpenDivision();
                        }
                    }
                    else
                    {
                        if (item.itemType == Enums.ItemType.supply_Item || item.itemType == Enums.ItemType.Production_Item)
                        {
                            //use(rightMouse)
                            Inventory.Instance.SelectionParent.TryOpenSelection(Inventory.Instance.curSlot,item.itemType, eventData.position);
                        }
                        else return; 
                    }

                }
            }
        }
    }


    //�巡�� ó�� ���۽� ������ġ�� ���콺 ��ġ�� �ޱ�
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (item != null)
            {
                if (DivisionProcess.DivisionActivated) return;

                DragSlot.instance.dragSlot = this;
                DragSlot.instance.DragSetImage(item);
                DragSlot.instance.transform.position = eventData.position;
            }
        }
    }

    //�巡�� ���϶� ������ ���콺 ��ġ ����ٴϰ� �ϱ�
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (DivisionProcess.DivisionActivated) return;

            if (item != null)
            {
                Inventory.Instance.curSlot = this;

                DragSlot.instance.transform.position = eventData.position;
            }
        }
    }

    //�巡�װ� ������ �� ������ �ٽ� ���� ��ġ�� ���ư����ϱ�
    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (DragSlot.instance.dragSlot != null)
            {
                GameObject obj = eventData.pointerCurrentRaycast.gameObject;
                if (obj != null)
                {
                    if(obj.GetComponentInParent<QuickSlot>() != null && obj.GetComponentInParent<EquiptSlot>() == null)
                    {
                        QuickSlot quickSlot = obj.GetComponentInParent<QuickSlot>();
                        quickSlot.DragRegister(Inventory.Instance.curSlot,DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);
                        DragSlot.instance.SetColor(0);
                        DragSlot.instance.dragSlot = null;
                        Inventory.Instance.curSlot = null;
                    }
                    else if(obj.GetComponentInParent<EquiptSlot>() != null)
                    {
                        //�巡�� ��� ���� ��������

                        EquiptSlot quickSlot = obj.GetComponentInParent<EquiptSlot>();
                        quickSlot.DragEquiptment(Inventory.Instance.curSlot, DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);
                        DragSlot.instance.SetColor(0);
                        DragSlot.instance.dragSlot = null;
                        Inventory.Instance.curSlot = null;
                    }
                    else
                    {
                        DragSlot.instance.SetColor(0);
                        DragSlot.instance.dragSlot = null;
                    }
                }
                else
                {
                    if (Inventory.Instance.curSlot.item.itemType == Enums.ItemType.Armor_Equiptment_Item ||
                        Inventory.Instance.curSlot.item.itemType == Enums.ItemType.Defence_Equiptment_Item ||
                        Inventory.Instance.curSlot.item.itemType == Enums.ItemType.Helmet_Equiptment_Item ||
                        Inventory.Instance.curSlot.item.itemType == Enums.ItemType.weapon_Equiptment_Item)
                    {
                        Inventory.Instance.ThrowingParent.Throw(Inventory.Instance.curSlot,1);
                        Inventory.Instance.curSlot = null;
                        DragSlot.instance.SetColor(0);
                        DragSlot.instance.dragSlot = null;
                    }
                    else
                    {
                        Inventory.Instance.ThrowingParent.TryOpenThrow(eventData.position);
                        DragSlot.instance.SetColor(0);
                    }
                    //DragSlot.instance.dragSlot = null;
                    //ClearSlot();
                }
            }
            //Inventory.Instance.curSlot = null;
        }
    }

    //EndDrag�� OnDrop�� ����
    //EndDrag�� �ƹ������� �巡�� ����� ȣ��
    //OnDrop�� �ش� ������ ������ ����� ȣ��
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (DragSlot.instance.dragSlot != null)
            {
                if (DragSlot.instance.dragSlot == this) return;

                if (item != null)
                {
                    if (DragSlot.instance.dragSlot.isQuick && !isQuick || DragSlot.instance.dragSlot.isEquiptment && !isEquiptment)
                    {
                        ChangeSlot();
                    }
                    else if (!DragSlot.instance.dragSlot.isQuick && isQuick || !DragSlot.instance.dragSlot.isEquiptment && isEquiptment)
                    {
                        if (DragSlot.instance.dragSlot.item.objName == item.objName
                            && (DragSlot.instance.dragSlot.item.itemType == Enums.ItemType.Production_Item
                            || DragSlot.instance.dragSlot.item.itemType == Enums.ItemType.supply_Item))
                        {
                            SumSlot();
                        }
                        else ChangeSlot();
                    }
                    else if (!DragSlot.instance.dragSlot.isQuick && !isQuick || !DragSlot.instance.dragSlot.isEquiptment && !isEquiptment)
                    {
                        if (DragSlot.instance.dragSlot.item.objName == item.objName
                            && (DragSlot.instance.dragSlot.item.itemType == Enums.ItemType.Production_Item
                            || DragSlot.instance.dragSlot.item.itemType == Enums.ItemType.supply_Item))
                        {
                            SumSlot();
                        }
                        else ChangeSlot();
                    }
                    else if (DragSlot.instance.dragSlot.isQuick && isQuick || DragSlot.instance.dragSlot.isEquiptment && isEquiptment)
                    {
                        ChangeSlot();
                    }
                }
                else ChangeSlot();
            }
        }
        DragSlot.instance.dragSlot = null;
    }

    private void ChangeSlot()
    {
        if (DivisionProcess.DivisionActivated) return;

        Item tempItem = item;
        int tempItemCount = itemCount;

        if (DragSlot.instance.dragSlot.isQuick && !DragSlot.instance.dragSlot.isEquiptment)
        {
            if (!isQuick && !isEquiptment)
            {
                SetRegister(true);
                DragSlot.instance.dragSlot.SetRegister(false);
                curRegisterQuickSlot = DragSlot.instance.dragSlot.curRegisterQuickSlot;
                curRegisterQuickSlot.invenSlot = this;
                DragSlot.instance.dragSlot.curRegisterQuickSlot = null;
            }
            else if(!isQuick && isEquiptment)
            {
                Debug.Log("R -> E");
                QuickSlot temp = curRegisterQuickSlot;
                Slot tempIvenSlot = curRegisterQuickSlot.invenSlot;

                SetEquiptment(false);
                SetRegister(true);

                curRegisterQuickSlot = DragSlot.instance.dragSlot.curRegisterQuickSlot;
                curRegisterQuickSlot.invenSlot = DragSlot.instance.dragSlot.curRegisterQuickSlot.invenSlot;

                DragSlot.instance.dragSlot.SetRegister(false);
                DragSlot.instance.dragSlot.SetEquiptment(true);

                DragSlot.instance.dragSlot.curRegisterQuickSlot = temp;
                DragSlot.instance.dragSlot.curRegisterQuickSlot.invenSlot = tempIvenSlot;
            }
            else if (isQuick && !isEquiptment)
            {
                QuickSlot temp = curRegisterQuickSlot;
                Slot tempIvenSlot = curRegisterQuickSlot.invenSlot;
                curRegisterQuickSlot = DragSlot.instance.dragSlot.curRegisterQuickSlot;
                curRegisterQuickSlot.invenSlot = DragSlot.instance.dragSlot.curRegisterQuickSlot.invenSlot;
                DragSlot.instance.dragSlot.curRegisterQuickSlot = temp;
                DragSlot.instance.dragSlot.curRegisterQuickSlot.invenSlot = tempIvenSlot;
            }
        }
        else if (DragSlot.instance.dragSlot.isEquiptment && !DragSlot.instance.dragSlot.isQuick)
        {
            if (!isEquiptment && !isQuick)
            {
                SetEquiptment(true);
                DragSlot.instance.dragSlot.SetEquiptment(false);
                curRegisterQuickSlot = DragSlot.instance.dragSlot.curRegisterQuickSlot;
                curRegisterQuickSlot.invenSlot = this;
                DragSlot.instance.dragSlot.curRegisterQuickSlot = null;
            }
            else if(!isEquiptment && isQuick)
            {
                Debug.Log("E -> R");
                QuickSlot temp = curRegisterQuickSlot;
                Slot tempIvenSlot = curRegisterQuickSlot.invenSlot;
                
                SetRegister(false);
                SetEquiptment(true);
                
                curRegisterQuickSlot = DragSlot.instance.dragSlot.curRegisterQuickSlot;
                curRegisterQuickSlot.invenSlot = DragSlot.instance.dragSlot.curRegisterQuickSlot.invenSlot;
                
                DragSlot.instance.dragSlot.SetEquiptment(false);
                DragSlot.instance.dragSlot.SetRegister(true);
                
                DragSlot.instance.dragSlot.curRegisterQuickSlot = temp;
                DragSlot.instance.dragSlot.curRegisterQuickSlot.invenSlot = tempIvenSlot;
            }
            else if(isEquiptment && !isQuick)
            {
                QuickSlot temp = curRegisterQuickSlot;
                Slot tempIvenSlot = curRegisterQuickSlot.invenSlot;
                curRegisterQuickSlot = DragSlot.instance.dragSlot.curRegisterQuickSlot;
                curRegisterQuickSlot.invenSlot = DragSlot.instance.dragSlot.curRegisterQuickSlot.invenSlot;
                DragSlot.instance.dragSlot.curRegisterQuickSlot = temp;
                DragSlot.instance.dragSlot.curRegisterQuickSlot.invenSlot = tempIvenSlot;
            }
        }
        else if (!DragSlot.instance.dragSlot.isEquiptment && !DragSlot.instance.dragSlot.isQuick)
        {
            if (!isQuick && isEquiptment)
            {
                SetEquiptment(false);
                DragSlot.instance.dragSlot.SetEquiptment(true);
                DragSlot.instance.dragSlot.curRegisterQuickSlot = curRegisterQuickSlot;
                DragSlot.instance.dragSlot.curRegisterQuickSlot.invenSlot = this;
                curRegisterQuickSlot = null;
            }
            else if (isQuick && !isEquiptment)
            {
                SetRegister(false);
                DragSlot.instance.dragSlot.SetRegister(true);
                DragSlot.instance.dragSlot.curRegisterQuickSlot = curRegisterQuickSlot;
                DragSlot.instance.dragSlot.curRegisterQuickSlot.invenSlot = this;
                curRegisterQuickSlot = null;
            }
        }

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);
        if(tempItem != null)
        {
            DragSlot.instance.dragSlot.AddItem(tempItem, tempItemCount);
        }
        else
        {
            DragSlot.instance.dragSlot.ClearSlot();
        }

    }
    private void SumSlot()
    {
        if (DivisionProcess.DivisionActivated) return;

        int sum = itemCount + DragSlot.instance.dragSlot.itemCount;

        if (sum <= MaxCount && sum >= 1)
        {
            SetSlotCount(DragSlot.instance.dragSlot.itemCount);
            if (isQuick)
            {
                if(DragSlot.instance.dragSlot.item.itemType == Enums.ItemType.Production_Item)
                {
                    UiManager.Instance.quickSlot1.SetSlotCount_q(DragSlot.instance.dragSlot.itemCount);
                }
                else if (DragSlot.instance.dragSlot.item.itemType == Enums.ItemType.supply_Item)
                {
                    UiManager.Instance.quickSlot3.SetSlotCount_q(DragSlot.instance.dragSlot.itemCount);
                }
            }
            DragSlot.instance.dragSlot.ClearSlot();
        }
        else if(itemCount == MaxCount) return;
        else
        {
            int rest = itemCount + DragSlot.instance.dragSlot.itemCount - MaxCount;
            SetSlotCount(MaxCount - itemCount);
            if (isQuick)
            {
                if (DragSlot.instance.dragSlot.item.itemType == Enums.ItemType.Production_Item)
                {
                    UiManager.Instance.quickSlot1.SetSlotCount_q(MaxCount - UiManager.Instance.quickSlot1.itemCount);
                }
                else if (DragSlot.instance.dragSlot.item.itemType == Enums.ItemType.supply_Item)
                {
                    UiManager.Instance.quickSlot3.SetSlotCount_q(MaxCount - UiManager.Instance.quickSlot3.itemCount);
                }
            }
            DragSlot.instance.dragSlot.ClearSlot();
            Inventory.Instance.DivisionItemIn(item, rest);
        }
    }
    private void ItemThrow(Item _item, int _count)
    {
        Debug.Log(item.objName + " " + itemCount + " �� ������");
        DragSlot.instance.dragSlot.ClearSlot();
    }

    private void ChangeRegister()
    {
        if (DragSlot.instance.dragSlot.isQuick)
        {
            if (!isQuick)
            {
                SetRegister(true);
                DragSlot.instance.dragSlot.SetRegister(false);
                curRegisterQuickSlot = DragSlot.instance.dragSlot.curRegisterQuickSlot;
                curRegisterQuickSlot.invenSlot = this;
                DragSlot.instance.dragSlot.curRegisterQuickSlot = null;
            }
            else
            {
                QuickSlot temp = curRegisterQuickSlot;
                Slot tempIvenSlot = curRegisterQuickSlot.invenSlot;
                curRegisterQuickSlot = DragSlot.instance.dragSlot.curRegisterQuickSlot;
                curRegisterQuickSlot.invenSlot = DragSlot.instance.dragSlot.curRegisterQuickSlot.invenSlot;
                DragSlot.instance.dragSlot.curRegisterQuickSlot = temp;
                DragSlot.instance.dragSlot.curRegisterQuickSlot.invenSlot = tempIvenSlot;
            }
        }
        else
        {
            if (isQuick)
            {
                SetRegister(false);
                DragSlot.instance.dragSlot.SetRegister(true);
                DragSlot.instance.dragSlot.curRegisterQuickSlot = curRegisterQuickSlot;
                DragSlot.instance.dragSlot.curRegisterQuickSlot.invenSlot = this;
                curRegisterQuickSlot = null;
            }
        }
    }

    private void ChangeEquiptment()
    {
        if (DragSlot.instance.dragSlot.isEquiptment)
        {
            if (!isEquiptment)
            {
                SetEquiptment(true);
                DragSlot.instance.dragSlot.SetEquiptment(false);
                curRegisterQuickSlot = DragSlot.instance.dragSlot.curRegisterQuickSlot;
                curRegisterQuickSlot.invenSlot = this;
                DragSlot.instance.dragSlot.curRegisterQuickSlot = null;
            }
            else
            {
                QuickSlot temp = curRegisterQuickSlot;
                Slot tempIvenSlot = curRegisterQuickSlot.invenSlot;
                curRegisterQuickSlot = DragSlot.instance.dragSlot.curRegisterQuickSlot;
                curRegisterQuickSlot.invenSlot = DragSlot.instance.dragSlot.curRegisterQuickSlot.invenSlot;
                DragSlot.instance.dragSlot.curRegisterQuickSlot = temp;
                DragSlot.instance.dragSlot.curRegisterQuickSlot.invenSlot = tempIvenSlot;
            }
        }
        else
        {
            if (isEquiptment)
            {
                SetEquiptment(false);
                DragSlot.instance.dragSlot.SetEquiptment(true);
                DragSlot.instance.dragSlot.curRegisterQuickSlot = curRegisterQuickSlot;
                DragSlot.instance.dragSlot.curRegisterQuickSlot.invenSlot = this;
                curRegisterQuickSlot = null;
            }
        }
    }
}
    
//���� �������� ���ϰ� ���ƾ��� -> �ƴ��� ��� �� ĭ�� ���Ե�Ͻ� ��� ������ ������

