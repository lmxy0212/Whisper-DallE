using OpenAI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities.Extensions;
using System.Collections.Generic;
using OpenAI.Chat;
using OpenAI.Models;
using HuggingFace.API;

public class DallEGenerateImg : MonoBehaviour
{
    public string m_prompt;
    public GameObject m_plane;
    public List<GameObject> imgs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("space key was pressed");
            GenerateImage(m_prompt, m_plane.transform);
        }
    }

    public async void GenerateImage(string prompt, Transform transform, Action<Transform, Texture2D> callBack = null)
    {

        try
        {
            var api = new OpenAIClient();

            var messages = new List<Message>
            {
                new Message(Role.System, "You speaks like Shakespeare. And you are the prompty engineer to DallE. " +
                "Summarize what the user said in 1000 characters."),
                new Message(Role.User, prompt),
            };
            var chatRequest = new ChatRequest(messages, Model.GPT3_5_Turbo);
            var chat_result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
            Debug.Log(chat_result);

            var results = await api.ImagesEndPoint.GenerateImageAsync(chat_result, 3, OpenAI.Images.ImageSize.Small);
            GameObject prev = m_plane;
            foreach (GameObject plane in imgs)
            {
                plane.Destroy();
            }
            foreach (var (path, texture) in results)
            {
                Debug.Log(path);
                // path == file://path/to/image.png
                Assert.IsNotNull(texture);
                // texture == The preloaded Texture2D
                GameObject newPlane = Instantiate(m_plane, prev.transform.position + new Vector3(0.6f,0f,0f), m_plane.transform.rotation);
                prev = newPlane;
                newPlane.transform.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
                imgs.Add(newPlane);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}


