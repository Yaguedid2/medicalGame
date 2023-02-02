using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Client : MonoBehaviour
{
    Animator animator;
    NavMeshAgent navMeshAgent;
     Transform patientChair,scale,heightScale,tensionScale,thiknessScale;
    public bool Male = false;
    public bool sitDown = false;
    public bool scaleYouself = false;
    public bool heightScaleYouself = false;
    public bool tensionScaleYouself = false;
    public bool thiknessScaleYourself = false;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0, -180, 0);
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        initiateRefs();
        StartCoroutine(_Enter());
       
    }

    // Update is called once per frame
    void Update()
    {
        if (sitDown)
            StartCoroutine(_sitDown());
        if (scaleYouself)
            StartCoroutine(_scaleYourself());
        if (heightScaleYouself)
            StartCoroutine(_heightScaleYourself());
        if (tensionScaleYouself)
            StartCoroutine(_tensionScaleYourself());
        if (thiknessScaleYourself)
            StartCoroutine(_thiknessScaleYourself());

    }
    IEnumerator _sitDown()
    {
        sitDown = false;
        
      
        animator.Play("walk");
        navMeshAgent.destination = patientChair.position;
     
        while (Vector3.Distance(transform.position,patientChair.transform.position)>1f)
            yield return new WaitForEndOfFrame();
      
        animator.Play("sitDown");
        transform.rotation = Quaternion.Euler(0, -476.429f, 0);
    }
    IEnumerator _scaleYourself()
    {
        scaleYouself = false;


        animator.Play("walk");
        navMeshAgent.destination = scale.position;

        while (Vector3.Distance(transform.position, scale.transform.position) > 1f)
            yield return new WaitForEndOfFrame();
       
        animator.Play("step");
       // transform.rotation = Quaternion.Euler(0, -476.429f, 0);
    }
    IEnumerator _heightScaleYourself()
    {
        heightScaleYouself = false;


        animator.Play("walk");
        navMeshAgent.destination = heightScale.position;

        while (Vector3.Distance(transform.position, heightScale.transform.position) > 1f)
            yield return new WaitForEndOfFrame();
        transform.position = heightScale.position;
        animator.Play("step");
        // transform.rotation = Quaternion.Euler(0, -476.429f, 0);
    }
    IEnumerator _tensionScaleYourself()
    {
        tensionScaleYouself = false;


        animator.Play("walk");
        navMeshAgent.destination = tensionScale.position;

        while (Vector3.Distance(transform.position, tensionScale.transform.position) > 1f)
            yield return new WaitForEndOfFrame();
       // transform.position = heightScale.position;
        animator.Play("layHand");
        transform.rotation = Quaternion.Euler(0, -37.367f, 0);
    }
    IEnumerator _thiknessScaleYourself()
    {
        thiknessScaleYourself = false;


        animator.Play("walk");
        navMeshAgent.destination = thiknessScale.position;

        while (Vector3.Distance(transform.position, thiknessScale.transform.position) > 1f)
            yield return new WaitForEndOfFrame();
        // transform.position = heightScale.position;
        animator.Play("layHand");
        transform.rotation = Quaternion.Euler(0, 62.197f, 0);
    }
    IEnumerator _Enter()
    {

        yield return new WaitForSeconds(2f);

        animator.Play("walk");
        navMeshAgent.destination = GameManager.instance.clientStandingPoint.position;

        while (Vector3.Distance(transform.position, GameManager.instance.clientStandingPoint.position) > 1f)
            yield return new WaitForEndOfFrame();
        animator.Play("talk");
        transform.LookAt(Doctor.instance.transform);
        SpeachManager.instance.speak(gameObject, "Hello !!");
      
        yield return new WaitForSeconds(2f);
        Doctor.instance.sayHello();
        yield return new WaitForSeconds(2f);
        sitDown = true;
    }


    bool checkIfStoped()
    {
        float dist = navMeshAgent.remainingDistance;
        Debug.Log(dist);
        if (dist != Mathf.Infinity && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && navMeshAgent.remainingDistance < 0.1f)
            return true;
        return false;
    }
   void initiateRefs()
    {
       
       
        scale = GameManager.instance.scale;
        heightScale= GameManager.instance.heightScale;
        tensionScale= GameManager.instance.tensionScale;
        thiknessScale= GameManager.instance.thiknessScale;
        patientChair = GameManager.instance.patientChair;
    }
    public int answerAge()
    {
       int age= UnityEngine.Random.Range(18, 70);
        transform.LookAt(Doctor.instance.transform);
        string response = "I Have " + age + "years old";
        SpeachManager.instance.speak(gameObject,response);
        return age;
    }
    public int answerPregnancy()
    {
        int pregnancyCount = UnityEngine.Random.Range(0, 5);
        transform.LookAt(Doctor.instance.transform);
        string response = "I Have been pregnant " + pregnancyCount + " times";
        SpeachManager.instance.speak(gameObject, response);
        return pregnancyCount;
    }
}
