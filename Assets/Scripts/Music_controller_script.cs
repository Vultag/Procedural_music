using F1yingBanana.SfizzUnity;
using Melanchall.DryWetMidi.MusicTheory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq; //juste pour check des notes -> a retirer

public class Music_controller_script : MonoBehaviour
{
    
    /*
    [field: SerializeField]
    public UnityEngine.Object Instrument
    {
        get; set;
    }
    */
    
    public SfizzPlayer Player;


    public int BPM;
    private float time_multiplicator;
    //private float max_time_frame = 0.25f; //division max possible des temps
    private float[] temps_division = { 0.25f, 0.5f, 0.75f, 1f };
    private int[] play_limit = { 28, 96 }; //renplacer "limites_de_jeu" avec �a 

    public bool activate_TEMPO;

    //private float timer = 0;

    private int note;
    private int previous_note;
    private int chord_note_1;
    private int chord_note_2;
    private int chord_note_3;
    private int chord_note_4;

    //pas utilise ?
    /*
    private int octave = 12;
    private int _do = 60;
    private int _re = 62;
    private int _mi = 64;
    private int _fa = 65;
    private int _sol = 67;
    private int _la = 69;
    private int _si = 71;
    */

    private int[] supposed_note = { 24,26,27,29,31,33,35,36,38,40,41,43,44,46,48,50,51,53,55,56,58,60, 62, 63, 65, 67, 68, 70, 72, 74, 75, 77, 79, 80, 82, 84, 86, 87, 89, 91, 92, 94 ,96 };

    private int[] Ionien = { 2, 2, 1, 2, 2, 2, 1 };
    private int[] Dorien = { 2, 1, 2, 2, 2, 1, 2 };
    private int[] Phrygien = { 1, 2, 2, 2, 1, 2, 2 };
    private int[] Lydien = { 2, 2, 2, 1, 2, 2, 1 };
    private int[] Mixolydien = { 2, 2, 1, 2, 2, 1, 2 };
    private int[] Eolien = { 2, 1, 2, 2, 1, 2, 2 };
    private int[] Locrien = { 1, 2, 2, 1, 2, 2, 2 };

    

    //public int[] interval_prop = { 40, 20, 12, 10, 8, 10 };
    List<float> interval_prob = new List<float>{ 0,46, 20, 12, 8, 8, 6 };

    //respectivement  - ronde, blanche, noire, croche, doublecroche
    List<float> rhythm_prob = new List<float> { 2, 5, 23, 40, 30};

    private string[] string_modes = { "Ionien", "Dorien", "Phrygien", "Lydien", "Mixolydien", "Eolien", "Locrien" };
    private int[][] modes;
    private int[] mode;
    //respectivement  - ronde, blanche, noire, croche, doublecroche
    private float[] rhythm_type = { 4, 2, 1, 0.5f, 0.25f };
    private float latest_rhythm;
    private float current_rhythm;
    private int current_mode_location = 0;
    private int displacement_note;
    private int displacement_tone;
    private int loop_int = 0;

    List<float> cumulativeProbability;


    // Start is called before the first frame update
    void Start()
    {

        latest_rhythm = 0; // A RENPLACER PAR LE RYTM DE DEBUT POUR LA MUSIQUE

        time_multiplicator = ((float)60 / BPM);


        
        string instrumentPath = Path.GetFullPath(
            Path.Combine(
                Application.streamingAssetsPath,
                "Instruments", "Marimba", "SalamanderGrandPianoV3Retuned.sfz"
            )
        );
        string path = Path.Combine(Application.streamingAssetsPath, "Instruments", "Piano", "SalamanderGrandPianoV3Retuned.sfz");

        bool success = Player.Sfizz.LoadFile(instrumentPath);
        if (!success)
        {
            Debug.LogWarning($"Sfz not found at the given path: {instrumentPath}, player will remain silent.");
        }
        
        




        modes = new int[][] { Ionien, Dorien, Phrygien, Lydien, Mixolydien, Eolien, Locrien };

        //int mode_target = Random.Range(0, modes.Length);
        int mode_target = 5;

        mode = modes[mode_target];
        print("Joue actuellement en " + string_modes[mode_target]);
        previous_note = 60;

        if (activate_TEMPO)
            StartCoroutine("TEMPO");

        //StartCoroutine("free_play");
        StartCoroutine(accompaniement());

        
    }

