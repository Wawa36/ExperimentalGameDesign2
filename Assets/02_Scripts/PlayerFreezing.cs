using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreezing : MonoBehaviour
{
    float collisionTimer = 2f;
    private bool hasCollided = false;
    [SerializeField] List<GameObject> freezeShots;


    private void Start()
    {
        ActivateNewFreezeShot();
    }

    public void ActivateNewFreezeShot()
    {
        for (int i = 0; i < freezeShots.Count; i++)
        {
            freezeShots[i].GetComponent<SpriteRenderer>().color = ObjectManager.Instance.ColorFrozen;
            if(i <= ObjectManager.Instance.maxFreezeNumber - 1)
            {
                freezeShots[i].SetActive(true);
            }
            else
            {
                freezeShots[i].SetActive(false);
            }
        }
    }
     
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Object" && collisionTimer >= .4f )
        {
            ExpendFreeze();
            if (this.hasCollided == true) {  return; }
            this.hasCollided = true;
            ObjectManager.Instance.FreezeObject(collision.gameObject);
            StartCoroutine(CollisionCooldown());
        }
    }

    public void ExpendFreeze()
    {
        foreach (GameObject freezeShot in freezeShots)
        {
            if (freezeShot.GetComponent<SpriteRenderer>().color == ObjectManager.Instance.ColorNormal)
            {
                continue;
            }
            else
            {
                freezeShot.GetComponent<SpriteRenderer>().color = ObjectManager.Instance.ColorNormal;
                break;
            }
        }
    }

    public void RetrieveFreeze()
    {
        foreach (GameObject freezeShot in freezeShots)
        {
            if (freezeShot.GetComponent<SpriteRenderer>().color == ObjectManager.Instance.ColorFrozen)
            {
                continue;
            }
            else
            {
                freezeShot.GetComponent<SpriteRenderer>().color = ObjectManager.Instance.ColorFrozen;
                break;
            }
        }
    }

    IEnumerator CollisionCooldown()
    {
        collisionTimer = 0f;
        while (collisionTimer < 1)
        {
            collisionTimer += Time.deltaTime;
            yield return null;
        }
    }

    void LateUpdate()
    {
        this.hasCollided = false;
    }
}
