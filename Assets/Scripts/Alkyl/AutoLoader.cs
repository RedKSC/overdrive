using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLoader{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnLoad(){
        AutoLoaderSettings settings = Resources.Load<AutoLoaderSettings>("Auto Loader Settings");

        if(settings){
            foreach(GameObject obj in settings.Objects){
                Object.Instantiate(obj);
                //All objects spawned like this must be DontDestroyOnLoad
                //This is because this method only gets called whent he game first loads
                Object.DontDestroyOnLoad(obj);
            }
        }
    }
}
