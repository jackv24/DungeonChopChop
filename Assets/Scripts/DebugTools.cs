using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour
{
    public bool showDebugMenu = false;
    private bool enableCheats = false;

    public string enableCheatsString = "gimme cheats";

    private bool showInputField = false;
    private string inputString;

    public float fpsUpdateRate = 4.0f;
    private float dt = 0;
    private float fps = 0;
    private int frameCount = 0;

    void Start()
	{
        showDebugMenu = Application.isEditor;
        enableCheats = Application.isEditor || Debug.isDebugBuild;
    }

	private void Update()
    {
        frameCount++;
        dt += Time.deltaTime;
		if(dt > 1.0f/fpsUpdateRate)
		{
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0f / fpsUpdateRate;
        }

        if(!LevelGenerator.Instance)
            return;

		if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.I))
		{
			showInputField = !showInputField;
		}

		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D))
            showDebugMenu = !showDebugMenu;

		if (showInputField && Input.GetKeyDown(KeyCode.Return))
		{
            if (inputString == enableCheatsString)
            {
                enableCheats = true;
                Debug.Log("<b>Enabled cheats!</b>");
            }
			else
                Debug.Log("Unrecognised command");
        }

        //Debug and testing shortcuts
        if (enableCheats)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                List<LevelTile> generatedTiles = LevelGenerator.Instance.generatedTiles;

                if (Input.GetKeyDown(KeyCode.U))
                {
                    Debug.Log("Revealing tiles " + generatedTiles.Count);

                    foreach (LevelTile tile in generatedTiles)
                    {
                        if (tile != LevelGenerator.Instance.currentTile)
                            tile.ShowTile(false, true);
                    }
                }
            }

            if (Input.GetKey(KeyCode.T))
            {
				List<LevelTile> generatedTiles = LevelGenerator.Instance.generatedTiles;

                //Teleport codes for overworld
                if (LevelGenerator.Instance.profile is OverworldGeneratorProfile)
                {
                    //Teleport to dungeons
                    bool dungeonTP = false;
                    LevelTile.Biomes dungeonBiome = LevelTile.Biomes.Grass;

                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        dungeonTP = true;
                        dungeonBiome = LevelTile.Biomes.Forest;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        dungeonTP = true;
                        dungeonBiome = LevelTile.Biomes.Desert;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        dungeonTP = true;
                        dungeonBiome = LevelTile.Biomes.Ice;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha4))
                    {
                        dungeonTP = true;
                        dungeonBiome = LevelTile.Biomes.Fire;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha0)) //Teleport to town
                    {
                        PlayerInformation[] playerInfos = FindObjectsOfType<PlayerInformation>();

                        generatedTiles[0].SetCurrent(LevelGenerator.Instance.currentTile);

                        foreach (PlayerInformation playerInfo in playerInfos)
                            playerInfo.transform.position = generatedTiles[0].tileOrigin.position;
                    }

                    if (dungeonTP)
                    {
                        //Find all dungeons, to match to correct biome
                        DungeonEntrance[] dungeons = FindObjectsOfType<DungeonEntrance>();

                        foreach (DungeonEntrance entrance in dungeons)
                        {
                            LevelTile tile = entrance.GetComponentInParent<LevelTile>();

                            //If this dungeon is on a tile of the correct biome, it is the one
                            if (tile && tile.Biome == dungeonBiome)
                            {
                                //Randomly choose a door to walk in from
                                Transform doorTransform = tile.doors[UnityEngine.Random.Range(0, tile.doors.Count)];

                                LevelDoor door = doorTransform.GetComponent<LevelDoor>();

                                if (door)
                                {
                                    //Walk in from this doors connected door
                                    LevelDoor walkIntoDoor = door.targetDoor;

                                    if (walkIntoDoor)
                                    {
                                        PlayerInformation[] playerInfos = FindObjectsOfType<PlayerInformation>();

                                        walkIntoDoor.targetTile.SetCurrent(LevelGenerator.Instance.currentTile);

                                        foreach (PlayerInformation playerInfo in playerInfos)
                                            playerInfo.transform.position = walkIntoDoor.transform.position + (-walkIntoDoor.transform.forward) * walkIntoDoor.exitDistance;
                                    }
                                }

                                //Once we've teleported, no need to continue
                                break;
                            }
                        }
                    }
                }
                //Teleport codes for dungeon
                else if (LevelGenerator.Instance.profile is DungeonGeneratorProfile)
                {
                    DungeonGeneratorProfile dungeonProfile = (DungeonGeneratorProfile)LevelGenerator.Instance.profile;
                    LevelTile tpTile = null;

                    //Get tile to teleport to
                    if (Input.GetKeyDown(KeyCode.Alpha0))
                    {
                        tpTile = generatedTiles[0];
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        tpTile = dungeonProfile.keyTileObj.GetComponentInParent<LevelTile>();
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        tpTile = dungeonProfile.chestTileObj.GetComponentInParent<LevelTile>();
                    }

                    //if tile was selected to teleport, teleport to it
                    if (tpTile)
                    {
                        //Randomly choose a door to walk in from
                        Transform doorTransform = tpTile.doors[UnityEngine.Random.Range(0, tpTile.doors.Count)];

                        LevelDoor door = doorTransform.GetComponent<LevelDoor>();

                        if (door)
                        {
                            //Walk in from this doors connected door
                            LevelDoor walkIntoDoor = door.targetDoor;

                            if (walkIntoDoor)
                            {
                                PlayerInformation[] playerInfos = FindObjectsOfType<PlayerInformation>();

                                walkIntoDoor.targetTile.SetCurrent(LevelGenerator.Instance.currentTile);

                                foreach (PlayerInformation playerInfo in playerInfos)
                                    playerInfo.transform.position = walkIntoDoor.transform.position + (-walkIntoDoor.transform.forward) * walkIntoDoor.exitDistance;
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnGUI()
    {
		if(showInputField)
		{
            inputString = GUI.TextField(new Rect(10, Screen.height - 30, 200, 20), inputString);
        }

        if (showDebugMenu)
        {
            GUI.Label(new Rect(5, 5, 150, 20), "FPS: " + fps.ToString("0.00"));

            Vector2 size = new Vector2(Screen.width - 10, 400);
            Vector2 pos = new Vector2(10, 200);

            string text = "<b>Debug Menu:</b>\n";

            text += "Start Seed: " + LevelGenerator.Instance.startSeed + "\n\n";
            text += "Current Tile: " + (LevelGenerator.Instance.currentTile ? LevelGenerator.Instance.currentTile.gameObject.name : "NULL") + "\n";
            if (LevelGenerator.Instance.currentTile)
                text += "Current Tile Graphic: " + (LevelGenerator.Instance.currentTile.currentGraphic ? LevelGenerator.Instance.currentTile.currentGraphic.gameObject.name : "NULL") + "\n";

            if (LevelGenerator.Instance.profile is OverworldGeneratorProfile)
            {
                OverworldGeneratorProfile p = (OverworldGeneratorProfile)LevelGenerator.Instance.profile;

                text += "\n<b>Biomes</b>\n";
                text += "Top Left: " + p.topLeftBiome.ToString() + "\n";
                text += "Top Right: " + p.topRightBiome.ToString() + "\n";
                text += "Bottom Left: " + p.bottomLeftBiome.ToString() + "\n";
                text += "Bottom Right: " + p.bottomRightBiome.ToString() + "\n";
            }

            if (enableCheats)
            {
                text += "\n<b>Hotkeys</b>\n";
                text += "Debug Menu: CTRL+D\n";
                text += "Reveal Map: CTRL+U\n";

                if (LevelGenerator.Instance.profile is OverworldGeneratorProfile)
                {
                    text += "\nTP to Dungeon 1: T+1\n";
                    text += "TP to Dungeon 2: T+2\n";
                    text += "TP to Dungeon 3: T+3\n";
                    text += "TP to Dungeon 4: T+4\n";
                    text += "\nTP to Town: T+0\n";
                }
                else if (LevelGenerator.Instance.profile is DungeonGeneratorProfile)
                {
                    text += "\nTP to Entrance: T+0\n";
                    text += "TP to Key Room: T+1\n";
                    text += "TP to Chest Room: T+2\n";
                }
            }

            GUI.Label(new Rect(pos, size), text);
        }
    }
}
