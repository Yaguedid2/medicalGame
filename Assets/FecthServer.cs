using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System;



public class FecthServer : MonoBehaviour
{
   
    public static FecthServer instance;
 
    public static string ip = "192.168.11.104";
   
   
    private void Awake()
    {
        instance = this;
    }
   
    private void Start()
    {


        LoadModel();

     

    }
    private void Update()
    {
        
       
       
       
    }


    public void call(List<float> listOfValues)
    {
        
      
        getResponse(listOfValues);
       
        
    }


    public static async void getResponse(List<float> values)
    {
        //Define your baseUrl
        string baseUrl = "http://"+ip+":7000"+"/predict/" +values[0]+"/"+values[1] + "/" + values[2] + "/" + values[3] + "/" + values[4] + "/" + values[5] + "/" + values[6] + "/" + values[7];
        //Have your using statements within a try/catch block
        try
        {
            //We will now define your HttpClient with your first using statement which will use a IDisposable.
            using (HttpClient client = new HttpClient())
            {
                //In the next using statement you will initiate the Get Request, use the await keyword so it will execute the using statement in order.
                //The HttpResponseMessage which contains status code, and data from response.
                using (HttpResponseMessage res = await client.GetAsync(baseUrl))
                {
                    //Then get the data or content from the response in the next using statement, then within it you will get the data, and convert it to a c# object.
                    using (HttpContent content = res.Content)
                    {
                        var data = await content.ReadAsStringAsync();
                        GameManager.instance.showResult(data);
                        
                    }
                }
            }
        }
        catch (Exception e)
        {

        }
       
    }
    



    bool modelLoaded = false;
    bool readyToGetResponse = false;

    public  async void LoadModel()
    {
        //Define your baseUrl
        string baseUrl = "http://"+ip+":7000"+"/loadModel";
        //Have your using statements within a try/catch block
        try
        {
            //We will now define your HttpClient with your first using statement which will use a IDisposable.
            using (HttpClient client = new HttpClient())
            {
                //In the next using statement you will initiate the Get Request, use the await keyword so it will execute the using statement in order.
                //The HttpResponseMessage which contains status code, and data from response.
                using (HttpResponseMessage res = await client.GetAsync(baseUrl))
                {
                    //Then get the data or content from the response in the next using statement, then within it you will get the data, and convert it to a c# object.
                    using (HttpContent content = res.Content)
                    {
                        var data = await content.ReadAsStringAsync();
                        //If the data isn't null return log convert the data using newtonsoft JObject Parse class method on the data.
                        
                    }
                }
            }
        }
        catch (Exception e)
        {

        }
    }

   
  
  
  
  
}