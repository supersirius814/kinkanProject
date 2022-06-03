using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RakutenControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
             
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void show_portal(){
       RakutenReward.GetInstance().LogAction("YdxgoVfiary__qZK"); 
       RakutenReward.GetInstance().OpenPortal();  
    }
}
