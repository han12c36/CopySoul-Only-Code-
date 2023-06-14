using UnityEngine;

public class Spirit_RemainderWeapon : MonoBehaviour
{
    public float existTime;
    float time;

    void Update()
    {
        time += Time.deltaTime;
        if (time >= existTime) gameObject.SetActive(false);
    }
}
