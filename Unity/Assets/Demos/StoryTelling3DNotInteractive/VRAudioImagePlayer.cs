using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Org.BouncyCastle.Utilities;

public class VRAudioImagePlayer : MonoBehaviour
{
    public List<AudioSource> audioSource;
    
    List<List<float>> times = new List<List<float>>();
    

    private float currentTime = 0;
    private int currentImageIndex = 0;
    private bool isStarted = false;
    private MeshRenderer meshRenderer;

    public GameObject targetObject001;
    public GameObject targetObject002;
    private int currentStory = 0;
    private Material[] materials;

    public GameObject menu;

    bool imagesEnabled = false;


    private void Start()
    {
        times.Add(new List<float>() {7.0f, 7.0f, 5.0f, 5.0f, 7.0f, 4.0f, 8.0f, 5.0f, 6.0f, 6.0f, 6.0f, 7.0f}); //first story times
        times.Add(new List<float>() { 4.0f, 6.0f, 4.0f, 5.0f, 4.0f, 5.0f, 4.0f, 5.0f, 4.0f, 3.0f, 5.0f, 4.0f, 4.0f, 5.0f, 7.0f }); //second story times


    }

    public void SetSessionStart(int session) 
    {

        // enum 0 = session 1, enum 1 = session 2
        menu.SetActive(false);

        if (session % 2 ==  0) // audio only
        {
            imagesEnabled = false;
            
            if(session == 0)
            {
                meshRenderer = targetObject001.GetComponent<MeshRenderer>();
                currentStory = 0;
            } else if (session == 2)
            {
                meshRenderer = targetObject002.GetComponent<MeshRenderer>();
                currentStory = 1;
            }
            
            materials = meshRenderer.materials;
            

        } else if (session %2 == 1) // adio and images
        {
            imagesEnabled = true;

            if (session == 1)
            {
                meshRenderer = targetObject001.GetComponent<MeshRenderer>();
                currentStory = 0;
            }
            else if (session == 3)
            {
                meshRenderer = targetObject002.GetComponent<MeshRenderer>();
                currentStory = 1;
            }
        }
        isStarted = true;
    }


    void Update()
    {
        // Check if the start variable is set to true
        if (isStarted)
        {
            // Play the audio
            audioSource[currentStory].Play();

            meshRenderer.enabled = imagesEnabled;

            isStarted = false;

            StartCoroutine("ChangeImage");
        }
    }

    IEnumerator ChangeImage()
    {
        
        Debug.Log("ChangeImage " + currentImageIndex.ToString());

        // Get the list of materials

        // Select the material that you want to make the main material

        meshRenderer.material.mainTexture = meshRenderer.materials[currentImageIndex].mainTexture;
        

        
        yield return new WaitForSeconds(times[currentStory][currentImageIndex]);
        // Increment the current image index
        currentImageIndex++;

        // Check if we have reached the end of the list
        if (currentImageIndex >= times[currentStory].Count)
        {
            meshRenderer.enabled = false;
        } else
        {
            // Schedule the next image change
            StartCoroutine("ChangeImage");
        }
    }
}
