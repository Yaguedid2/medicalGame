using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateCamera : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{
    Camera cam;
    public bool right;
    bool pointerIn=false;
    public void OnPointerDown(PointerEventData eventData)
    {
       

    }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(pointerIn)
        {
            Vector3 rotation = cam.transform.eulerAngles;
            if (right)
                rotation += new Vector3(0, 1, 0);
            else
                rotation += new Vector3(0, -1, 0);
            cam.transform.eulerAngles = rotation;
        }
       
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerIn = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerIn = false;
    }
}
