using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnalysisListElement : MonoBehaviour
{
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text value;

    public void SetDisplayValues(string title, string value)
    {
        this.title.text = title;
        this.value.text = value;
    }
}
