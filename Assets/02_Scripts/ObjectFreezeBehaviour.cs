using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFreezeBehaviour : MonoBehaviour
{
    public Coroutine runningFreezeCoroutine;
    public bool isCoroutineRunning;


    public IEnumerator Freeze()
    {
        Material mat = transform.GetChild(1).GetComponent<Renderer>().material;
        mat.SetFloat("_Arc2", 0);
        GetComponentInChildren<SpriteRenderer>().color = ObjectManager.Instance.ColorNormal;
        isCoroutineRunning = true;
        GetComponent<ObjectBehaviour>().isFrozen = true;
        var time = 0f;
        while (time < ObjectManager.Instance.freezeCountdown)
        {
            time += Time.deltaTime;
            mat.SetFloat("_Arc2", time * 360 / ObjectManager.Instance.freezeCountdown);
            yield return null;
        }
        StopFreezeCoroutine();
        isCoroutineRunning = false;
    }

    public void StartFreezeCoroutine()
    {
        runningFreezeCoroutine = StartCoroutine(Freeze());
    }


   public void StopFreezeCoroutine()
    {
        if (isCoroutineRunning)
        {
            
            StopCoroutine(runningFreezeCoroutine);
            isCoroutineRunning = false;
            Material mat = transform.GetChild(1).GetComponent<Renderer>().material;
            mat.SetFloat("_Arc2", 360);

        }
        if (ObjectManager.Instance.frozenObjects.Contains(this.gameObject))
        {
            GetComponent<ObjectBehaviour>().isFrozen = false;
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
