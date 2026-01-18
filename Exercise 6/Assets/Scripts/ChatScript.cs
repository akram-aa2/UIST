using System;
using System.Collections;
using System.Collections.Generic;
using Gpt4All;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ChatScript : MonoBehaviour
{

    // LlmManager to call NLP model
    public LlmManager llmManager;

    /**
    *  Get all the objects necessary for the agent chat.
    *  Something to get the user input, you will also need to decide how to tell the script to send the input to the agent.
    *  Something to display the user's and agent's messages. You probably want to place the messages in the ScrollRect's content object. 
    *  You need to be able to hide and display the loading icon. You can do this by setting the active state of the loading icon's game object. The loading icon should also be animated via script or animation. (e.g. rotate the icon)
    *  What color should the agent's messages be? What color should the user's messages be?
    **/

    // TMP Input Field
    public TMPro.TMP_InputField inputField;

    // User bubble prefab
    public GameObject userBubblePrefab;

    // Agent bubble prefab
    public GameObject agentBubblePrefab;

    // Colors for the chat
    //public Color userColor;

    // Agent animator
    public Animator agentAnimator;

    // Scroll View
    public ScrollRect scrollRect;

    // Scroll Content
    public GameObject scrollContent;

    // loading icon
    public GameObject loadingIcon;

    // Was agent bubble created for the current response
    private GameObject agentBubble;

    // TODO Add missing references  

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to the OnResponseUpdated event
        llmManager.OnResponseUpdated += OnResponseHandler;

        // Any code here will be executed when the scene starts.
    }

    
    /// <summary>
    /// Event handler for handling the response from the agent.
    /// This method is always called when the agent responds partially or fully. (e.g. "Hello" -> "Hello, how" -> "Hello, how are you?")
    /// This method is not run on the main thread. You cannot modify the UI from this method. You can use a coroutine to execute code on the main thread. See ChangeAnimatorProperty for an example.
    /// </summary>
    /// <param name="response">The response received from the agent.</param>
    private void OnResponseHandler(string response)
    {
        // When this method is called the first time after the user sends a message, we know that the agent started responding. We should do something with that information.
        
        // TODO Add code here
        if (agentBubble == null)
        {
            // Deactivate loading icon
            loadingIcon.SetActive(false);

            // Create agent chat bubble
            agentBubble = Instantiate(agentBubblePrefab, scrollContent.transform);

            // Start agent speaking animation
            StartCoroutine(ChangeAnimatorProperty(agentAnimator, "isTalking", true));
        }
        TMPro.TMP_Text agentBubbleText = agentBubble.GetComponentInChildren<TMPro.TMP_Text>();
        agentBubbleText.text = "Agent:\n" + response;
        
        // Scroll to the bottom of the scroll view
        StartCoroutine(ScrollToBottom(scrollRect));
    }

    
    /// <summary>
    /// Coroutine that scrolls the scroll rect to the bottom. Call via StartCoroutine(ScrollToBottom(scrollRectReference)); https://docs.unity3d.com/Manual/Coroutines.html
    /// </summary>
    /// <returns>An IEnumerator used for coroutine execution.</returns>
    IEnumerator ScrollToBottom(ScrollRect sr)
    {
        yield return new WaitForEndOfFrame();
        sr.verticalNormalizedPosition = 0f;
    }

    public void OnSendButton()
    {
        // TODO Add code here
        string usertext = inputField.text;
        inputField.text = "";
        inputField.interactable = false;

        // Create user chat bubble
        GameObject userBubble = Instantiate(userBubblePrefab, scrollContent.transform);
        TMPro.TMP_Text userBubbleText = userBubble.GetComponentInChildren<TMPro.TMP_Text>();
        userBubbleText.text = "User:\n" + usertext;

        // Activate loading icon
        loadingIcon.SetActive(true);
        loadingIcon.transform.SetAsLastSibling ();

        // Scroll to the bottom of the scroll view
        StartCoroutine(ScrollToBottom(scrollRect));
        
        // Send text to the agent
        SendText(usertext);
    }


    /// <summary>
    /// Sends the specified text to the chat manager. OnResponseHandler will be called when the agent responds.
    /// </summary>
    /// <param name="txt">The text to send.</param>
    public async void SendText(string txt)
    {
        // Await will wait until the response is fully received before continuing.
        await llmManager.Prompt(txt);

        // Any code here will be executed after the response is fully received.

        // Re-enable input field
        inputField.interactable = true;

        // Stop agent speaking animation
        StartCoroutine(ChangeAnimatorProperty(agentAnimator, "isTalking", false));

        // Reset agent bubble
        agentBubble = null;
    }


    /// <summary>
    /// You can use a coroutine to executy code from asynchronus methods. This coroutine can be used to set properties of an animator. Call via StartCoroutine(AnimateAgentMouth(agentMouthAnimator, "isSpeaking", true));
    /// </summary>
    /// <param name="isSpeaking">A boolean value indicating whether the agent is speaking or not.</param>
    /// <returns>An IEnumerator object.</returns>
    public IEnumerator ChangeAnimatorProperty(Animator animator, string property, bool value)
    {
        yield return new WaitForEndOfFrame();
        animator.SetBool(property, value);
    }

}
