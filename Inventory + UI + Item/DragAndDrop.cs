using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Dummy Slot을 활용한 Inventory Item DragAndDrop Func
/// </summary>
public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector2 ImagePos;
    Vector2 eventPos;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Inventory.inventoryActivated || EquipmentWindow.EquipmentActivated)
        {
            ImagePos = transform.position;
            eventPos = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Inventory.inventoryActivated || EquipmentWindow.EquipmentActivated)
        {
            Vector2 moveValue = eventData.position - eventPos;
            transform.position = transform.position + new Vector3 (moveValue.x,moveValue.y,0f);
            eventPos = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