    // Update is called once per frame
    void Update()
    {


        //print(timer += Time.deltaTime);
       // Random.Range(-6, 7);

    }

    IEnumerator TEMPO()
    {
        while (true)
        { 
            yield return new WaitForSeconds(60f / BPM);

            Player.Sfizz.SendNoteOn(0, 50, 120);

            yield return null;
        }
    }

    

    IEnumerator free_play() //renomer Time_line?
    {
        while (true)
        {

            time_multiplicator = ((float)60 / BPM); //permet de changer dynamiquement dans l editor a enlever

            //division du temps sur la plus petite unite
            //yield return new WaitForSeconds((60f / BPM) * temps_division[0]);

            //Player.Sfizz.SendNoteOff(0, note, 120);

            //chaine de markov ici ?
            current_rhythm = rhythm_type[GetItemByProbability(rhythm_prob)];

            //print("play");
            StartCoroutine(Play_note());


            latest_rhythm = current_rhythm;

            //yield return new WaitForSeconds(2f);
            yield return new WaitForSeconds(current_rhythm*time_multiplicator);


            yield return null;
        }
    }


    IEnumerator accompaniement()
    {
        while (true)
        {

            time_multiplicator = ((float)60 / BPM); //permet de changer dynamiquement dans l editor a enlever

            //division du temps sur la plus petite unite
            //yield return new WaitForSeconds((60f / BPM) * temps_division[0]);

            //Player.Sfizz.SendNoteOff(0, note, 120);

            //chaine de markov ici ?
            //current_rhythm = rhythm_type[GetItemByProbability(rhythm_prob)];

            //print("play");
            StartCoroutine(Play_chord());


            //latest_rhythm = current_rhythm;

            yield return new WaitForSeconds(1f);
            //yield return new WaitForSeconds(current_rhythm*time_multiplicator);


            yield return null;
        }
    }


    IEnumerator Play_note()
    {

        displacement_note = GetItemByProbability(interval_prob);

        // la chance de monter ou descendre
        if (previous_note + displacement_note > 84 || previous_note - displacement_note < 45) // defini les limites de jeu
            displacement_note *= -System.Math.Sign(previous_note - 60);
        else
            displacement_note *= (Random.Range(0, 2) * 2 - 1);

        bool loopback_check = false;

        //print(displacement_note);

        if (displacement_note != 0)
        {

            //print("current_mode_location : " + current_mode_location);
            //print("displacement_note:" + displacement_note);

            for (int i = 1; i < MathF.Abs(displacement_note) + 1; i++) // faire variable pour la range : interval_length
            {

                int y = System.Math.Sign(displacement_note) * i - loop_int;


                if (System.Math.Sign(displacement_note) < 0)
                {

                    if (loop_int == 0)
                    {

                        // if going out of bound of the array
                        if ((current_mode_location + y) < 0)
                        {

                            loop_int = -(-y + 6); // +6 ?
                            y = 6;

                        }
                    }

                }
                if (System.Math.Sign(displacement_note) > 0)
                {

                    if (loop_int == 0) //bandage
                    {
                        // if going out of bound of the array
                        if (Mathf.Abs(current_mode_location + y) > mode.Length-1)
                        {
                            if (current_mode_location + y == 7) //TRUC A FAIRE ICI
                            {
                                loopback_check = true;
                                y = 6;
                            }
                            else
                            {
                                loop_int = y;
                                y = 0;
                            }


                        }
                    }

                }


                if (displacement_note > 0)
                {
                    if (loop_int == 0 && loopback_check == false)
                        displacement_tone += mode[current_mode_location + y - 1];
                    else
                    { 
                        displacement_tone += mode[y];
                        loopback_check = false;
                    }
                    
                }
                else
                {

                    if (loop_int == 0)
                        displacement_tone += mode[current_mode_location + y];//pas de +1 ?
                    else
                        displacement_tone += mode[y];

                }



                //reassign current mode location
                if (i + 1 >= MathF.Abs(displacement_note) + 1)
                {

                    if (displacement_note > 0)
                    {
                        if (current_mode_location + y < 7)
                        {
                            if (loop_int == 0)
                                current_mode_location = current_mode_location + y;
                            else
                                current_mode_location = y+1;
                        }
                        else
                            current_mode_location = y+1;//bandage //ICI

                    }
                    else
                    {
                        if (loop_int == 0)
                            current_mode_location = current_mode_location + y;
                        else
                            current_mode_location = y;
                    }
                    loop_int = 0;


                }


            }

            displacement_tone *= System.Math.Sign(displacement_note);

            note = previous_note + displacement_tone;


            displacement_tone = 0;

            //if (!supposed_note.Contains(note)) Debug.LogWarning("   NOTE DISSONANTE   :   "+note);

            Player.Sfizz.SendNoteOn(0, note, 120);

            previous_note = note;

            yield return new WaitForSeconds(0.5f);

            Player.Sfizz.SendNoteOff(0, note, 120);

        }
        else
        {
            print("aretirer si pas print");

        }

    } //ajouter la maniere dont est jouee la note en argument

