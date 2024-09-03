using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeVolumeLevels : MonoBehaviour
{
    public Slider thisSlider;
    public float masterVolume;
    public float musicVolume;
    public float SFXVolume;
    public float sliderValue;
    float value;
    int type = 1;
    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.GetRTPCValue("MasterVolume", gameObject, 0, out value, ref type);
        thisSlider.value = value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSpecificVolume(string whichValue)
    {
       sliderValue = thisSlider.value;

        if(whichValue.Contains("volume"))
        {
            masterVolume = this.thisSlider.value;
            AkSoundEngine.SetRTPCValue("MasterVolume", masterVolume);            
        }

        if (whichValue.Contains("MUSIC"))
        {
            musicVolume = this.thisSlider.value;
            AkSoundEngine.SetRTPCValue("MusicVolume", musicVolume);
        }

        if (whichValue.Contains("SFX"))
        {
            SFXVolume = this.thisSlider.value;
            AkSoundEngine.SetRTPCValue("SFXVolume", SFXVolume);
        }
    }
}
