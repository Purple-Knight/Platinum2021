using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharBox : MonoBehaviour
{
    // Color -------------------------------------
    Image image;
    public List<Color> colorList = new List<Color>();
    public List<Sprite> characterList = new List<Sprite>();


    // Variables -------------------------------------
    [HideInInspector] public int idColor;
    [HideInInspector] public int idChar;
    [HideInInspector] public bool once;

    private void Start()
    {
        image = GetComponent<Image>();
        image.color = colorList[0];
        image.sprite = characterList[0];
    }

    public void changeColor(bool up)
    {
        if (!once)
        {
            once = true;

            if (up)
            {
                idColor++;
                if (idColor > colorList.Count - 1) idColor = 0;
            }
            else
            {
                idColor--;
                if (idColor < 0) idColor = colorList.Count - 1;
            }

            image.color = colorList[idColor];
        }
    }

    public void changeChar(bool up)
    {
        if (!once)
        {
            once = true;

            if (up)
            {
                idChar++;
                if (idChar > characterList.Count - 1) idChar = 0;
            }
            else
            {
                idChar--;
                if (idChar < 0) idChar = characterList.Count - 1;
            }

            image.sprite = characterList[idChar];
        }
    }

}
