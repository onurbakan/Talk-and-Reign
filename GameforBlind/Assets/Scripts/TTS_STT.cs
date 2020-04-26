/**
* (C) Copyright IBM Corp. 2015, 2020.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/
#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using IBM.Watson.SpeechToText.V1;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.DataTypes;

using System;
using IBM.Watson.TextToSpeech.V1;

namespace IBM.Watsson.Examples
{
    public class TTS_STT : MonoBehaviour
    {
        #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
        // TTS
        [Space(10)]
        [Tooltip("The service URL (optional). This defaults to \"https://api.eu-de.text-to-speech.watson.cloud.ibm.com/instances/dd633c37-a884-4dc0-b3b9-1fc3d289b56e\"")]
        [SerializeField]
        private string _TTSUrl;
        [Header("IAM Authentication")]
        [Tooltip("The IAM apikey.")]
        [SerializeField]
        private string _TTSApikey;
        // STT
        [Space(10)]
        [Tooltip("The service URL (optional). This defaults to \"https://stream.watsonplatform.net/speech-to-text/api\"")]
        [SerializeField]
        private string _STTUrl;
        [Tooltip("Text field to display the results of streaming.")]
        public Text ResultsSTTField;
        [Header("IAM Authentication")]
        [Tooltip("The IAM apikey.")]
        [SerializeField]
        private string _STTApikey;


        [Header("Parameters")]
        // https://www.ibm.com/watson/developercloud/speech-to-text/api/v1/curl.html?curl#get-model
        [Tooltip("The Model to use. This defaults to en-US_BroadbandModel")]
        [SerializeField]
        private string _recognizeModel;
        #endregion


        private int _recordingRoutine = 0;
        private string _microphoneID = null;
        private AudioClip _recording = null;
        private int _recordingBufferSize = 1;
        private int _recordingHZ = 22050;
        private string mytext = "";
        private string PlayerName = "";




        private SpeechToTextService _STTservice;

        // TTS
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


        /// <summary>
        /// //Game Waited Word Area
        /// </summary>
        [SerializeField]
        private WaitedWordList[] waitedWordList;


        void Start()
        {

            audioStatus = ProcessingStatus.Idle;
            LogSystem.InstallDefaultReactors();
            Runnable.Run(CreateService());

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
            // textspeech("My fourth sentences");
            if (audioQueue.Count > 0 && !outputAudioSource.isPlaying)
            {
                PlayClip(audioQueue.Dequeue());
            }


            if (Input.GetKey(KeyCode.S))
            {
                StartRecording();
            }
            else
            {
                StopRecording();
            }


        }

        private IEnumerator CreateService()
        {
            if (string.IsNullOrEmpty(_STTApikey))
            {
                throw new IBMException("Plesae provide IAM ApiKey for the service.");
            }

            IamAuthenticator TTSauthenticator = new IamAuthenticator(apikey: _STTApikey);

            //  Wait for tokendata
            while (!TTSauthenticator.CanAuthenticate())
                yield return null;

            _STTservice = new SpeechToTextService(TTSauthenticator);
            if (!string.IsNullOrEmpty(_STTUrl))
            {
                _STTservice.SetServiceUrl(_STTUrl);
            }
            _STTservice.StreamMultipart = true;

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

            Active = true;
            StartCheck();


            StartRecording();
        }


        public void StartCheck()
        {
            if (PlayerPrefs.GetString("playerName") != "")
            {
                Debug.Log(PlayerPrefs.GetString("playerName"));

                PlayerName = PlayerPrefs.GetString("playerName");
                textspeech("Welcome " + PlayerName + " to our game!");
            }
            else
            {
                textspeech("Hello welcome to the my game. My name is Olivia. What is your name ?");

            }
        }




        private void textspeech(string textt)
        {
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
                text: textt,
                voice: "en-" + voice,
                accept: "audio/wav"
            );
        }


        public void AddTextToQueue(string text)
        {
            Debug.Log("AddTextToQueue: " + text);
            if (!string.IsNullOrEmpty(text))
            {
                textQueue.Enqueue(text);
                //inputField.text = string.Empty;
            }
        }

        public bool IsFinished()
        {
            return !outputAudioSource.isPlaying && audioQueue.Count < 1 && textQueue.Count < 1;
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

        public bool Active {
            get { return _STTservice.IsListening; }
            set {
                if (value && !_STTservice.IsListening)
                {
                    _STTservice.RecognizeModel = (string.IsNullOrEmpty(_recognizeModel) ? "en-US_BroadbandModel" : _recognizeModel);
                    _STTservice.DetectSilence = true;
                    _STTservice.EnableWordConfidence = true;
                    _STTservice.EnableTimestamps = true;
                    _STTservice.SilenceThreshold = 0.01f;
                    _STTservice.MaxAlternatives = 1;
                    _STTservice.EnableInterimResults = true;
                    _STTservice.OnError = OnError;
                    _STTservice.InactivityTimeout = -1;
                    _STTservice.ProfanityFilter = false;
                    _STTservice.SmartFormatting = true;
                    _STTservice.SpeakerLabels = false;
                    _STTservice.WordAlternativesThreshold = null;
                    _STTservice.EndOfPhraseSilenceTime = null;
                    _STTservice.StartListening(OnRecognize, OnRecognizeSpeaker);
                }
                else if (!value && _STTservice.IsListening)
                {
                    _STTservice.StopListening();
                }
            }
        }

        private void StartRecording()
        {
            if (_recordingRoutine == 0)
            {
                UnityObjectUtil.StartDestroyQueue();
                _recordingRoutine = Runnable.Run(RecordingHandler());
            }
        }

        private void StopRecording()
        {
            if (_recordingRoutine != 0)
            {
                Microphone.End(_microphoneID);
                Runnable.Stop(_recordingRoutine);
                _recordingRoutine = 0;
            }
        }

        private void OnError(string error)
        {
            Active = false;

            Log.Debug("ExampleStreaming.OnError()", "Error! {0}", error);
        }

        private IEnumerator RecordingHandler()
        {
            Log.Debug("ExampleStreaming.RecordingHandler()", "devices: {0}", Microphone.devices);
            _recording = Microphone.Start(_microphoneID, true, _recordingBufferSize, _recordingHZ);
            yield return null;      // let _recordingRoutine get set..

            if (_recording == null)
            {
                StopRecording();
                yield break;
            }

            bool bFirstBlock = true;
            int midPoint = _recording.samples / 2;
            float[] samples = null;

            while (_recordingRoutine != 0 && _recording != null)
            {
                int writePos = Microphone.GetPosition(_microphoneID);
                if (writePos > _recording.samples || !Microphone.IsRecording(_microphoneID))
                {
                    Log.Error("ExampleStreaming.RecordingHandler()", "Microphone disconnected.");

                    StopRecording();
                    yield break;
                }

                if ((bFirstBlock && writePos >= midPoint)
                  || (!bFirstBlock && writePos < midPoint))
                {
                    // front block is recorded, make a RecordClip and pass it onto our callback.
                    samples = new float[midPoint];
                    _recording.GetData(samples, bFirstBlock ? 0 : midPoint);

                    AudioData record = new AudioData();
                    record.MaxLevel = Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples));
                    record.Clip = AudioClip.Create("Recording", midPoint, _recording.channels, _recordingHZ, false);
                    record.Clip.SetData(samples, 0);

                    _STTservice.OnListen(record);

                    bFirstBlock = !bFirstBlock;
                }
                else
                {
                    // calculate the number of samples remaining until we ready for a block of audio, 
                    // and wait that amount of time it will take to record.
                    int remaining = bFirstBlock ? (midPoint - writePos) : (_recording.samples - writePos);
                    float timeRemaining = (float)remaining / (float)_recordingHZ;

                    yield return new WaitForSeconds(timeRemaining);
                }
            }
            yield break;
        }

        private void OnRecognize(SpeechRecognitionEvent result)
        {
            if (result != null && result.results.Length > 0)
            {
                foreach (var res in result.results)
                {
                    foreach (var alt in res.alternatives)
                    {
                        string text = string.Format("{0} ({1}, {2:0.00})\n", alt.transcript, res.final ? "Final" : "Interim", alt.confidence);
                        Log.Debug("ExampleStreaming.OnRecognize()", text);
                        // Konustugumuz kelimeler text'e geliyor.
                        ResultsSTTField.text = text;

                        //  DENEME BAŞ  //
                        if (alt.transcript.Contains("blue") && ResultsSTTField.text.Contains("Final")) // needs to be final or ECHO happens
                        {
                            textspeech("Thank you blue");
                        }


                        for (int i = 0; i < waitedWordList[0].waitedWords.Length; i++)
                        {
                            if (alt.transcript.Contains(waitedWordList[0].waitedWords[i])) // needs to be final or ECHO happens
                            {
                                textspeech(waitedWordList[0].output[i]);
                            }
                        }






                    }

                    if (res.keywords_result != null && res.keywords_result.keyword != null)
                    {
                        foreach (var keyword in res.keywords_result.keyword)
                        {
                            Log.Debug("ExampleStreaming.OnRecognize()", "keyword: {0}, confidence: {1}, start time: {2}, end time: {3}", keyword.normalized_text, keyword.confidence, keyword.start_time, keyword.end_time);
                        }
                    }

                    if (res.word_alternatives != null)
                    {
                        foreach (var wordAlternative in res.word_alternatives)
                        {
                            Log.Debug("ExampleStreaming.OnRecognize()", "Word alternatives found. Start time: {0} | EndTime: {1}", wordAlternative.start_time, wordAlternative.end_time);
                            foreach (var alternative in wordAlternative.alternatives)
                                Log.Debug("ExampleStreaming.OnRecognize()", "\t word: {0} | confidence: {1}", alternative.word, alternative.confidence);
                        }
                    }
                }
            }
        }

        private void OnRecognizeSpeaker(SpeakerRecognitionEvent result)
        {
            if (result != null)
            {
                foreach (SpeakerLabelsResult labelResult in result.speaker_labels)
                {
                    Log.Debug("ExampleStreaming.OnRecognizeSpeaker()", string.Format("speaker result: {0} | confidence: {3} | from: {1} | to: {2}", labelResult.speaker, labelResult.from, labelResult.to, labelResult.confidence));
                }
            }
        }












    }
}
