using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControllerScript : MonoBehaviour
{

    private float time;
    public int BPM;

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

        if(temps - mesure*3>mesure*3+3) // mesure a 4 temps
        {
            mesure++; //rest les temps a 0 ?
            //indiquer aux instru de jouer pendant 1 mesure
        }

    }

}
