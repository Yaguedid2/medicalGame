using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public static GameObject selectedObject;
    Outline myOutline;
    Camera cam;
    Vector3 initialPosition;
    public bool takable = false;
    void Start()
    {
        myOutline = GetComponent<Outline>();
        myOutline.enabled = false;
        cam = Camera.main;
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(selectedObject==gameObject && !Selected)
        {
            selectMe();
        }
        if (selectedObject != gameObject && Selected)
        {
            UnselectMe();
        }

    }
    bool Selected = false;
    void selectMe()
    {
        Selected = true;
        myOutline.enabled = true;
        transform.position = Vector3.MoveTowards(transform.position, cam.transform.position, 0.5f);
    }
    void UnselectMe()
    {
        Selected = false;
        myOutline.enabled = false;
        transform.position = initialPosition;
    }
}
