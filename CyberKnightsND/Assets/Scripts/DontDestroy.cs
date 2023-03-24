using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i < Object.FindObjectsOfType<DontDestroy>(true).Length; i++)
        {
            if(Object.FindObjectsOfType<DontDestroy>(true)[i] != this)
            {
                if (Object.FindObjectsOfType<DontDestroy>(true)[i].name == gameObject.name)
                {
                    Destroy(gameObject);
                }
            }
        }
   
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
