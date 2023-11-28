using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dice : MonoBehaviour
{
    [SerializeField] bool autoSetNumbers = false;
    public TextMeshPro[] faceNumberList;
    private int sideNumber = 1;
    // Start is called before the first frame update
    void Awake()
    {
        if (autoSetNumbers)
            SetNumbers();
    }
    public void SetNumbers()
    {
        foreach (var textMeshPro in faceNumberList)
        {
            textMeshPro.text = sideNumber.ToString();
            // Sprawdź, czy sideNumber to 6 lub 9, i dodaj tag podkreślenia
            if (sideNumber == 6 || sideNumber == 9)
            {
                textMeshPro.fontStyle = FontStyles.Underline;
            }
            sideNumber +=1 ;
        }   
    }

}
