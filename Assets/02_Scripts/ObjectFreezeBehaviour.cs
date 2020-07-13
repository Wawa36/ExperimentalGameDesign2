using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFreezeBehaviour : MonoBehaviour
{
    public Coroutine runningFreezeCoroutine;
    public bool isCoroutineRunning;


    public IEnumerator Freeze()
    {
        GetComponentInChildren<SpriteRenderer>().color = ObjectManager.Instance.ColorNormal;
        isCoroutineRunning = true;
        GetComponentInChildren<SpriteRenderer>().color = ObjectManager.Instance.ColorFrozen;
        GetComponent<ObjectBehaviour>().enabled = false;
        GetComponentInChildren<SpriteRenderer>().sortingLayerName = "IsTotallyVisible";
        var time = 0f;
        while (time < ObjectManager.Instance.freezeCountdown)
        {
            time += Time.deltaTime;
            GetComponentInChildren<SpriteRenderer>().color
                = Color.Lerp(ObjectManager.Instance.ColorFrozen, ObjectManager.Instance.ColorNormal, time / ObjectManager.Instance.freezeCountdown);
            yield return null;
        }
        StopFreezeCoroutine();
        isCoroutineRunning = false;
    }

   public void StopFreezeCoroutine()
    {
        GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Objects";
        if (isCoroutineRunning)
        {
            StopCoroutine(runningFreezeCoroutine);
            isCoroutineRunning = false;
        }
        if (ObjectManager.Instance.frozenObjects.Contains(this.gameObject))
        {
            this.gameObject.GetComponent<ObjectBehaviour>().enabled = true;
            ObjectManager.Instance.frozenObjects.Remove(this.gameObject);
        }
        
        GetComponentInChildren<SpriteRenderer>().color = ObjectManager.Instance.ColorNormal;
        FindObjectOfType<PlayerFreezing>().RetrieveFreeze();
    }

    void OnDestroy()
    {
        if (isCoroutineRunning)
        {
            StopFreezeCoroutine();
            ObjectManager.Instance.frozenObjects.Remove(this.gameObject);
        }
        //StopFreezeCoroutineRegular();
    }
}
