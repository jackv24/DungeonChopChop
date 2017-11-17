using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
	private static DamageText Instance;

    public GameObject canvasPrefab;

    [Space()]
    public AnimationCurve animationCurve;
	public AnimationCurve sizeCurve;
    public Gradient animationGradient;
    public float animationLength = 0.5f;

    [Space()]
    public float minSpeed = -1f;
    public float maxSpeed = 1f;

    void Awake()
	{
        Instance = this;
    }

	public static void Show(float damageAmount, Vector3 position)
	{
		if(Instance && Instance.canvasPrefab)
		{
            Instance.StartCoroutine(Instance.ShowText(damageAmount, position));
        }
	}

	IEnumerator ShowText(float damageAmount, Vector3 position)
	{
        GameObject obj = ObjectPooler.GetPooledObject(canvasPrefab);

		if(obj)
		{
            obj.transform.position = position;

            Text text = obj.GetComponentInChildren<Text>();
			if (text)
			{
				string s = damageAmount.ToString();

				if (s.Contains("."))
				{
					int index = s.IndexOf('.');
					s = s.Remove(index - 1, 1);
				}

				text.text = s;
			}

            float speed = Random.Range(minSpeed, maxSpeed);
            Vector3 offset = Vector3.zero;

            float elapsed = 0;
			while(elapsed <= animationLength)
			{
                float time = elapsed / animationLength;

                if(text)
                    text.color = animationGradient.Evaluate(time);

                offset += Vector3.right * speed * Time.deltaTime;

                obj.transform.position = position + Vector3.up * animationCurve.Evaluate(time) + offset;
                obj.transform.localScale = canvasPrefab.transform.localScale * sizeCurve.Evaluate(time);

                yield return new WaitForEndOfFrame();
                elapsed += Time.deltaTime;
            }

            obj.SetActive(false);
        }
    }
}