    IEnumerator Play_chord()
    {
        //print(current_mode_location);
        displacement_note = GetItemByProbability(interval_prob);

        // la chance de monter ou descendre
        if (previous_note + displacement_note > 84 || previous_note - displacement_note < 45) // defini les limites de jeu
            displacement_note *= -System.Math.Sign(previous_note - 60);
        else
            displacement_note *= (Random.Range(0, 2) * 2 - 1);

        bool loopback_check = false;


        if (displacement_note != 0)
        {

            //print("current_mode_location : " + current_mode_location);
            //print("displacement_note:" + displacement_note);

            for (int i = 1; i < MathF.Abs(displacement_note) + 1; i++) // faire variable pour la range : interval_length
            {

                int y = System.Math.Sign(displacement_note) * i - loop_int;

                if (System.Math.Sign(displacement_note) < 0)
                {

                    if (loop_int == 0)
                    {

                        // if going out of bound of the array
                        if ((current_mode_location + y) < 0)
                        {

                            loop_int = -(-y + 6); // +6 ?
                            y = 6;

                        }
                    }

                }
                if (System.Math.Sign(displacement_note) > 0)
                {

                    if (loop_int == 0) //bandage
                    {
                        // if going out of bound of the array
                        if (Mathf.Abs(current_mode_location + y) > mode.Length - 1)
                        {
                            if (current_mode_location + y == 7) //TRUC A FAIRE ICI
                            {
                                loopback_check = true;
                                y = 6;
                            }
                            else
                            {
                                loop_int = y;
                                y = 0;
                            }


                        }
                    }

                }


                if (displacement_note > 0)
                {
                    if (loop_int == 0 && loopback_check == false)
                        displacement_tone += mode[current_mode_location + y - 1];
                    else
                    {
                        displacement_tone += mode[y];
                        loopback_check = false;
                    }

                }
                else
                {

                    if (loop_int == 0)
                        displacement_tone += mode[current_mode_location + y];//pas de +1 ?
                    else
                        displacement_tone += mode[y];

                }



                //reassign current mode location
                if (i + 1 >= MathF.Abs(displacement_note) + 1)
                {

                    if (displacement_note > 0)
                    {
                        if (current_mode_location + y < 7)
                        {
                            if (loop_int == 0)
                                current_mode_location = current_mode_location + y;
                            else
                                current_mode_location = y + 1;
                        }
                        else
                            current_mode_location = y + 1;//bandage //ICI

                    }
                    else
                    {
                        if (loop_int == 0)
                            current_mode_location = current_mode_location + y;
                        else
                            current_mode_location = y;
                    }
                    loop_int = 0;


                }


            }

            displacement_tone *= System.Math.Sign(displacement_note);

            note = previous_note + displacement_tone;

            displacement_tone = 0;

            
            // Chaine de markov ici pour la taille
            int chord_lenght = 2 + Random.Range(0, 6);
            int chord_number = 1 + Random.Range(0, 4); // plutot chord_length - 3 ?
            int free_space = 5; // sur ce schema  0;0;1;1;1;1;0;1
            int chords_assigned = 0;
            int loop_int_chord = 0;

            for (int i = 1; i < chord_lenght + 1; i++) // faire gaff a ce que les chords ne sortes pas de la limite de jeu
            {

                int y = i - loop_int_chord;


                if (loop_int_chord == 0)
                {
                    // if going out of bound of the array
                    if (current_mode_location + y > mode.Length + loop_int_chord)
                    {

                        loop_int_chord = y;
                        y = 0;

                    }
                }


                if (loop_int_chord == 0)
                    displacement_tone += mode[current_mode_location + y - 1];
                else
                    displacement_tone += mode[y];

                

                switch (i)
                {
                    case 2:
                        if (Random.Range(0, 2) > 0)
                        {
                            chord_note_1 = note + displacement_tone;
                            chords_assigned++;
                            free_space = 3;
                        }
                        else
                        {
                            free_space = 4;
                        }
                        break;
                    case 3:
                        if(chords_assigned == 0) // et free space -
                        {
                            chord_note_1 = note + displacement_tone;
                            chords_assigned++;
                            free_space = 2;
                        }
                        break;
                    case 4:
                        if (free_space == 3)
                        {
                            if (Random.Range(0, 2) > 0)
                            {
                                if (chords_assigned == 0)
                                {
                                    chord_note_1 = note + displacement_tone;
                                    chords_assigned++;
                                    free_space = 1;
                                }
                                else
                                {
                                    chord_note_2 = note + displacement_tone;
                                    chords_assigned++;
                                    free_space = 1;
                                }

                            }
                        }
                        break;
                    case 5:
                        if(free_space == 2 && chord_number - chords_assigned < 2)
                        {
                            if (Random.Range(0, 2) > 0)
                            {
                                if (chords_assigned == 0)
                                {
                                    chord_note_1 = note + displacement_tone;
                                    chords_assigned++;
                                    free_space = 1;
                                }
                                else if(chords_assigned == 1)
                                {
                                    chord_note_2 = note + displacement_tone;
                                    chords_assigned++;
                                    free_space = 1;
                                }
                                else if (chords_assigned == 2)
                                {
                                    chord_note_3 = note + displacement_tone;
                                    chords_assigned++;
                                    free_space = 1;
                                }

                            }
                            else
                            {
                                free_space = 1;
                            }
                        }
                        break;
                    case 7:
                        if (chords_assigned == 0)
                        {
                            chord_note_1 = note + displacement_tone;
                            chords_assigned++;
                            free_space = 0;
                        }
                        else if (chords_assigned == 1)
                        {
                            chord_note_2 = note + displacement_tone;
                            chords_assigned++;
                            free_space = 0;
                        }
                        else if (chords_assigned == 2)
                        {
                            chord_note_3 = note + displacement_tone;
                            chords_assigned++;
                            free_space = 0;
                        }
                        else
                        {
                            chord_note_4 = note + displacement_tone;
                            chords_assigned++;
                            free_space = 0;
                        }


                        break;
                    default:
                        //Debug.LogWarning("Some wrong...");
                        break;
                }

                /*
                for (int j = chord_number; j > 0; j --) //j = le nb restant de notes a attribuer
                {
                    
                    if (i > 1) //ameliorer pour traiter les intervales aussi
                    {
                        if( free_space - j > 0) // assigne tout si plus a tester


                        if (Random.Range(0, 2) > 0)
                        {
                            switch (-(j - chord_number))
                            {
                                case == 0:
                                    chord_note_1 = displacement_tone;
                                    chords_assigned++;
                                    free_space += -2;
                                    break;
                                case == 1:
                                    chord_note_2 = displacement_tone;
                                    chords_assigned++;
                                    break;
                                case == 2:
                                    chord_note_3 = displacement_tone;
                                    chords_assigned++;
                                    break;
                                case == 3:
                                    chord_note_4 = displacement_tone;
                                    chords_assigned++;
                                    break;
                                default:
                                    Debug.LogWarning("Some wrong...");
                                    break;
                            }

                            //chord_note_1 = ;
                        }
                        else
                        {
                                break;
                        }


                    }
                    else
                        j++;
                    

                }
                */

                /*
                // peu optimiser en testant la length plus haut
                if (chord_lenght == 4)
                { 
                    if (i == chord_lenght*0.5f +1)
                    {
                        chord_note_1 = note + displacement_tone;
                    }
                }
                else
                {
                    if (i == (chord_lenght+ ((Random.Range(0, 2)) * 2 - 1)) * 0.5f + 1)
                    {
                        chord_note_1 = note + displacement_tone;
                    }
                }

                if (i > chord_lenght-1)
                {
                    chord_note_2 = note + displacement_tone;
                    loop_int = 0;
                }
                */


            }

            


            displacement_tone = 0;

            //chord_note_1 = note

            Player.Sfizz.SendCC(0, 64, 127); //pedal

            Player.Sfizz.SendNoteOn(0, note, 120);

            //Quelle maniere la plus opi ?

            int chords_to_play = chords_assigned;


            while (chords_to_play > 0)
            {
                Player.Sfizz.SendNoteOn(0, chord_note_1, 120);
                chords_to_play--;
                Player.Sfizz.SendNoteOn(0, chord_note_2, 120);
                chords_to_play--;
                Player.Sfizz.SendNoteOn(0, chord_note_2, 120);
                chords_to_play--;
                Player.Sfizz.SendNoteOn(0, chord_note_2, 120);
                break;
            }
            chords_to_play = chords_assigned;

            previous_note = note;

            yield return new WaitForSeconds(0.5f);



            Player.Sfizz.SendNoteOff(0, note, 120);

            while (chords_to_play > 0)
            {
                Player.Sfizz.SendNoteOff(0, chord_note_1, 120);
                chords_to_play--;
                Player.Sfizz.SendNoteOff(0, chord_note_2, 120);
                chords_to_play--;
                Player.Sfizz.SendNoteOff(0, chord_note_3, 120);
                chords_to_play--;
                Player.Sfizz.SendNoteOff(0, chord_note_4, 120);
                break;
            }


        }
        else
        {
            print("si print pas a enlever");

        }

    } //ajouter la maniere dont est jouee la note en argument





