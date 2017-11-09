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
    public Text titleText;
    public Text descriptionText;

    [System.Serializable]
	public class TextPair
	{
        public string title;
        public string description;

		public TextPair()
		{
            title = "Empty Title";
            description = "";
        }

		public TextPair(string title)
		{
            this.title = title;
            description = "";
        }

		public TextPair(string title, string description)
		{
            this.title = title;
            this.description = description;
        }
    }

	[Space()]
    public TextPair grassText = new TextPair("Grasslands", "Very grassy");
    public TextPair forestText = new TextPair("Forest", "Quite dank");
    public TextPair desertText = new TextPair("Desert", "Much sand");
	public TextPair fireText = new TextPair("Lava Land", "Ouch very burn");

	[Space()]
	public TextPair dungeon1Text = new TextPair("Dungeon 1");
	public TextPair dungeon2Text = new TextPair("Dungeon 2");
	public TextPair dungeon3Text = new TextPair("Dungeon 3");
	public TextPair dungeon4Text = new TextPair("Dungeon 4");

    private LevelTile.Biomes lastBiome = LevelTile.Biomes.Dungeon1;

    private Coroutine fadeGroupRoutine = null;

    void Start()
	{
        LevelGenerator.Instance.OnTileEnter += Show;
    }

	public void Show()
	{
		if(LevelGenerator.Instance.currentTile == LevelGenerator.Instance.generatedTiles[0])
            return;

        TextPair textPair = null;

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
			if(titleText)
                titleText.text = textPair.title;

			if(descriptionText)
                descriptionText.text = textPair.description;

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
