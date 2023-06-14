using System.Collections;
using UnityEngine;

public class Enemy_Ragdoll : MonoBehaviour
{
    public float aliveTime;

    public IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(aliveTime);

        gameObject.SetActive(false);
    }

	public virtual void OnEnable()
	{
        StartCoroutine(DisappearCoroutine());
	}
}
