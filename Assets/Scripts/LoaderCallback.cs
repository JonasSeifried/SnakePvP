using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    bool isFirstUpdate = true;

    private void Update() {
        if(isFirstUpdate) {
            //Loader.LoadCallback();
            isFirstUpdate = false;
        }
    }
}
