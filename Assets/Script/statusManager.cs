using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class statusManager : MonoBehaviour
{
    public string status;

    public string pattern;

    public static string stage;

    public static int totalStage;

    public static int clearStage;

    public static int tapCount;

    public static int miss;

    public static int row_count;

    public static int col_count;

    public static question[,] current_tasks;

    public static int exact_rate = 0;

    public static string question_1_task_hanzi;

    public static string question_2_task_hanzi;

    public static string question_1_task_reading;

    public static string question_2_task_reading;

    public static bool is_current_result_exact;

    public GameObject textImage_1;

    public GameObject textImage_2;

    public RectTransform ParentPanel;

    public GameObject prefabButton;

    void Awake()
    {
        textImage_1 = GameObject.Find("textImage_1");
        textImage_2 = GameObject.Find("textImage_2");
        prefabButton = Resources.Load<GameObject>("Button");
        ParentPanel =
            GameObject.Find("gamePanel").GetComponent<RectTransform>() as
            RectTransform;
    }

    // Start is called before the first frame update
    void Start()
    {
        status = DbManager.beginStatus();
        string[] status_params = status.Split(":");
        stage = status_params[0];
        totalStage = System.Convert.ToInt32(status_params[1]);
        clearStage = System.Convert.ToInt32(status_params[2]);
        miss = System.Convert.ToInt32(status_params[3]);
    }

    public void statusSetting()
    {
        string pattern_stage =
            DbManager.getCurrentPatten((clearStage + 1).ToString());
        pattern = pattern_stage.Split(":")[0];
        stage = pattern_stage.Split(":")[1];

        row_count = System.Convert.ToInt32(pattern.Split("x")[0]);
        col_count = System.Convert.ToInt32(pattern.Split("x")[1]);

        current_tasks = DbManager.gettingTasks(row_count, col_count);

        this.viewing();
        string[] message_params;
        message_params = new string[2] { pattern, stage };

        // message_params[0] = pattern;
        // message_params[1] = stage;
        print("p:" + pattern);
        print("s:" + stage);

        CameraControl.is_game_playing = true;        

        SendMessage("start_stage");
        SendMessage("setting_current_stage_time", message_params);
        SendMessage("camera_game_in_new_playing");
        SendMessage("RequestBanner_50");
        SendMessage("play_bgm_play");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void info_button_clicked(){
        Application.OpenURL("https://sp.i-appli.net/media/policy.html");
    }
    public void viewing()
    {
        GameObject[] pre_buttons = GameObject.FindGameObjectsWithTag("hanzi");

        foreach (GameObject pre_btn in pre_buttons)
        {
            Destroy (pre_btn);
        }
        print("stage=>" + stage);
        if (stage == "1")
        {
            textImage_1.SetActive(true);
            textImage_2.SetActive(false);
            GameObject.Find("taskLabel").GetComponent<Text>().text =
                question_1_task_hanzi;
        }
        else
        {
            textImage_1.SetActive(false);
            textImage_2.SetActive(true);
        }

        // question_1_task_hanzi = current_tasks[new System.Random().Next(0, row_count), new System.Random().Next(0, col_count)].hanzi;
        print("hanzi:" + question_1_task_hanzi);

        float start_point_x = -200 * (col_count - 1) / 2.0f;
        float start_point_y = row_count > 3 ? 110.0f : -10;
        for (int i = 0; i < row_count; i++)
        {
            for (int j = 0; j < col_count; j++)
            {
                GameObject taskButton = Instantiate(prefabButton) as GameObject;
                taskButton.name =
                    "taskButton_" + i.ToString() + "_" + j.ToString();
                taskButton.transform.SetParent(ParentPanel, true);
                taskButton.transform.localScale = new Vector3(1, 1, 1);

                print("i" + i + "j" + j);

                taskButton.GetComponent<RectTransform>().localPosition =
                    new Vector3(start_point_x + 200 * j,
                        start_point_y - 200 * i,
                        0);

                Button tempButton = taskButton.GetComponent<Button>();
                Image tempImage = taskButton.GetComponent<Image>();
                string image_resource_path =
                    "image-question/" + current_tasks[i, j].q_name;
                image_resource_path = image_resource_path.Split(".")[0];

                // print(image_resource_path);
                taskButton.GetComponent<Image>().sprite =
                    Resources.Load<Sprite>(image_resource_path);

                question btn_quesion = taskButton.GetComponent<question>();

                taskButton.GetComponent<question>().hanzi =
                    current_tasks[i, j].hanzi;
                taskButton.GetComponent<question>().row =
                    current_tasks[i, j].row;
                taskButton.GetComponent<question>().column =
                    current_tasks[i, j].column;
                taskButton.GetComponent<question>().visible =
                    current_tasks[i, j].visible;
                taskButton.GetComponent<question>().q_name =
                    current_tasks[i, j].q_name;
                taskButton.GetComponent<question>().reading =
                    current_tasks[i, j].reading;

                // int tempInt = i;
                taskButton
                    .GetComponent<Button>()
                    .onClick
                    .AddListener(() => ButtonClicked(taskButton));
            }
        }
    }

    IEnumerator to_Delay(GameObject taskButton)
    {
        print("delay");

       

        timeManager.timeStarted = false;
        CameraControl.is_game_playing = false;

        taskButton.SetActive(false);

        print("clicked..." + taskButton.GetComponent<question>().hanzi);
        string button_name = taskButton.name;
        int button_row = System.Convert.ToInt32(button_name.Split("_")[1]);
        int button_col = System.Convert.ToInt32(button_name.Split("_")[2]);
        string clicked_hanzi = taskButton.GetComponent<question>().hanzi;

        current_tasks[button_row, button_col].visible = false;

        GameObject.Find("gamePanel").SetActive(false);

       
        if (stage == "1")
        {
            if (clicked_hanzi == question_1_task_hanzi)
            {
                SendMessage("moving");
                yield return new WaitForSeconds(1.0f);
                SendMessage("RequestBanner_250");
                panelManager.to_True();
                GameObject.Find("resultLabel").GetComponent<Text>().text =
                    question_1_task_hanzi + ": " + question_1_task_reading;
                setting_stage_vars_true();
            }
            else
            {
                setting_stage_vars_false();
                
                yield return new WaitForSeconds(2.0f);
                SendMessage("RequestBanner_250");
                
                panelManager.to_False();
                GameObject.Find("resultFalseLabel").GetComponent<Text>().text =
                    clicked_hanzi +
                    ": " +
                    taskButton.GetComponent<question>().reading;
                
            }
        }
        else
        {
            if (clicked_hanzi == question_2_task_hanzi)
            {
                SendMessage("moving");
                yield return new WaitForSeconds(1.0f);
                panelManager.to_True();
                SendMessage("RequestBanner_250");
                GameObject.Find("resultLabel").GetComponent<Text>().text =
                    question_2_task_hanzi + ": " + question_2_task_reading;
                setting_stage_vars_true();
            }
            else
            {
                setting_stage_vars_false();
                 
                yield return new WaitForSeconds(2.0f);
                SendMessage("RequestBanner_250");
               
                panelManager.to_False();
                GameObject.Find("resultFalseLabel").GetComponent<Text>().text =
                    clicked_hanzi +
                    ": " +
                    taskButton.GetComponent<question>().reading;
                
            }
        }
        

        print("stage" + stage);
    }

    public void ButtonClicked(GameObject taskButton)
    {
        if (!taskButton)
        {
            print("no exist");
            return;
        }
        StartCoroutine(to_Delay(taskButton));
    }

    public void setting_stage_vars_true()
    {
        print("true" + is_current_result_exact.ToString());
        totalStage++;
        clearStage++;
        tapCount++;

        // animation will be enabled here...
        CameraControl.fettis.SetActive(true);
        GemControl.is_true_staying=true;
        SendMessage("play_success_play");

        is_current_result_exact = true;
        if (tapCount % 7 == 0) advertManager.tap_advert = true;
        if (clearStage % 6 == 0) advertManager.clear_advert = true;
        if (totalStage % 13 == 0) advertManager.total_advert = true;
    }

    public void setting_stage_vars_false()
    {
        print("false" + is_current_result_exact.ToString());
        SendMessage("play_miss_play");
        GemControl.game_stage_failed();

        tapCount++;
        miss++;
        is_current_result_exact = false;
        if (tapCount % 7 == 0) advertManager.tap_advert = true;
    }

    public void true_button_clicked()
    {
        print("true_button_clicked");
        CameraControl.fettis.SetActive(false);
        if (clearStage == 50)
        {
            panelManager.to_Exact();
            SendMessage("RequestBanner_50");
            return;
        }

        bool offline = false;

        if (Application.internetReachability == NetworkReachability.NotReachable
        )
        {
            print("Error. Check internet connection!");
            offline = true;
        }

        if (offline)
        {
            // panelManager.to_Offline();
        }

        if (advertManager.tap_advert)
        {
            advertManager.tap_advert = false;
            SendMessage("to_popup");
            return;
        }
        if (advertManager.clear_advert)
        {
            advertManager.clear_advert = false;
            SendMessage("RequestInterstitial");
            SendMessage("to_Advert");
        }

        panelManager.to_Stage();
        GameObject.Find("stageLabel").GetComponent<Text>().text =
            clearStage.ToString() + "/50クリア";
    }

    public void stage_button_clicked()
    {
        panelManager.to_Game_upStage();
        this.to_next_stage();
    }

    public void false_button_clicked()
    {
        GemControl.gem_stop_gravity();
        if (advertManager.total_advert)
        {
            advertManager.total_advert = false;
            SendMessage("RequestInterstitial");
            SendMessage("to_popup_false");
        }
        else
        {
            panelManager.to_Retry();
        }
    }

    public void popup_button_clicked()
    {
        print("popup_button_clicked");
        panelManager.to_any_from_popup (is_current_result_exact);
        SendMessage("true_false_adverting");
    }

    public void to_next_stage()
    {
        if (advertManager.total_advert)
        {
            advertManager.total_advert = false;
            SendMessage("RequestRewarded");
        }
        else
        {
            next_stage_starting();
        }
    }

    public void next_stage_starting()
    {
        stage = DbManager.gettingStage(clearStage + 1);
        DbManager.settingStage (stage, totalStage, clearStage, miss);
        panelManager.to_Game_upStage();
        this.statusSetting();
    }

    public void home_button_clicked()
    {
        panelManager.to_Home();
        SendMessage("play_bgm_play");
    }

    public void retry_button_clicked()
    {
        panelManager.to_Game_retry();
        SendMessage("setting_continue");
        SendMessage("start_stage");
        SendMessage("camera_game_in_new_playing");
        SendMessage("RequestBanner_50");
        SendMessage("play_bgm_play");
    }

    public void exact_button_clicked()
    {
        exact_rate = 100 - miss * 100 / 50;
        if (exact_rate < 0) exact_rate = 0;
        panelManager.to_Result (exact_rate);
    }

    public void result_home_button_clicked()
    {
        panelManager.to_Home_from_Result();
        DbManager.settingBeginStage("1", 0, 0);
        stage = "1";
        clearStage = 0;
        miss = 0;
    }

    public void offline_button_clicked()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable
        )
        {
            print("Error. Check internet connection!");
            return;
        }
        panelManager.to_True_Offline();
    }
}
