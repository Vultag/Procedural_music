using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

public class TimeControllerScript : MonoBehaviour
{

    [Range(30f, 250f)]
    public int beatsPerMinute; // pas possibiliter de changer le temps dynamique temps , ajouter ChangeBPM() ?
    [Range(1, 9)]
    public int beatsPerMeasure = 4; // une mesure a quatre temps.

    private int measureIdx;
    private int beatIdx {
        get => measureIdx * beatsPerMeasure + beatAccumulator;
    }

    private float timeAccumulator;
    private int beatAccumulator;

    private float beatLength {
        get => 60.0f / beatsPerMinute;
    }
    private int instrument_number;


    // Start is called before the first frame update
    void Start()
    {
        timeAccumulator = 0;
        beatAccumulator = 0;
        measureIdx = 0;

        Assert.IsTrue(beatsPerMeasure > 0);
        Assert.IsTrue(beatsPerMinute > 0);
    }

    void Update()
    {
        timeAccumulator += Time.deltaTime; 
        int numberNewBeats = Mathf.FloorToInt(timeAccumulator / beatLength);
        if (numberNewBeats > 0) {
            timeAccumulator = timeAccumulator % beatLength;
            beatAccumulator += numberNewBeats;
            Debug.Log($@"New Beat: {beatIdx}");
        }
        int numberNewMeasures = beatAccumulator / beatsPerMeasure;
        if(numberNewMeasures > 0)
        {
            beatAccumulator = beatAccumulator % beatsPerMeasure;
            measureIdx += numberNewMeasures;
            //respectivement : 0 = accompagnement ; 1 = free_play
            int play_styles_available = 1; // augmenter quand je rajoute des playstyle

            Debug.Log($@"New Measure: {measureIdx} - {beatIdx}");
            // print("nouvelle mesure");
            //indiquer aux instru de jouer pendant 1 mesure
            for(int i = 0; i < this.transform.childCount; i++)
            { 
                if (play_styles_available == 1)
                {
                    this.transform.GetChild(i).gameObject.GetComponent<MusicControllerScript>().StartCoroutine("free_play");
                    play_styles_available--;
                }
                else if(play_styles_available == 0)
                {
                    this.transform.GetChild(i).gameObject.GetComponent<MusicControllerScript>().StartCoroutine("accompagnement");
                }
            }
        }
    }

}
