using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class VRAudioImagePlayer : MonoBehaviour
{
    private AudioSource audioSource;
    List<float> times = new List<float>();

    private float currentTime = 0;
    private int currentImageIndex = 0;
    private bool isStarted = false;
    private MeshRenderer meshRenderer;

    public GameObject targetObject;
    private Material[] materials;

    public GameObject menu;
    

    bool imagesEnabled = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        for (int i=0; i< 3 ; i++)
        {
            times.Add(8.0f); //set here manually
        }

        meshRenderer = targetObject.GetComponent<MeshRenderer>();
        materials = meshRenderer.materials;


    }

    public void SetSessionStart(int session) 
    {
        // enum 0 = session 1, enum 1 = session 2
        menu.SetActive(false);

        if (session == 0)
        {
            imagesEnabled = false;
        } else if (session == 1)
        {
            imagesEnabled = true;   
        }
        isStarted = true;
    }


    void Update()
    {
        // Check if the start variable is set to true
        if (isStarted)
        {
            // Play the audio
            audioSource.Play();

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
        

        
        yield return new WaitForSeconds(times[currentImageIndex]);
        // Increment the current image index
        currentImageIndex++;

        // Check if we have reached the end of the list
        if (currentImageIndex >= times.Count)
        {
           
        } else
        {
            // Schedule the next image change
            StartCoroutine("ChangeImage");
        }
    }
}
