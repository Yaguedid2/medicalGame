using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;
using System;

public class GameManager : MonoBehaviour
{
    Camera cam;
    public TextMeshProUGUI objectTitle;
    public Transform spawnPosition;
    Animator playerAnimator;
    public NavMeshAgent player;
    public static GameManager instance;
    bool inventoryDisplayed = false;
    public GameObject Client;
    public List<GameObject> listOfClients = new List<GameObject>();
    public Transform patientChair, scale, heightScale, tensionScale, thiknessScale,clientStandingPoint,telescope;
    public GameObject scaleFigure,heightScaleFigure,thiknessFigure,tensionFigure;
    public TextMeshPro scaleText,heightScaleText,thiknessScaleText,tensionScaleText;
    public GameObject resultPanel,whichMeans,supervisor,youPassedText,youFailedText,ReportPanel;
    public TextMeshProUGUI yourEstimationText,aiEstimationText;
    public Camera endSceneCamera;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        cam = Camera.main;
        objectTitle.gameObject.SetActive(false);
        playerAnimator = player.GetComponent<Animator>();
        instantianteClient();
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
    public void instantianteClient()
    {
        Client = Instantiate(listOfClients[UnityEngine.Random.Range(0,listOfClients.Count)]);
        Client.transform.position = spawnPosition.position;
    }

    public bool PlayerDecision_willHaveDiabets = false;

    public void takeDecision(bool willHaveDiabetes)
    {
        PlayerDecision_willHaveDiabets = willHaveDiabetes;
        StartCoroutine(_showResult());
    }
    bool win = false;
    float perc;
    public void showResult(string result)
    {
        Debug.Log(result);
        result =result.Substring(1,4);

        result=result.Replace(".", ",");
         perc=float.Parse(result)*100;
        Debug.Log(perc);
       

    }
    IEnumerator _showResult()
    {
        resultPanel.SetActive(true);
        ReportPanel.SetActive(false);
        if (PlayerDecision_willHaveDiabets)
            yourEstimationText.text = "Has diabetes";
        else
            yourEstimationText.text = "Don't Have diabetes";
        yield return new WaitForSeconds(2f);
        aiEstimationText.text = "There is a pourcentage of " + perc + " % That the client has diabetes";
        if (perc < 50)
            if (PlayerDecision_willHaveDiabets)
                win = false;
            else
                win = true;
        else
             if (PlayerDecision_willHaveDiabets)
            win = true;
        else
            win = false;

        yield return new WaitForSeconds(2f);
        whichMeans.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        whichMeans.SetActive(false);
        resultPanel.SetActive(false);
        endSceneCamera.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        if(win)
        {
            supervisor.GetComponent<Animator>().Play("rightAnswer");
            youPassedText.SetActive(true);
        }
        else
        {
            supervisor.GetComponent<Animator>().Play("wrongAnswer");
            youFailedText.SetActive(true);
        }
    }

}
