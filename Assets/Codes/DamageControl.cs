using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DamageControl : MonoBehaviour
{
    bool hit = false;
    public int lifes = 3;
    public IAWalk iawalk;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetDamage()
    {
        iawalk.currentState = IAWalk.IaState.Dying;
       

    }

    void Damage()
    {
        if(hit==false)
        {
            lifes--;
            hit = true;
            StartCoroutine(HitTimer());

            if (lifes<=0)
            {
                iawalk.currentState = IAWalk.IaState.Dying;
            }
            else
            {
                iawalk.currentState = IAWalk.IaState.Damage;
            }
            
            
        }
       
    }
    IEnumerator HitTimer()
    {
        yield return new WaitForSeconds(0.5f);
        hit = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Projectiles"))
        {
            lifes--;
            iawalk.currentState = IAWalk.IaState.Damage;
        }
        if (lifes < 0)
        {
            iawalk.currentState = IAWalk.IaState.Dying;
          
        }
    }

    
}
