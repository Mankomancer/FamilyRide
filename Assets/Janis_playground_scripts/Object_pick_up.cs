using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_pick_up : MonoBehaviour
{
    [SerializeField] private float carHoldZ_offset = 0.1f;
    private Input_controlls controlls;
    private bool Action_button;
    public bool boughtCone = false;
    private GameObject hold_object; // salabo negative scale varning, tika izmantota nepareiza metode nest objektus
    [SerializeField] private GameObject item_hold_spot;

    private void Awake()
    {
        Action_button = false;
        controlls = new Input_controlls();
        controlls.Gameplay.Action.performed += ctx => Action_set();
        controlls.Gameplay.Action.canceled += ctx =>Action_Release();
    }

    private void Update()
    {
        if (hold_object)
        {//objekta nešanas funkcija
            var newZ = item_hold_spot.transform.position.z - carHoldZ_offset;
           if(hold_object.tag=="Auto")
             newZ = item_hold_spot.transform.position.z + carHoldZ_offset;
           hold_object.transform.position = new Vector3(item_hold_spot.transform.position.x,hold_object.transform.position.y,newZ);
        }
    }

    public void HandleTrigger(Collider other)
    {
        Rigidbody otherRigidbody = other.attachedRigidbody;
        
        if (Action_button && other && ScoreManager.itemSlot == null && other.tag!="Shop")
        {
            if (other.tag=="Cone" && !boughtCone && ScoreManager.money>=ScoreManager.conePrice){ //buying cone
                ScoreManager.DecimateMoney(ScoreManager.conePrice);
                boughtCone = true;
            }
            if (other.tag!="Cone" || boughtCone)
            {   //in case player already bought cone, but dropped it somewhere else
                if (otherRigidbody != null)
                {
                    ScoreManager.InsertItem(other.transform.parent.gameObject);
                }
                else
                {
                    ScoreManager.InsertItem(other.gameObject);
                }

                hold_object = ScoreManager.ItemRecall();
            }
        }
    }

    void Action_Release()
    {
        Action_button = false;
    }
    void Action_set()
    {
        if (ScoreManager.itemSlot == null)
        {
            Action_button = true;
            return;
        }
        ReleaseItem();  //maybe, need to decrese height of item released, otherwise they tend to hover
    }

    void ReleaseItem()
    {
        hold_object = null;
       // ScoreManager.ItemRecall().transform.parent = null;
        ScoreManager.DropItem();
    }
    private void OnEnable()
    {
        controlls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controlls.Gameplay.Disable();
    }
}