    //this function creates the cumulative list
    bool MakeCumulativeProbability(List<float> probability)
    {
        float probabilitiesSum = 0;

        cumulativeProbability = new List<float>(); //reset the Array

        for (int i = 0; i < probability.Count; i++)
        {
            probabilitiesSum += probability[i]; //add the probability to the sum
            cumulativeProbability.Add(probabilitiesSum); //add the new sum to the list

            //All Probabilities need to be under 100% or it ll throw an exception
            if (probabilitiesSum > 100f)
            {
                Debug.LogError("Probabilities exceed 100%");
                return false;
            }
        }
        return true;
    }


    //This function is called with the Item probability array and it ll return the index of the item
    // for example the list can look like [10,25,30] so the first item has 10% of showing and next one has 25% and so on
    public int GetItemByProbability(List<float> probability) //[50,10,20,20]
    {
        //if your game will use this a lot of time it is best to build the arry just one time
        //and remove this function from here.
        if (!MakeCumulativeProbability(probability))
            return -1; //when it return false then the list excceded 100 in the last index

        float rnd = Random.Range(1, 101); //Get a random number between 0 and 100

        for (int i = 0; i < probability.Count; i++)
        {
            if (rnd <= cumulativeProbability[i]) //if the probility reach the correct sum
            {
                return i; //return the item index that has been chosen 
            }
        }
        return -1; //return -1 if some error happens
    }

    
}
