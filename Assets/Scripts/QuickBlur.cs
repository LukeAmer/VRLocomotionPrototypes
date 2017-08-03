using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class QuickBlur : MonoBehaviour
{

    BlurOptimized blur;

	// Use this for initialization
	void Start ()
    {
        blur = gameObject.GetComponent<BlurOptimized>();
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    public IEnumerator FastBlur(float delay)
    {
        //float delay = 0.1f;

        blur.blurSize = 2.0f;
        blur.downsample = 1;
        blur.blurIterations = 2;

        blur.enabled = true;

        while(delay > 0.0f)
        {
            delay -= 1.0f * Time.deltaTime;
            yield return null;
        }

        while(blur.blurSize > 0.0f)
        {
            blur.blurSize -= 5.0f * Time.deltaTime;

            yield return null;
        }

        blur.enabled = false;

    }
}
