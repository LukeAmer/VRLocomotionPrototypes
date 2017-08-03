using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialControl : MonoBehaviour
{
    [SerializeField]
    GameObject hmdCamera;
    [SerializeField]
    Image image;
    [SerializeField]
    Text text;

    // Use this for initialization
    void Start()
    {
        gameObject.transform.position = hmdCamera.transform.position + hmdCamera.transform.forward * 2.0f;

        StartCoroutine(FadeOutTutImage());
        StartCoroutine(FadeText());
    }
	
	// Update is called once per frame
	void Update ()
    {
        gameObject.transform.position = Vector3.Lerp(transform.position, hmdCamera.transform.position + hmdCamera.transform.forward * 2.0f, 5.0f * Time.deltaTime);
        gameObject.transform.LookAt(hmdCamera.transform);


	}

    IEnumerator FadeOutTutImage()
    {
        yield return new WaitForSeconds(1.0f);

        while (image.color.a < 1.0f)
        {
            Color col = image.color;
            col.a += 1.0f * Time.deltaTime;

            image.color = col;

            yield return null;
        }

        yield return new WaitForSeconds(5.0f);

        while(image.color.a > 0.0f)
        {
            Color col = image.color;
            col.a -= 1.0f * Time.deltaTime;

            image.color = col;

            yield return null;
        }
    }

    IEnumerator FadeText()
    {
        yield return new WaitForSeconds(8.0f);

        while(text.color.a < 1.0f)
        {
            Color col = text.color;
            col.a += 1.0f * Time.deltaTime;

            text.color = col;

            yield return null;
        }

        yield return new WaitForSeconds(5.0f);

        while(text.color.a > 0.0f)
        {
            Color col = text.color;
            col.a -= 1.0f * Time.deltaTime;

            text.color = col;

            yield return null;
        }
    }
}
