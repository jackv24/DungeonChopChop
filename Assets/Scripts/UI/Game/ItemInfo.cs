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
            animator.SetTrigger("Reset");
            if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Default"))
            {
                StartCoroutine(wait(animator, item, .5f));
            }
            else
            {
                StartCoroutine(wait(animator, item, .1f));
            }
        }
    }

    IEnumerator wait(Animator animator, BaseItem item, float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetTrigger("ShowItemInfo");
        itemNameText.text = "" + item.displayName;
        itemInfoText.text = "" + item.itemInfo;
    }

}
