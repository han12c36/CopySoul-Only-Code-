using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DragSlot : MonoBehaviour
{
    static public DragSlot instance;

    public Slot dragSlot;
    [SerializeField]
    private Image iamgeItem;

    void Start()
    {
        instance = this;
    }

    public void DragSetImage(Item item)
    {
        if (dragSlot.item != null)
        {
            iamgeItem.sprite = item.itemImage;
            SetColor(1);
        }
    }

    public void SetColor(float _alpha)
    {
        Color color = iamgeItem.color;
        color.a = _alpha;
        iamgeItem.color = color;
    }

}
