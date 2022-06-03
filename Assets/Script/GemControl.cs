using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemControl : MonoBehaviour
{
    public static GameObject[] gems;
    public static List<Vector3> gems_positions_begin;

    public static bool is_true_staying;
    // Start is called before the first frame update
    void Start()
    {
        gems = GameObject.FindGameObjectsWithTag("gem");
        gems_positions_begin = new List<Vector3>();
        foreach (GameObject gem in gems)
        {
            gems_positions_begin.Add(gem.transform.localPosition);
        }
        is_true_staying = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (is_true_staying)
        {
            
            foreach (GameObject gem in gems)
            {               
                gem.GetComponent<Transform>().localEulerAngles=new Vector3(0,60f*Time.time,0);
            }
        }
    }

    public void start_stage()
    {
        print("stage--start");
        is_true_staying = false;
        for (int i = 0; i < gems.Length; i++)
        {
            gems[i].GetComponent<Rigidbody>().useGravity = false;
            gems[i].transform.localPosition = gems_positions_begin[i];
            gems[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            gems[i].GetComponent<Rigidbody>().mass =0.3f;
        }
    }

    public static void game_stage_failed()
    {
       
        foreach (GameObject gem in gems)
        {
            gem.GetComponent<Rigidbody>().useGravity = true;
            gem.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public static void gem_stop_gravity(){
        foreach (GameObject gem in gems)
        {
            gem.GetComponent<Rigidbody>().mass =1122.5f;
            gem.GetComponent<Rigidbody>().useGravity = false;
            gem.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
