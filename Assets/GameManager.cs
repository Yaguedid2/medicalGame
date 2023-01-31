using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    Camera cam;
    public TextMeshProUGUI objectTitle;
    Animator playerAnimator;
    public NavMeshAgent player;
    public static GameManager instance;
    bool inventoryDisplayed = false; 
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        cam = Camera.main;
        objectTitle.gameObject.SetActive(false);
        playerAnimator = player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inventoryDisplayed)
            return;
        hover();
       if (Input.GetMouseButtonDown(0) && !alreadyInQuest)
        {
            selectObject(); 
        }

        
    }
    void selectObject()
    {
        GameObject onSpot = fireRay();
        if (onSpot != null)
        {
           
            if (onSpot.GetComponent<SelectableObject>() != null)
            {
                StartCoroutine(startQuest(onSpot));
            }



         
        }
    }
    void hover()
    {
        GameObject onSpot = fireRay();
        if (onSpot != null)
        {
            objectTitle.gameObject.SetActive(true);
            if (onSpot.GetComponent<SelectableObject>() != null)
            {
                SelectableObject.selectedObject = onSpot;
            }else
                SelectableObject.selectedObject = null;



            objectTitle.text = onSpot.name;
        }
    }

    Vector2 hitPoint;
    GameObject fireRay()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            hitPoint = hit.point;
            return hit.transform.gameObject;
        }
        else
            hitPoint = Vector2.zero;
        
        return null;
    }

    public bool alreadyInQuest = false;
    IEnumerator startQuest(GameObject objectToBehaveOn)
    {
        alreadyInQuest = true;
        Vector3 actualPosition = player.transform.position;
        Vector3 destination = new Vector3(objectToBehaveOn.transform.position.x, actualPosition.y, objectToBehaveOn.transform.position.z);
       
       
            player.destination = destination;
        player.GetComponent<Doctor>().objectToBehaveWith = objectToBehaveOn;
            playerAnimator.Play("walk");
        player.GetComponent<Doctor>().amIdle = false;

        if(objectToBehaveOn.GetComponent<SelectableObject>()!=null && objectToBehaveOn.GetComponent<SelectableObject>().takable)
        {

            while (!player.GetComponent<Doctor>().amIdle)
                yield return new WaitForEndOfFrame();

            player.GetComponent<Doctor>().addItemToPocket(objectToBehaveOn);
        }
        yield return new WaitForEndOfFrame();





    }
    public void showInventory()
    {
        inventoryDisplayed = true;
    }
    public void hideInventory()
    {
        inventoryDisplayed = false;
    }
}
