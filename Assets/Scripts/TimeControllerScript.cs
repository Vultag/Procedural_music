using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControllerScript : MonoBehaviour
{

    private float time;
    public int BPM; // pas possibiliter de changer le temps dynamique temps , ajouter ChangeBPM() ?

    private int temps;
    private float tempo;
    private int mesure;

    private int instrument_number;


    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        temps = 0;
        mesure = 0;
        tempo = 60 / BPM;
    }

    void Update()
    {

        time += Time.deltaTime; 

        if(time - temps*tempo>tempo)
        {
            temps++;
        }

        if(temps - mesure*3>mesure*3) // mesure a 4 temps
        {


            //respectivement : 0 = accompagnement ; 1 = free_play
            int play_styles_available = 1; // augmenter quand je rajoute des playstyle

            print("nouvelle mesure");
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


            mesure++; //rest les temps a 0 ?
        }

    }

}
