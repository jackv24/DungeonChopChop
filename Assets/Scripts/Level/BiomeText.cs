using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BiomeText : MonoBehaviour
{
    public CanvasGroup group;
    public float fadeInDelay = 1.0f;
    public float fadeInTime = 0.5f;
    public float fadeHoldTime = 1.0f;
    public float fadeOutTime = 0.25f;

    [Space()]
    public Sprite[] grassText;
    public Sprite[] forestText;
    public Sprite[] desertText;
    public Sprite[] iceText;
    public Sprite[] fireText;

    [Space()]
	public Sprite[] dungeon1Text;
	public Sprite[] dungeon2Text;
	public Sprite[] dungeon3Text;
	public Sprite[] dungeon4Text;

    private LevelTile.Biomes lastBiome = LevelTile.Biomes.Dungeon1;

    private Coroutine fadeGroupRoutine = null;

    void Start()
	{
		if(LevelGenerator.Instance)
			LevelGenerator.Instance.OnTileEnter += Show;
    }

	public void Show()
	{
		if(LevelGenerator.Instance.currentTile == LevelGenerator.Instance.generatedTiles[0])
            return;

        Sprite[] textPair = null;

        TileText tileText = LevelGenerator.Instance.currentTile.GetComponentInChildren<TileText>();
        if(tileText)
            textPair = tileText.text;

        if (textPair == null)
        {
            LevelTile.Biomes biome = LevelGenerator.Instance.currentTile.Biome;

            if (biome == lastBiome)
                return;
            else
                lastBiome = biome;

            switch (biome)
            {
                case LevelTile.Biomes.Grass:
                    textPair = grassText;
                    break;
                case LevelTile.Biomes.Forest:
                    textPair = forestText;
                    break;
                case LevelTile.Biomes.Desert:
                    textPair = desertText;
                    break;
                case LevelTile.Biomes.Ice:
                    textPair = iceText;
                    break;
                case LevelTile.Biomes.Fire:
                    textPair = fireText;
                    break;
                case LevelTile.Biomes.Dungeon1:
                    textPair = dungeon1Text;
                    break;
                case LevelTile.Biomes.Dungeon2:
                    textPair = dungeon2Text;
                    break;
                case LevelTile.Biomes.Dungeon3:
                    textPair = dungeon3Text;
                    break;
                case LevelTile.Biomes.Dungeon4:
                    textPair = dungeon4Text;
                    break;
            }
        }

        if(textPair != null)
		{
			if(group)
            {
                group.gameObject.DestroyChildren();

                foreach(Sprite sprite in textPair)
                {
                    GameObject obj = new GameObject("Image");
                    obj.transform.SetParent(group.transform);

                    Image img = obj.AddComponent<Image>();
                    img.sprite = sprite;
                    img.SetNativeSize();
                    img.transform.localScale = Vector3.one;
                }
            }

			if(fadeGroupRoutine != null)
                StopCoroutine(fadeGroupRoutine);

            fadeGroupRoutine = StartCoroutine(FadeGroup());
        }
    }

	IEnumerator FadeGroup()
	{
		if(group)
		{
            yield return new WaitForSeconds(fadeInDelay);

            group.gameObject.SetActive(true);

            group.alpha = 0;

            float elapsed = 0;

			while(elapsed < fadeInTime)
			{
                group.alpha = elapsed / fadeInTime;

                yield return new WaitForEndOfFrame();
                elapsed += Time.deltaTime;
            }

            group.alpha = 1;

            yield return new WaitForSeconds(fadeHoldTime);

            elapsed = 0;
			while(elapsed < fadeOutTime)
			{
                group.alpha = 1 - (elapsed / fadeOutTime);

                yield return new WaitForEndOfFrame();
                elapsed += Time.deltaTime;
            }

            group.alpha = 0;

            group.gameObject.SetActive(false);
        }
	}
}
