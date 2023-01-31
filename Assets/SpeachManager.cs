using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeachManager : MonoBehaviour
{
    public static SpeachManager instance;
    public float heightFromHead = 2;
    public float timeToDestroyText = 3f;
    public TextMeshPro speechModel;


    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void speak(GameObject person,string text)
    {
      
       
        TextMeshPro speechClone = Instantiate(speechModel,person.transform.position,Quaternion.identity,person.transform);
       
        speechClone.transform.LookAt(Camera.main.transform);
        speechClone.gameObject.transform.localEulerAngles+= new Vector3(0, 180, 0);
        speechClone.transform.localPosition += new Vector3(0, heightFromHead,0);
        speechClone.text = text;

       Destroy(speechClone.gameObject, timeToDestroyText);
    }
}
