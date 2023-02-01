using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;
using System;
public class Doctor : MonoBehaviour
{
    public GameObject objectToBehaveWith;
    public GameObject inventoryItem, Inventory;
    public bool amIdle = false;
    Animator animator;
    NavMeshAgent navMeshAgent;
    public RenderTexture smallTubeImage, diabeteMeterImage;
    List<GameObject> listOfItemsInPocket = new List<GameObject>();
    Dictionary<string, bool> tasks = new Dictionary<string, bool>();

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        setupTasks();
    }

    bool onetime = true;
    Vector3 actualPosition,latePos;
    void Update()
    {
        actualPosition = transform.position;
       

        
        if (checkIfStoped() && onetime)
        {

            onetime = false;
            if (checkTasks())
            {
                animator.SetBool("idle", true);
                amIdle = true;
                GameManager.instance.alreadyInQuest = false;
            }
            else
                onetime = true;
           
        }
        else if (!checkIfStoped())
        {
            onetime = true;
            animator.SetBool("idle", false);
        }
    }
 
   bool checkIfStoped()
    {
        float dist = navMeshAgent.remainingDistance;

        if (dist != Mathf.Infinity && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && navMeshAgent.remainingDistance < 0.1f)
            return true;
        return false;
    }
    List<GameObject> listOfItemNoRed= new List<GameObject>();
  
    public void addItemToPocket(GameObject item)
    {
        tasks["takeElement"] = true;
        int itemCount = 1;
        transform.LookAt(item.transform);
        GameObject invenItem;
        foreach (GameObject i in listOfItemsInPocket)
        {
            if (i.name == item.name)
                itemCount++;
        }
        if (itemCount == 1)
        {
            listOfItemsInPocket.Add(item);
             invenItem = Instantiate(inventoryItem,Inventory.transform);
            invenItem.transform.localScale = new Vector3(8.215859f, 0.4915323f, 1);
            invenItem.GetComponentsInChildren<TextMeshProUGUI>()[0].text = item.name;
            invenItem.GetComponentsInChildren<TextMeshProUGUI>()[1].text = itemCount.ToString();
            listOfItemNoRed.Add(invenItem);
            switch(item.name)
            {
                case "Diabete Meter":invenItem.GetComponentInChildren<RawImage>().texture = diabeteMeterImage;break;
                case "Small Tube": invenItem.GetComponentInChildren<RawImage>().texture = smallTubeImage; break;

            }
            invenItem.GetComponentInChildren<Button>().onClick.AddListener(() => returnBackElement(invenItem.GetComponentsInChildren<TextMeshProUGUI>()[0]));

        }
        else
        {
            listOfItemsInPocket.Add(item);
            foreach (GameObject i in listOfItemNoRed)
                if(i.GetComponentsInChildren<TextMeshProUGUI>()[0].text==item.name)
                {
                    i.GetComponentsInChildren<TextMeshProUGUI>()[1].text = itemCount.ToString();
                    break;
                }
           
        }
     
        item.SetActive(false);
        
    }
    GameObject itemToRemove = null;
    public void returnBackElement(TextMeshProUGUI ElementName)
    {
        //searchForThe element
        int elemCount = 0;
         itemToRemove=null;
        foreach(GameObject item in listOfItemsInPocket)
            if(item.name==ElementName.text)
            {
                elemCount++;
                itemToRemove = item;
            }
        Debug.Log(elemCount);
        listOfItemsInPocket.Remove(itemToRemove);
        animator.Play("walk");
        tasks["putElement"] = true;
     
        navMeshAgent.destination=itemToRemove.transform.position;
        
       
        if (elemCount==1)
        {
            Destroy(listOfItemNoRed.Find(i => i.GetComponentsInChildren<TextMeshProUGUI>()[0].text == ElementName.text));
            listOfItemNoRed.Remove(listOfItemNoRed.Find(i => i.GetComponentsInChildren<TextMeshProUGUI>()[0].text == ElementName.text));
        }
        else
        {
            GameObject ivenItem = listOfItemNoRed.Find(i => i.GetComponentsInChildren<TextMeshProUGUI>()[0].text == ElementName.text);
            int itemCount =Int32.Parse(ivenItem.GetComponentsInChildren<TextMeshProUGUI>()[1].text);
            itemCount--;
            ivenItem.GetComponentsInChildren<TextMeshProUGUI>()[1].text = itemCount.ToString();
        }
         

       
    }
    bool checkTasks()
    {
        foreach (string task in tasks.Keys)
            if (tasks[task] == true)
            {
                doTask(task);
                return false;
            }
        return true;
    }
    void doTask(string taskName)
    {
        switch(taskName)
        {
            case "takeElement": animator.Play("layHand");break;
            case "putElement": transform.LookAt(itemToRemove.transform); animator.Play("layHand"); itemToRemove.SetActive(true); break;

        }
        tasks[taskName] = false;
    }
    void setupTasks()
    {
        tasks.Add("takeElement", false);
        tasks.Add("putElement", false);
    }
    public void addTask(string taskName)
    {
        tasks[taskName] = true;
    }


    public void askAboutAge()
    {
        StartCoroutine(askAge());
    }
    IEnumerator askAge()
    {
        GameObject client=GameManager.instance.Client;
        Vector3 destination = new Vector3(client.transform.position.x - 1, transform.position.y, client.transform.position.z + 1);
        navMeshAgent.destination = destination;
        while (!checkIfStoped())
            yield return new WaitForEndOfFrame();
        SpeachManager.instance.speak(gameObject, "What s your age ?");
    }
}
