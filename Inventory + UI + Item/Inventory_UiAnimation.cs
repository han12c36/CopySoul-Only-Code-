using UnityEngine;

public class Inventory_UiAnimation : MonoBehaviour
{
    public static Inventory_UiAnimation instance;

    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void InteractionSlotAnimation_Tween(bool value)
    {
        //if (curSlot == null) Debug.Log("현재슬롯이 없는데?");
        //if (value) curSlot.GetComponent<Image>().DOColor(Color.black, 1);
    }

}
