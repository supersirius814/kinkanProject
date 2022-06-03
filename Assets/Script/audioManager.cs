using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class audioManager : MonoBehaviour
{
    public AudioClip bgm_sound;
    public AudioClip miss_sound;
    public AudioClip success_sound;

    AudioSource m_MyAudioSource;

    public GameObject soundButton;

    // Start is called before the first frame update
    void Start()
    {
        m_MyAudioSource = GetComponent<AudioSource>();
        // play_bgm_play();

    }

    // Update is called once per frame
    void Update()
    {


    }

    public void sound_button_clicked()
    {
        if (m_MyAudioSource.isPlaying)
        {
            soundButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("img_ui/sound_off");
            play_bgm_stop();
        }
        else
        {
            soundButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("img_ui/sound_on");
            play_bgm_play();
        }

    }
    public void play_bgm_play()
    {
        if(m_MyAudioSource.clip != bgm_sound)m_MyAudioSource.clip = bgm_sound;
        if (m_MyAudioSource.isPlaying)
            return;
        m_MyAudioSource.clip = bgm_sound;
        m_MyAudioSource.loop = true;
        m_MyAudioSource.Play();// bgm_sound.
    }

    public void play_bgm_stop()
    {
        if (m_MyAudioSource.isPlaying)
            m_MyAudioSource.Stop();
    }

    public void play_success_play()
    {
        m_MyAudioSource.clip = success_sound;
        m_MyAudioSource.loop = false;
        m_MyAudioSource.Play();// bgm_sound.
    }
    public void play_miss_play()
    {
        m_MyAudioSource.clip = miss_sound;
        m_MyAudioSource.loop = false;
        m_MyAudioSource.Play();// bgm_sound.
    }
}
