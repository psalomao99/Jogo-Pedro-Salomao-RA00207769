﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrabber : MonoBehaviour
{
    public GameObject weaponOnHand; 
    public Transform handposition;
    public GameObject[] myobjs;
    private void Start()
    {
        if (CommomStatus.weapononhand > -1)
        {

            weaponOnHand =Instantiate(myobjs[CommomStatus.weapononhand]);
            weaponOnHand.transform.parent = handposition; //coloca como filho da mao
            weaponOnHand.transform.localPosition = Vector3.zero;//vai pra posicao da mao
            weaponOnHand.GetComponent<Rigidbody>().isKinematic = true;//desativa o rigidbody
            weaponOnHand.transform.localRotation = Quaternion.identity;//reseta a rotacao
            weaponOnHand.transform.gameObject.layer = transform.gameObject.layer;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("WeaponDroped"))
        {
            //dropa a arma se tem uma na mao
            if (weaponOnHand)
            {
                weaponOnHand.transform.parent = null;
                weaponOnHand.GetComponent<Rigidbody>().isKinematic = false;
                weaponOnHand.transform.Translate(-transform.up);
                weaponOnHand.layer = 0;
                weaponOnHand.GetComponent<Collider>().enabled = true;
                weaponOnHand.tag = "WeaponDroped";
            }

            weaponOnHand = other.gameObject;
            weaponOnHand.tag = "weaponOnHand";
            other.transform.parent = handposition; //coloca como filho da mao
            other.transform.localPosition = Vector3.zero;//vai pra posicao da mao
            weaponOnHand.GetComponent<Rigidbody>().isKinematic = true;//desativa o rigidbody
            other.transform.localRotation = Quaternion.identity;//reseta a rotacao
            other.transform.gameObject.layer = transform.gameObject.layer;
            weaponOnHand.GetComponent<Collider>().enabled = false;

            for (int i = 0; i < myobjs.Length; i++) {
               if(other.gameObject.name == myobjs[i].name)
                {
                    CommomStatus.weapononhand = i;
                    print("arma"+ i);
                }
            }

           

        }
    }
    
}
