using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuOption : MonoBehaviour
{
    public bool selected = false;
    public string levelName = "";

    [SerializeField]
    Color unselectedColour;
    [SerializeField]
    Color selectedColour;
    [SerializeField]
    Image background;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(selected)
        {
            background.color = selectedColour;
        }
        else
        {
            background.color = unselectedColour;
        }
	}
}
