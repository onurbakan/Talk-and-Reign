using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Keep a queue of audio files to play when called
 * If user enters a Sound Zone, the audio should be placed in the queue to be played once the previous 
 * audio source finishes playing
 * */
public class AudioQueue : MonoBehaviour
{
    //Changed to list
    List<AudioSource> audioQueue = new List<AudioSource>(); //Queue of audio files, can grow as needed
    AudioSource currentAudioSource; //current audio source
    int index = 0;
    bool hasAudio;

    /*
     * If no audio is playing and there are audio source in the queue:
     * 1.) Get the currentAudioSource
     * 2.) Play the audio,
     * 3.) Get the next audioSource from the queue 
     *  4.) Play that audio
    */
    public void AudioPlay()
    {
        foreach (AudioSource sourceAudio in audioQueue)
        {
            if (!sourceAudio.isPlaying && hasAudioInQueue() == true)
            {
                sourceAudio.Play(); //play audio source
            }
        }
    }

    /* This checks to see if there is audio in the queue
     * 
     * */
    private bool hasAudioInQueue()
    {
        if (audioQueue.Count > 0)
        {
            hasAudio = true;
        }
        else
        {
            hasAudio = false;
        }

        return hasAudio;
    }

    /*
     * This is to communicate with the PlayAudio script
     * If a player runs into an audio zone, Play Audio will add the audio source to the queue and then start the play function
     * */
    public void AddAudioSourceToList(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            audioQueue.Add(audioSource);
            Debug.Log("Added Audio To Queue! Size: " + audioQueue.Count);
        }
        else
        {
            Debug.Log("Cannot add audio, no source found");
        }
    }

    public int getSize()
    {
        return audioQueue.Count;
    }
}