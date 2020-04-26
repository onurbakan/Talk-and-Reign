using System;
using System.Collections;
using System.Collections.Generic;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.TextToSpeech.V1;

using UnityEngine;
using UnityEngine.UI;

public class TTSExample : MonoBehaviour
{
    [Space(10)]
    [Tooltip("The service URL (optional). This defaults to \"https://api.eu-de.text-to-speech.watson.cloud.ibm.com/instances/dd633c37-a884-4dc0-b3b9-1fc3d289b56e\"")]
    [SerializeField]
    private string _TTSUrl;
    [Header("IAM Authentication")]
    [Tooltip("The IAM apikey.")]
    [SerializeField]
    private string _TTSApikey;

    public enum IBM_voices
    {
        GB_KateV3Voice,
        US_AllisonVoice,
        US_AllisonV3Voice,
        US_EmilyV3Voice,
        US_HenryV3Voice,
        US_KevinV3Voice,
        US_LisaVoice,
        US_LisaV3Voice,
        US_MichaelVoice,
        US_MichaelV3Voice,
        US_OliviaV3Voice
    }
    [SerializeField]
    private IBM_voices voice = IBM_voices.US_MichaelV3Voice;

    private TextToSpeechService tts_service; // IBM Watson text to speech service
    private IamAuthenticator tts_authenticator; // IBM Watson text to speech authenticator

    //Keep track of when the processing of text to speech is complete.
    //I don't want processing of text to speech to start until the previous 
    //text is processed. Otherwise, short text samples get processed faster
    //than longer samples and may be placed on the queue out of order.
    //It seems that AudioSource.isPlaying doesn't work reliably.
    public enum ProcessingStatus { Processing, Idle };
    private ProcessingStatus audioStatus;

    [SerializeField]
    private AudioSource outputAudioSource; // The AudioSource for speaking

    // A queue for storing the entered texts for conversion to speech audio files
    private Queue<string> textQueue = new Queue<string>();
    // A queue for storing the speech AudioClips for playing
    private Queue<AudioClip> audioQueue = new Queue<AudioClip>();

    //public string[] textArray;

    public enum InputFieldTrigger { onValueChanged, onEndEdit };

    //[SerializeField]
    private InputField inputField;




    // Start is called before the first frame update
    void Start()
    {
        audioStatus = ProcessingStatus.Idle;
        LogSystem.InstallDefaultReactors();
        Runnable.Run(CreateService());

        // Get or make the AudioSource for playing the speech
        if (outputAudioSource == null)
        {
            gameObject.AddComponent<AudioSource>();
            outputAudioSource = gameObject.GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // If no AudioClip is playing, remove the next clip from the
        // queue and play it.

        if (audioQueue.Count > 0 && !outputAudioSource.isPlaying)
        {
            PlayClip(audioQueue.Dequeue());
        }

    }

    public IEnumerator CreateService()
    {
        //  Create credential and instantiate service
        tts_authenticator = new IamAuthenticator(apikey: _TTSApikey);

        //  Wait for tokendata
        while (!tts_authenticator.CanAuthenticate())
            yield return null;

        tts_service = new TextToSpeechService(tts_authenticator);
        if (!string.IsNullOrEmpty(_TTSUrl))
        {
            tts_service.SetServiceUrl(_TTSUrl);
        }

        // Yazilan string baslangicta asistan tarafından söylenir
        string nextText2 = "Hello welcome to the my game. My name is Olivia ";
        byte[] synthesizeResponse = null;
        AudioClip clip = null;
        tts_service.Synthesize(
            callback: (DetailedResponse<byte[]> response, IBMError error) =>
            {
                synthesizeResponse = response.Result;
                clip = WaveFile.ParseWAV("myClip", synthesizeResponse);

                //Place the new clip into the audio queue.
                audioQueue.Enqueue(clip);
            },
            text: nextText2,
            voice: "en-" + voice,
            accept: "audio/wav"
        );
    }

    private void PlayClip(AudioClip clip)
    {
        if (Application.isPlaying && clip != null)
        {
            outputAudioSource.spatialBlend = 0.0f;
            outputAudioSource.loop = false;
            outputAudioSource.clip = clip;
            outputAudioSource.Play();
        }
    }

}
