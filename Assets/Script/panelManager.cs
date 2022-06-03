using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Networking;

public class panelManager : MonoBehaviour
{
    public static GameObject slashPanel;

    public static GameObject startPanel;

    public static GameObject studyPanel;

    public static GameObject attPanel;

    public static GameObject gamePanel;

    public static GameObject truePanel;

    public static GameObject falsePanel;

    public static GameObject popupPanel;

    public static GameObject stagePanel;

    public static GameObject advertPanel;

    public static GameObject advertVideoPanel;

    public static GameObject retryPanel;

    public static GameObject exactPanel;

    public static GameObject resultPanel;

    public static GameObject offlinePanel;

    GameObject popButton;

    // Start is called before the first frame update
    void Start()
    {
        slashPanel = GameObject.Find("slashPanel");
        startPanel = GameObject.Find("startPanel");
        studyPanel = GameObject.Find("studyPanel");
        attPanel = GameObject.Find("attPanel");
        gamePanel = GameObject.Find("gamePanel");
        truePanel = GameObject.Find("truePanel");
        falsePanel = GameObject.Find("falsePanel");
        popupPanel = GameObject.Find("popupPanel");
        stagePanel = GameObject.Find("stagePanel");
        advertPanel = GameObject.Find("advertPanel");
        advertVideoPanel = GameObject.Find("advertVideoPanel");
        retryPanel = GameObject.Find("retryPanel");
        exactPanel = GameObject.Find("exactPanel");
        resultPanel = GameObject.Find("resultPanel");
        offlinePanel = GameObject.Find("offlinePanel");

        GameObject[] panels = GameObject.FindGameObjectsWithTag("panel");
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
        attPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void to_Splash()
    {
        Debug.Log("toslash");
        attPanel.SetActive(false);
        slashPanel.SetActive(true);
        StartCoroutine(to_Start());
    }

    IEnumerator to_Start()
    {
        yield return new WaitForSeconds(3.0f);
        Debug.Log("start");
        slashPanel.SetActive(false);
        startPanel.SetActive(true);
        SendMessage("play_bgm_play");
        SendMessage("RequestBanner_50");
    }

    public void to_Study(){
        startPanel.SetActive(false);
        if(statusManager.clearStage==0){
           studyPanel.SetActive(true);
        }else{
           gamePanel.SetActive(true);
           SendMessage("statusSetting");
        }       
    }

    public void to_Game()
    {
        Debug.Log("toGame");
        studyPanel.SetActive(false);
        gamePanel.SetActive(true);
        SendMessage("statusSetting");
    }

    public static void to_True()
    {
        Debug.Log("toTrue");

        // gamePanel.SetActive(false);
        truePanel.SetActive(true);
    }

    public static void to_False()
    {
        Debug.Log("toFalse");

        gamePanel.SetActive(false);
        falsePanel.SetActive(true);
    }

    public static void to_Stage()
    {
        truePanel.SetActive(false);
        stagePanel.SetActive(true);
    }

    public static void to_Game_upStage()
    {
        stagePanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    public void to_popup()
    {
        print("tt");

        truePanel.SetActive(false);
        popupPanel.SetActive(true);
        popButton = GameObject.Find("popupButton");
        popButton.SetActive(false);
        IEnumerator delay_coroutine = delay_true_panel(0.3f);
        StartCoroutine (delay_coroutine);
    }

    IEnumerator delay_true_panel(float delay)
    {
        yield return new WaitForSeconds(delay);
        SendMessage("popup_banner_250");
        yield return new WaitForSeconds(2.2f);
        popButton.SetActive(true);
    }

    public void to_Advert()
    {
        truePanel.SetActive(false);
        stagePanel.SetActive(true);
        // Debug.Log("toAdvert");
        // IEnumerator delay_coroutine = delay_advert_panel(3.0f);
        // StartCoroutine (delay_coroutine);
    }

    IEnumerator delay_advert_panel(float delay)
    {
        print("delay");
        yield return new WaitForSeconds(delay);
        stagePanel.SetActive(true);
        advertPanel.SetActive(false);
    }

    public void to_VideoAdvert()
    {
        stagePanel.SetActive(false);
        advertVideoPanel.SetActive(true);
        Debug.Log("toAdvert");
        IEnumerator delay_coroutine = delay_advert_video_panel(3.0f);
        StartCoroutine (delay_coroutine);
    }

    IEnumerator delay_advert_video_panel(float delay)
    {
        yield return new WaitForSeconds(delay);
        gamePanel.SetActive(true);
        advertVideoPanel.SetActive(false);
    }

    public void to_popup_false()
    {
        falsePanel.SetActive(false);
        popupPanel.SetActive(true);
        popButton = GameObject.Find("popupButton");
        popButton.SetActive(false);
        IEnumerator delay_coroutine = delay_false_panel(2.0f);
        StartCoroutine (delay_coroutine);
    }

    IEnumerator delay_false_panel(float delay)
    {
        yield return new WaitForSeconds(delay);
        popButton.SetActive(true);
    }

    public static void to_any_from_popup(bool is_result_exact)
    {
        if (is_result_exact)
        {
            truePanel.SetActive(true);
        }
        else
        {
            falsePanel.SetActive(true);
        }

        popupPanel.SetActive(false);

        //then, need animation of the advert_dowming...
        
    }

    public static void to_Retry()
    {
        Debug.Log("toHome");

        falsePanel.SetActive(false);
        retryPanel.SetActive(true);
    }

    public static void to_Home()
    {
        Debug.Log("toHome");

        retryPanel.SetActive(false);
        startPanel.SetActive(true);
    }

    public static void to_Game_retry()
    {
        Debug.Log("toGameRetry");

        retryPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    public static void to_Exact()
    {
        Debug.Log("toExact");

        truePanel.SetActive(false);
        exactPanel.SetActive(true);
    }

    public static void to_Result(int exact_rate)
    {
        Debug.Log("toResult");
        exactPanel.SetActive(false);
        resultPanel.SetActive(true);
        GameObject percentObj = GameObject.Find("percentLabel");
        percentObj.GetComponent<Text>().text = exact_rate.ToString() + "%";
    }

    public static void to_Home_from_Result()
    {
        Debug.Log("toHome_Result");
        resultPanel.SetActive(false);
        startPanel.SetActive(true);
    }

    public static void to_Offline()
    {
        Debug.Log("toOffline");
        truePanel.SetActive(false);
        offlinePanel.SetActive(true);
    }

    public static void to_True_Offline()
    {
        Debug.Log("fromOffline");

        offlinePanel.SetActive(false);
        truePanel.SetActive(true);
    }
}
