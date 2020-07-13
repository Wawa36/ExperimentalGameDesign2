using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFreezeBehaviour : MonoBehaviour
{
    public Coroutine runningFreezeCoroutine;
    public bool isCoroutineRunning;


    public IEnumerator Freeze(GameObject obj)
    {
        isCoroutineRunning = true;
        obj.GetComponentInChildren<SpriteRenderer>().color = ObjectManager.Instance.ColorFrozen;
        obj.GetComponent<ObjectBehaviour>().enabled = false;
        var time = 0f;
        while (time < ObjectManager.Instance.freezeCountdown)
        {
            time += Time.deltaTime;
            obj.GetComponentInChildren<SpriteRenderer>().color
                = Color.Lerp(ObjectManager.Instance.ColorFrozen, ObjectManager.Instance.ColorNormal, time / ObjectManager.Instance.freezeCountdown);
            yield return null;
        }
        StopFreezeCoroutine();
        isCoroutineRunning = false;
    }

   public void StopFreezeCoroutine()
    {
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
