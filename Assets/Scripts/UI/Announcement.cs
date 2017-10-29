using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Announcement : MonoBehaviour {

    public static Announcement Instance;

    private Text[] text;

    void Start()
    {
        Instance = this;

        text = GetComponentsInChildren<Text>();
    }

    void OnEnable()
    {
        
    }

    public static void DoAnnouncement(string header, string description, float delayTime)
    {
        Instance.text[0].text = header;
        Instance.text[1].text = description;

        Instance.StartCoroutine(Instance.PlayText(delayTime));
    }

    IEnumerator PlayText(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        foreach (Transform child in transform)
        {
            child.GetComponent<Animator>().SetTrigger("Triggered");
        }
    }
}
