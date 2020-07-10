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
        FindObjectOfType<PlayerFreezing>().RetrieveFreeze();
        StopFreezeCoroutineRegular();
        isCoroutineRunning = false;
    }

   public void StopFreezeCoroutineRegular()
    {
        if (ObjectManager.Instance.freezeCoroutines.Contains(runningFreezeCoroutine))
        {
            ObjectManager.Instance.freezeCoroutines.Remove(runningFreezeCoroutine);
        }
        if(ObjectManager.Instance.frozenObjects.Count > 0)
        {
            ObjectManager.Instance.frozenObjects.Peek().GetComponent<ObjectBehaviour>().enabled = true;
            ObjectManager.Instance.frozenObjects.Remove(this.gameObject);
        }
        GetComponentInChildren<SpriteRenderer>().color = ObjectManager.Instance.ColorNormal;
    }

    void OnDestroy()
    {
        if (isCoroutineRunning)
        {
            StopCoroutine(runningFreezeCoroutine);
            ObjectManager.Instance.freezeCoroutines.Remove(runningFreezeCoroutine);
            ObjectManager.Instance.frozenObjects.Remove(this.gameObject);
            

        }
        //StopFreezeCoroutineRegular();
    }
}
