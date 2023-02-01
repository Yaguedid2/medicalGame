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
    public GameObject insulineInHand, smallTubeInHand,taskError,tasksWindow,medicalReport;
    public static Doctor instance;
    public GameObject pregnancyTask;
    public Sprite done;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if(GameManager.instance.Client.GetComponent<Client>().Male)
        {
            pregnancyTask.GetComponentInChildren<Button>().interactable = false;
            pregnancyTask.GetComponentsInChildren<Image>()[1].sprite = done;
            nbrOfTasks++;
        }    
        setupTasks();
       
    }

    bool onetime = true;
    bool reportShown = false;
    Vector3 actualPosition,latePos;
    void Update()
    {
        if(nbrOfTasks==9 && !reportShown)
        {
            reportShown = true;
            fillReport();
            medicalReport.SetActive(true);
            return;
        }

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


    int nbrOfTasks = 0;

    public void askAboutAge(GameObject t)
    {
        StartCoroutine(askAge(t));
    }
    int clientAge;
    IEnumerator askAge(GameObject t)
    {
        GameObject client=GameManager.instance.Client;
        Vector3 destination = new Vector3(client.transform.position.x - 1, transform.position.y, client.transform.position.z + 1);
        navMeshAgent.destination = destination;
      
        yield return new WaitForSeconds(1f);
        transform.LookAt(client.transform);
        animator.Play("talk");
        SpeachManager.instance.speak(gameObject, "What s your age ?");
        yield return new WaitForSeconds(3f);
       clientAge=client.GetComponent<Client>().answerAge();
        t.GetComponentInChildren<Button>().interactable = false;
        t.GetComponentsInChildren<Image>()[1].sprite = done;
        nbrOfTasks++;
    }
    int clientpregnancyTimes=0;
    public void askAboutPregnancy(GameObject t)
    {
        StartCoroutine(askPregnancy(t));
    }
    IEnumerator askPregnancy(GameObject t)
    {
        GameObject client = GameManager.instance.Client;
        Vector3 destination = new Vector3(client.transform.position.x - 1, transform.position.y, client.transform.position.z + 1);
        navMeshAgent.destination = destination;

        yield return new WaitForSeconds(1f);
        transform.LookAt(client.transform);
        animator.Play("talk");
        SpeachManager.instance.speak(gameObject, "How many Times you've been pregnant ?");
        yield return new WaitForSeconds(3f);
        clientpregnancyTimes = client.GetComponent<Client>().answerPregnancy();
        t.GetComponentInChildren<Button>().interactable = false;
        t.GetComponentsInChildren<Image>()[1].sprite = done;
        nbrOfTasks++;
    }
    int clientScale = 0;
    public void askToGoScale(GameObject t)
    {
        StartCoroutine(askGoScale(t));
    }
    IEnumerator askGoScale(GameObject t)
    {
        GameObject client = GameManager.instance.Client;
        SpeachManager.instance.speak(gameObject, "Can you Go to the scale please ?");
        yield return new WaitForSeconds(2f);
        client.GetComponent<Client>().scaleYouself = true;
        clientScale = UnityEngine.Random.Range(40, 150);
        TextMeshPro scaleText = GameManager.instance.scaleText;

        yield return new WaitForSeconds(2f);
        GameManager.instance.scaleFigure.SetActive(true);
        for(int i=0;i<=clientScale;i+=2)
        {
            scaleText.text = i.ToString();
            yield return new WaitForSeconds(0.005f);
        }
        for(int j=0;j<=3;j++)
        {
            GameManager.instance.scaleText.gameObject.SetActive(j%2==0?false:true);
            yield return new WaitForSeconds(0.3f);
        }

        t.GetComponentInChildren<Button>().interactable = false;
        t.GetComponentsInChildren<Image>()[1].sprite = done;
        nbrOfTasks++;
    }
    int clientHeightScale = 0;
    public void askToGoHeightScale(GameObject t)
    {
        StartCoroutine(askGoHeightScale(t));
    }
    IEnumerator askGoHeightScale(GameObject t)
    {
        GameObject client = GameManager.instance.Client;
       
        
        client.GetComponent<Client>().heightScaleYouself = true;
        clientHeightScale = UnityEngine.Random.Range(150, 220);
        yield return new WaitForSeconds(2f);
        GameManager.instance.heightScaleFigure.SetActive(true);
        yield return new WaitForSeconds(2f);
        TextMeshPro heightScaleText = GameManager.instance.heightScaleText;
        heightScaleText.text = clientHeightScale.ToString();
        for (int j = 0; j <= 3; j++)
        {
            GameManager.instance.heightScaleText.gameObject.SetActive(j % 2 == 0 ? false : true);
            yield return new WaitForSeconds(0.3f);
        }
        t.GetComponentInChildren<Button>().interactable = false;
        t.GetComponentsInChildren<Image>()[1].sprite = done;
        nbrOfTasks++;
    }
    int clientThiknessScale =0;
    public void askToGoThiknessScale(GameObject t)
    {
        StartCoroutine(askGoThiknessScale(t));
    }
    IEnumerator askGoThiknessScale(GameObject t)
    {
        GameObject client = GameManager.instance.Client;
        SpeachManager.instance.speak(gameObject, "Can you Go to thickness scale ?");
        yield return new WaitForSeconds(2f);
        client.GetComponent<Client>().thiknessScaleYourself = true;
        clientThiknessScale = UnityEngine.Random.Range(10, 40);
        yield return new WaitForSeconds(2f);
        GameManager.instance.thiknessFigure.SetActive(true);
        yield return new WaitForSeconds(2f);
        TextMeshPro thikenessScaleText = GameManager.instance.thiknessScaleText;
        thikenessScaleText.text = clientThiknessScale.ToString();
        for (int j = 0; j <= 3; j++)
        {
            GameManager.instance.thiknessScaleText.gameObject.SetActive(j % 2 == 0 ? false : true);
            yield return new WaitForSeconds(0.3f);
        }
        t.GetComponentInChildren<Button>().interactable = false;
        t.GetComponentsInChildren<Image>()[1].sprite = done;
        nbrOfTasks++;
    }
    int clientBloodPressureScale = 0;
    public void askToGotensionScale(GameObject t)
    {
        StartCoroutine(askGoTensionScale(t));
    }
    IEnumerator askGoTensionScale(GameObject t)
    {
        GameObject client = GameManager.instance.Client;
        SpeachManager.instance.speak(gameObject, "Can you Go to blood pressure scale ?");
        yield return new WaitForSeconds(2f);
        client.GetComponent<Client>().tensionScaleYouself = true;
        int rand = UnityEngine.Random.Range(0, 10);
        if(rand>7)
             clientBloodPressureScale = UnityEngine.Random.Range(90, 120);
        else
            clientBloodPressureScale = UnityEngine.Random.Range(55, 90);
        yield return new WaitForSeconds(2f);
        GameManager.instance.tensionFigure.SetActive(true);
        yield return new WaitForSeconds(2f);
        TextMeshPro tensionScaleText = GameManager.instance.tensionScaleText;
        tensionScaleText.text = clientBloodPressureScale.ToString();
        for (int j = 0; j <= 3; j++)
        {
            GameManager.instance.tensionScaleText.gameObject.SetActive(j % 2 == 0 ? false : true);
            yield return new WaitForSeconds(0.3f);
        }
        t.GetComponentInChildren<Button>().interactable = false;
        t.GetComponentsInChildren<Image>()[1].sprite = done;
        nbrOfTasks++;
    }
    int clientInsulinScale = 0;
    public void takeInsulineScale(GameObject t)
    {
        StartCoroutine(_takeInsulineScale(t));
    }
    IEnumerator _takeInsulineScale(GameObject t)
    {
       if(listOfItemsInPocket.Find(i => i.name == "Diabete Meter")!=null)
        {
            insulineInHand.SetActive(true);
            Vector3 destination = GameManager.instance.Client.transform.position;
            animator.Play("walk");
            navMeshAgent.destination = destination;
            while (Vector3.Distance(transform.position, destination) > 1)
                yield return new WaitForEndOfFrame();
        
            animator.Play("layHandInsuline");
            yield return new WaitForSeconds(3f);
            animator.Play("walk");
            destination = GameManager.instance.telescope.transform.position;
            navMeshAgent.destination = destination;
            while (Vector3.Distance(transform.position, destination) > 3)
                yield return new WaitForEndOfFrame();
          
            insulineInHand.SetActive(false);
            animator.Play("layHandInsuline");
            yield return new WaitForSeconds(3f);
            animator.Play("idle");
            removeElementFromInventory("Diabete Meter");
            int rand = UnityEngine.Random.Range(0, 10);
            if(rand>6)
            {
                clientInsulinScale = UnityEngine.Random.Range(200, 500);
            }else
                clientInsulinScale = UnityEngine.Random.Range(5, 200);
            t.GetComponentInChildren<Button>().interactable = false;
            t.GetComponentsInChildren<Image>()[1].sprite = done;
            nbrOfTasks++;
        }
        else
        {
            tasksWindow.SetActive(true);
            taskError.SetActive(true);
            taskError.GetComponentInChildren<TextMeshProUGUI>().text = "You have to collect insulin meter to be able to do the 2-Hour serum insulin test";
        }
    }


    int clientGlucoseScale = 0;

    public void takeglucoseScale(GameObject t)
    {
        StartCoroutine(_takeglucoseScale(t));
    }
    IEnumerator _takeglucoseScale(GameObject t)
    {
        if (listOfItemsInPocket.Find(i => i.name == "Small Tube") != null)
        {
            smallTubeInHand.SetActive(true);
            Vector3 destination = GameManager.instance.Client.transform.position;
            animator.Play("walk");
            navMeshAgent.destination = destination;
            while (Vector3.Distance(transform.position, destination) > 1)
                yield return new WaitForEndOfFrame();

            animator.Play("layHandInsuline");
            yield return new WaitForSeconds(3f);
            animator.Play("walk");
            destination = GameManager.instance.telescope.transform.position;
            navMeshAgent.destination = destination;
            while (Vector3.Distance(transform.position, destination) > 3)
                yield return new WaitForEndOfFrame();

            smallTubeInHand.SetActive(false);
            animator.Play("layHandInsuline");
            yield return new WaitForSeconds(3f);
            animator.Play("idle");
            removeElementFromInventory("Small Tube");
            clientGlucoseScale = UnityEngine.Random.Range(88, 200);
            t.GetComponentInChildren<Button>().interactable = false;
            t.GetComponentsInChildren<Image>()[1].sprite = done;
            nbrOfTasks++;
        }
        else
        {
            tasksWindow.SetActive(true);
            taskError.SetActive(true);
            taskError.GetComponentInChildren<TextMeshProUGUI>().text = "You have to collect a small tube to be able to do the Plasma glucose concentration  test";
        }
    }
    float clientDiabetespedigree = 0;
    public void calculateDiabetespedigree(GameObject t)
    {
        clientDiabetespedigree = UnityEngine.Random.Range(0f,1f);
        t.GetComponentInChildren<Button>().interactable = false;
        t.GetComponentsInChildren<Image>()[1].sprite = done;
        nbrOfTasks++;
    }



    public void sayHello()
    {
        GameObject client = GameManager.instance.Client;
        transform.LookAt(client.transform);
        animator.Play("talk");
        SpeachManager.instance.speak(gameObject, "Hello,please take a seat !");
    }
    public void askToSitDown()
    {
        GameObject client = GameManager.instance.Client;
        client.GetComponent<Client>().sitDown = true;
    }
    void removeElementFromInventory(string elemName)
    {
        //searchForThe element
        int elemCount = 0;
        itemToRemove = null;
        foreach (GameObject item in listOfItemsInPocket)
            if (item.name == elemName)
            {
                elemCount++;
                itemToRemove = item;
            }
       
        listOfItemsInPocket.Remove(itemToRemove);
      

      


        if (elemCount == 1)
        {
            Destroy(listOfItemNoRed.Find(i => i.GetComponentsInChildren<TextMeshProUGUI>()[0].text == elemName));
            listOfItemNoRed.Remove(listOfItemNoRed.Find(i => i.GetComponentsInChildren<TextMeshProUGUI>()[0].text == elemName));
        }
        else
        {
            GameObject ivenItem = listOfItemNoRed.Find(i => i.GetComponentsInChildren<TextMeshProUGUI>()[0].text == elemName);
            int itemCount = Int32.Parse(ivenItem.GetComponentsInChildren<TextMeshProUGUI>()[1].text);
            itemCount--;
            ivenItem.GetComponentsInChildren<TextMeshProUGUI>()[1].text = itemCount.ToString();
        }


    }
    public TextMeshProUGUI numberOfTimePregneantText, glucoseConcentrationText, bloodPressureText, thiknessText, insulineText, bodyMassIndex, diabeteFunctionText,clientNameText,genderText;
    public RawImage clientImage;
    public RenderTexture clientTexture;
    void fillReport()
    {
     
        numberOfTimePregneantText.text = clientpregnancyTimes.ToString();
        glucoseConcentrationText.text = clientGlucoseScale.ToString() + " mg/dL";
        bloodPressureText.text = clientBloodPressureScale.ToString() + " mm Hg";
        thiknessText.text = clientThiknessScale.ToString() + " mm";
        insulineText.text = clientInsulinScale.ToString() + " mu U/ml";
        float cmTom = clientHeightScale * 0.01f;
        float BMI = clientScale / (cmTom*cmTom);
        bodyMassIndex.text = BMI.ToString() + " Kg/m²";
        diabeteFunctionText.text = clientDiabetespedigree.ToString();
        GameManager.instance.Client.GetComponent<Animator>().Play("idle");
        if(GameManager.instance.Client.GetComponent<Client>().Male)
        {
            clientNameText.text = Mennames[UnityEngine.Random.Range(0, Mennames.Count)];
            genderText.text = "Male";
        }
        else
        {
            clientNameText.text = Womennames[UnityEngine.Random.Range(0, Womennames.Count)];
            genderText.text = "Female";
        }
        clientImage.texture = clientTexture;
        listOfResults.Add(clientpregnancyTimes);
        listOfResults.Add(clientGlucoseScale);
        listOfResults.Add(clientBloodPressureScale);
        listOfResults.Add(clientThiknessScale);
        listOfResults.Add(clientInsulinScale);
        listOfResults.Add(BMI);
        listOfResults.Add(clientDiabetespedigree);
        listOfResults.Add(clientAge);
        FecthServer.instance.call(listOfResults);


    }
    private List<string> Mennames = new List<string>
    {
        "Mohammed", "Ahmed", "Ali", "Abdullah", "Omar",
        "Hassan", "Sayed", "Youssef", "Tarek", "Nasser"
    };
    private List<string> Womennames = new List<string>
    {
        "Fatima", "Aisha", "Leila", "Sarah", "Nura",
        "Najwa", "Rania", "Hana", "Lamis", "Mona"
    };
    List<float> listOfResults = new List<float>();




  
}
