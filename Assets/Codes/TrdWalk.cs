﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TrdWalk : MonoBehaviour
{
    public enum States
    {
        idle,
        walk,
        jump,
        die,
        attack,
        dano,
    }
    public States state;
    public Animator anim;
    public Rigidbody rdb;
    public float jumpforce=1000;
    float jumptime = .5f;
    public Vector3 move { get; private set; }
    public float movforce=100;
    private bool morreu = false;

    public bool ikActive = false;
    public GameObject painelDerrota;
    public GameObject skill;

    Vector3 direction;

    GameObject referenceObject;

    bool JumpPressed;

    public GameObject objectToLook;
    Vector3 lookposition;
    // Start is called before the first frame update
    Collider espadaColl;
    bool hit = false;
    public int lifes = 5;
    void Start()
    {
        StartCoroutine(Idle());

        referenceObject=Camera.main.GetComponent<trdCam>().GetRefereceObject();
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            if (CommomStatus.lastPosition.magnitude > 1)
            {
                transform.position = CommomStatus.lastPosition;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //criacao de vetor de movimento local
        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = referenceObject.transform.TransformDirection(move);

        //girar pra direcao das teclas
        if (move.magnitude > 0)
        {
            direction = move;
        }
        transform.forward =Vector3.Lerp(transform.forward,direction,Time.fixedDeltaTime*20);

       
        //reduz a força de movimento de acordo com a velocidade pra ter muita força de saida mas pouca velocidade. 
        rdb.AddForce(move * (movforce/(rdb.velocity.magnitude+1)));

        Vector3 velocityWoY = new Vector3(rdb.velocity.x, 0, rdb.velocity.z);
        rdb.AddForce(-velocityWoY * 500);


        if(Physics.Raycast(transform.position+ Vector3.up*.5f, Vector3.down,out RaycastHit hit, 65279))
        {
            anim.SetFloat("GroundDistance", hit.distance);
        }

    }
    private void Update()
    {
        if (!morreu)
        {


            if (Input.GetButtonDown("Fire1"))
            {
                StartCoroutine(Attack());
            }

            if (Input.GetButtonDown("Jump"))
            {
                StartCoroutine(Jump());
            }
            if (Input.GetButtonUp("Jump"))
            {
                jumptime = 0;
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(Skill());
            }
        }
        if (morreu)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("R");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
                
        }
    }
    IEnumerator Skill()
    {
        anim.SetTrigger("Skill");

        yield return new WaitForSeconds(0.5f);
        Vector3 tempPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        GameObject myprojectile = Instantiate(skill, tempPos + transform.forward, Quaternion.identity);
        myprojectile.GetComponent<Rigidbody>().AddForce(transform.forward * 5, ForceMode.Impulse);
    }
    IEnumerator Idle()
    {
        //equivalente ao Start 
        state = States.idle;

        
        //
        while (state == States.idle)
        {
            //equivalente ao update

            anim.SetFloat("Velocity", 0);
            if (rdb.velocity.magnitude > 0.1f)
            {
                StartCoroutine(Walk());
            }
            //
            yield return new WaitForEndOfFrame();
        }
        //saida do estado
    }

    IEnumerator Walk()
    {
        //equivalente ao Start 
        state = States.walk;


        //
        while (state == States.walk)
        {
            //equivalente ao update

            anim.SetFloat("Velocity", rdb.velocity.magnitude);
            if (rdb.velocity.magnitude < 0.1f)
            {
                StartCoroutine(Idle());
            }
            //
            yield return new WaitForEndOfFrame();
        }
        //saida do estado
    }


    IEnumerator Attack()
    {
        espadaColl = GameObject.FindGameObjectWithTag("weaponOnHand").GetComponent<Collider>();
        espadaColl.enabled = true;
        //equivalente ao Start 
        state = States.attack;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(.5f);
        //saida do estado
        espadaColl.enabled = false;
        StartCoroutine(Idle());
    }

    IEnumerator Die()
    {
        //equivalente ao Start 
        state = States.die;
        anim.SetBool("Die", true);
        GetComponent<Rigidbody>().isKinematic = true;
        
        morreu = true;
        painelDerrota.SetActive(true);
        yield return new WaitForEndOfFrame();
        

    }
    IEnumerator Dano()
    {
        //equivalente ao Start 
        state = States.dano;
        anim.SetTrigger("Dano");

        yield return new WaitForEndOfFrame();
        StartCoroutine(Idle());
    }
    IEnumerator Jump()
    {
        //equivalente ao Start 
        state = States.jump;
        jumptime = 0.5f;
        //checa se esta no chao
        if (Physics.Raycast(transform.position + Vector3.up * .5f, Vector3.down, out RaycastHit hit, 65279))
        {
            if(hit.distance > 0.6f)
            {
                StartCoroutine(Idle());
            }
        }
        
        while (state == States.jump)
        {
            //equivalente ao update
            //adiciona forca enquanto o tempo diminue
            rdb.AddForce(Vector3.up * jumpforce* jumptime);
            jumptime -= Time.fixedDeltaTime;
            //se o tempo acabar o estado acaba
            if (jumptime < 0)
            {
                StartCoroutine(Idle());
            }    
            yield return new WaitForFixedUpdate();
        }
        //saida do estado
    }



    private void OnAnimatorIK(int layerIndex)
    {
        if (ikActive)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            
            if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hit, 3, 65279))
            {
                if (hit.collider.CompareTag("Push"))
                {
                    anim.SetIKPosition(AvatarIKGoal.LeftHand, hit.point-transform.right*0.2f);
                    anim.SetIKPosition(AvatarIKGoal.RightHand, hit.point + transform.right * 0.2f);
                }
            }
        }

        if (objectToLook)
        {
            anim.SetLookAtWeight(1);
            lookposition = Vector3.Lerp(lookposition, objectToLook.transform.position, Time.deltaTime*10);
            anim.SetLookAtPosition(lookposition+Vector3.up*.5f);
        }
        
    }

    void Damage()
    {
        if (hit == false)
        {
            lifes--;
            hit = true;
            StartCoroutine(HitTimer());

            if (lifes <= 0)
            {
                StartCoroutine(Die());
            }
            else
            {
                StartCoroutine(Dano());
            }


        }

    }
    IEnumerator HitTimer()
    {
        yield return new WaitForSeconds(2f);
        hit = false;
    }

}
