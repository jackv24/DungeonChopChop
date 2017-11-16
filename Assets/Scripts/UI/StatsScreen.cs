using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsScreen : MonoBehaviour
{
    public Text playtimeText;

    [Space()]
    public RectTransform mapRect;

    private Vector2 initialMapPos;
    private Vector2 initialMapSize;

    private List<Camera> cameraList = new List<Camera>();

    void FixedUpdate()
    {
        string time = string.Format("{0}:{1:00}", (int)Statistics.Instance.TotalPlayTime / 60, (int)Statistics.Instance.TotalPlayTime % 60);

        playtimeText.text = "" + time;
    }

    void OnEnable()
	{
        if (MapCamera.Instance)
        {
            MapCamera.Instance.Hide();

            initialMapPos = MapCamera.Instance.mapRect.position;
            initialMapSize = MapCamera.Instance.mapRect.sizeDelta;

            MapCamera.Instance.followRect = mapRect;
        }
    }

	void OnDisable()
	{
        foreach (Camera cam in cameraList)
        {
			if(cam)
            	Destroy(cam.gameObject);
        }

        cameraList.Clear();

        if (MapCamera.Instance)
        {
            MapCamera.Instance.followRect = null;

            MapCamera.Instance.mapRect.position = initialMapPos;
            MapCamera.Instance.mapRect.sizeDelta = initialMapSize;

            MapCamera.Instance.Show();
        }
    }
}
