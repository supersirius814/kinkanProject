using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform target;

    public Transform mainCam;

    public static GameObject fettis;

    public static Vector3 target_begin_position;

    public Vector3 clicked_moment_pos, ending_pos;

    // Start is called before the first frame update
    public static bool is_game_playing;

    void Start()
    {
        is_game_playing = false;
        target_begin_position = target.position;
        fettis = GameObject.Find("fettis");
        fettis.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (is_game_playing)
        {
            if (timeManager.curren_timer < timeManager.limit_timer + 0.2f)
            {
                Vector3 obj_tras_direction =
                    mainCam.GetComponent<Transform>().position -
                    target.GetComponent<Transform>().position;
                target.GetComponent<Transform>().position =
                    target.GetComponent<Transform>().position +
                    obj_tras_direction * 0.003f;
                // return;
            }
            // is_game_playing = false;
            // target.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    public void moving()
    {
        print("moving");
        clicked_moment_pos = target.position;
        ending_pos =target_begin_position+(mainCam.position-target_begin_position)*0.6f;
        StartCoroutine(move());
    }

    IEnumerator move()
    {
        float
            timeElapsed = 0,
            duration = 1.0f;

        // transform.eulerAngles = new Vector3(0, y_degree, 0);
        while (timeElapsed < duration)
        {
            target.position =
                Vector3
                    .Lerp(clicked_moment_pos,
                    ending_pos,
                    timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void camera_game_in_new_playing()
    {
        // target.GetComponent<Rigidbody>().useGravity = false;
        print("newing");
        target.position = target_begin_position;
        is_game_playing = true;
    }
}
