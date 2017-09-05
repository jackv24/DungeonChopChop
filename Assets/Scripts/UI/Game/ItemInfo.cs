using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour {

    public Text itemNameText;
    public Text itemInfoText;

    private Animator[] animators;

	// Use this for initialization
	void Start () 
    {
        animators = GetComponentsInChildren<Animator>();
	}

    public void ShowItemInfo(BaseItem item)
    {  
        foreach (Animator animator in animators)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Default"))
            {
                itemNameText.text = "" + item.displayName;
                itemInfoText.text = "" + item.itemInfo;
                animator.SetTrigger("ShowItemInfo");
            }
        }
    }

}
