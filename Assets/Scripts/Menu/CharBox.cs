using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharBox : MonoBehaviour
{
    Image image;
    public List<Color> colorList = new List<Color>();
    int id;

    private void Start()
    {
        image = GetComponent<Image>();
        image.color = colorList[id];
    }

    public void changeColor(bool up)
    {
        if (up)
        {
            id++;
            if (id > colorList.Count - 1) id = 0;
        }
        else
        {
            id--;
            if (id < 0) id = colorList.Count - 1;
        }
        
        image.color = colorList[id];
    }

}
