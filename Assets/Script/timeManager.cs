using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timeManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static float limit_timer = 2.5f;
    public static float curren_timer = 0.0f;
    public static bool timeStarted = false;
    static public float timer;
    static string time_pattern = "";
    public static Image statusFillImg;

    int clear_stage = 0;

    void Awake()
    {
        statusFillImg = GameObject.Find("in_gageImage").GetComponent<Image>();
    }
    void Start()
    {

    }

    public void setting_current_stage_time(string[] msg_params)
    {
        print("time..");
        statusFillImg.fillAmount = 1.0f;
        curren_timer = 0.0f;
        // this is pattern replace from 2x2 to 2-2
        string pattern = msg_params[0];
        string stage = msg_params[1];
        time_pattern = pattern.Replace('x', '-');
        print(time_pattern);
        limit_timer = float.Parse(DbManager.gettingLimitTime(time_pattern, stage));
        timeStarted = true;
    }

    public void setting_continue()
    {
        statusFillImg.fillAmount = 1.0f;
        curren_timer = 0.0f;
        timeStarted = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timeStarted)
        {
            curren_timer = curren_timer + Time.fixedDeltaTime;
            if (curren_timer < limit_timer + 0.2f)
            {
                statusFillImg.fillAmount = 1.0f - curren_timer / limit_timer;
            }
            else
            {
                timeStarted = false;
                SendMessage("setting_stage_vars_false");
                GameObject.Find("gamePanel").SetActive(false);
                StartCoroutine("delay");                
            }
        }
    }

    IEnumerator delay(){
        yield return new WaitForSeconds(2.0f);
        SendMessage("RequestBanner_250");
        panelManager.to_False();
    }
}
