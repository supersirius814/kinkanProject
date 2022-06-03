using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using System;


public class DbManager : MonoBehaviour
{
    public static SqliteConnection con_db;
    public static SqliteCommand cmd_db;
    public static string path;
    public static string question_1_hanzi;
    public static string question_2_hanzi;
    private void Awake()
    {
        Connection();
        SetDB();
        statusManager.tapCount = 6;
        statusManager.miss = 0;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void Connection()
    {
        try
        {
            // path = Application.persistentDataPath + "/unity.s3db";
            // if (!File.Exists(path))
            // {
            //     // If not found on android will create Tables and database

            //     // Debug.LogWarning("File \"" + filepath + "\" does not exist. Attempting to create from \"" +
            //     //                  Application.dataPath + "!/assets/unity.s3db");

            //     // UNITY_ANDROID
            //     WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/unity.s3db");
            //     while (!loadDB.isDone) { }
            //     // then save to Application.persistentDataPath
            //     File.WriteAllBytes(path, loadDB.bytes);

            // }
            
            path = Application.dataPath + "/unity.s3db";

            con_db = new SqliteConnection("URI=file:" + path);
            con_db.Open();
            if (File.Exists(path))
            {
                Debug.Log("database opened!");
            }
        }
        catch
        {
            Debug.Log("database not exist");
        }

    }
    public static void SetDB()
    {
        cmd_db = new SqliteCommand("SELECT *FROM user", con_db);

        IDataReader reader = cmd_db.ExecuteReader();
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);

            // Debug.Log("value= " + id + "  name =" + name);
        }
        reader.Close();
        reader = null;
        // Disconnect();
    }
    private void Disconnect()
    {
        con_db.Close();
        Debug.Log("close....");
    }

    public static string beginStatus()
    {
        string stage = "", totalStage = "", clearStage = "", miss = "";
        string statusSql = "SELECT *FROM status";
        cmd_db = new SqliteCommand(statusSql, con_db);

        IDataReader reader = cmd_db.ExecuteReader();

        if (reader.Read())
        {
            stage = reader.GetString(1);
            totalStage = reader.GetString(2);
            clearStage = reader.GetString(3);
            miss = reader.GetString(4);
        }
        Debug.Log("stage= " + stage + "  total =" + totalStage + ":" + clearStage + ":" + miss);

        reader.Close();
        reader = null;

        return stage + ":" + totalStage + ":" + clearStage + ":" + miss;
    }

    public static string getCurrentPatten(string clearStage)
    {
        print("clear.." + clearStage);
        string pattern = "", stage = "1";
        string patternSql = "SELECT pattern,storage  FROM pattern WHERE id =" + clearStage;
        cmd_db = new SqliteCommand(patternSql, con_db);

        IDataReader reader = cmd_db.ExecuteReader();

        if (reader.Read())
        {
            pattern = reader.GetString(0);
            stage = reader.GetString(1);
        }
        Debug.Log("pattern1= " + pattern);
        reader.Close();
        reader = null;

        if (pattern != "")
        {
            int patternLength = pattern.Split("・").Length;
            int pattern_num = new System.Random().Next(0, patternLength - 1);
            pattern = pattern.Split("・")[pattern_num];
        }
        Debug.Log("pattern2= " + pattern);
        string result = pattern + ":" + stage;
        return result;
    }

    public static question[,] gettingTasks(int row_count, int col_count)
    {
        // stage2 ....
        int[] stage_2_task_ids = new int[2];
        stage_2_task_ids[0] = new System.Random().Next(1, 60);
        int second_num = new System.Random().Next(1, 60);
        while (second_num == stage_2_task_ids[0])
        {
            second_num = new System.Random().Next(1, 60);
        }
        stage_2_task_ids[1] = second_num;
        bool is_setting_diff_hanzi = false;

        print("---" + stage_2_task_ids[0] + ":" + stage_2_task_ids[1]);

        question[,] tasks = new question[row_count, col_count];
        var grid = new Dictionary<Vector2Int, int>();
        for (int i = 0; i < row_count; i++)
        {
            for (int j = 0; j < col_count; j++)
            {
                if (statusManager.stage == "1")
                {
                    int num = new System.Random().Next(1, 61);
                    while (grid.ContainsValue(num))
                    {
                        num = new System.Random().Next(1, 60);
                    }

                    grid.Add(new Vector2Int(i, j), num);
                    question task = new question();
                    task.column = j;
                    task.row = i;
                    task.visible = true;
                    task.hanzi = gettingTaskData(num)[0];
                    task.reading = gettingTaskData(num)[1];
                    task.q_name = gettingTaskData(num)[2];
                    tasks[i, j] = task;
                }
                else
                {
                    question task = new question();
                    task.column = j;
                    task.row = i;
                    task.visible = true;

                    // int item_id = new System.Random().Next(0, 2);
                    int question_id = stage_2_task_ids[0];

                    // if (item_id == 1 && is_setting_diff_hanzi == false)
                    // {
                    //     question_id = stage_2_task_ids[1];
                    //     is_setting_diff_hanzi = true;
                    // }

                    task.hanzi = gettingTaskData(question_id)[0];
                    task.reading = gettingTaskData(question_id)[1];
                    task.q_name = gettingTaskData(question_id)[2];
                    tasks[i, j] = task;
                }
            }
        }

        if(statusManager.stage == "2"&&!is_setting_diff_hanzi){
            int diff_row = new System.Random().Next(0, row_count-1);
            int diff_col = new System.Random().Next(0, col_count-1);
            tasks[diff_row,diff_col].row= diff_row;
            tasks[diff_row,diff_col].column=diff_col;
            tasks[diff_row,diff_col].visible = true;
            int question_id = stage_2_task_ids[1];
            tasks[diff_row,diff_col].hanzi = gettingTaskData(question_id)[0];
            tasks[diff_row,diff_col].reading = gettingTaskData(question_id)[1];
            tasks[diff_row,diff_col].q_name = gettingTaskData(question_id)[2];
        }

        statusManager.question_2_task_hanzi = gettingTaskData(stage_2_task_ids[1])[0];
        statusManager.question_2_task_reading = gettingTaskData(stage_2_task_ids[1])[1];

        statusManager.question_1_task_hanzi = tasks[new System.Random().Next(0, row_count), new System.Random().Next(0, col_count)].hanzi;
        statusManager.question_1_task_reading = tasks[new System.Random().Next(0, row_count), new System.Random().Next(0, col_count)].reading;

        return tasks;
    }
    static string[] gettingTaskData(int task_id)
    {
        // print("task_id.." + task_id);
        string[] task = new string[3];
        string taskSql = "SELECT *FROM question WHERE id =" + task_id;
        cmd_db = new SqliteCommand(taskSql, con_db);

        IDataReader reader = cmd_db.ExecuteReader();

        if (reader.Read())
        {
            task[0] = reader.GetString(1);
            task[1] = reader.GetString(2);
            task[2] = reader.GetString(3);
        }
        // Debug.Log("task= " + task[0]);
        reader.Close();
        reader = null;

        return task;

    }

    public static string gettingStage(int clear_stage)
    {
        string stageSql = "SELECT storage FROM pattern WHERE id=" + clear_stage.ToString();
        string stage = "1";
        cmd_db = new SqliteCommand(stageSql, con_db);

        IDataReader reader = cmd_db.ExecuteReader();

        if (reader.Read())
        {
            stage = reader.GetString(0);
        }

        reader.Close();
        reader = null;

        return stage;
    }
    public static void settingStage(string current_stage, int total_stage, int clear_stage, int miss)
    {
        print("settingStage");
        string updateSql = "UPDATE status SET stage='" + current_stage + "',total='+" + total_stage.ToString()
                         + "',clear='" + clear_stage.ToString() + "',miss='" + miss.ToString() + "'";

        cmd_db = new SqliteCommand(updateSql, con_db);

        cmd_db.ExecuteScalar();
    }

    public static void settingBeginStage(string current_stage, int clear_stage, int miss)
    {
        string updateSql = "UPDATE status SET stage='" + current_stage + "',clear='" + clear_stage.ToString() + "',miss='" + miss.ToString() + "'";

        cmd_db = new SqliteCommand(updateSql, con_db);

        cmd_db.ExecuteScalar();
    }


    public static string gettingLimitTime(string pattern, string stage)
    {
        string limitSql = "SELECT *FROM limit_time WHERE pattern='" + pattern.ToString() + "' AND stage='" + stage + "'";
        print("litime:" + limitSql);
        string limit_time = "";
        cmd_db = new SqliteCommand(limitSql, con_db);

        IDataReader reader = cmd_db.ExecuteReader();

        if (reader.Read())
        {
            limit_time = reader.GetString(3);
        }

        reader.Close();
        reader = null;

        return limit_time;
    }
}