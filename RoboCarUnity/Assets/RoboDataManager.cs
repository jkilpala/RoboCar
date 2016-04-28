using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class RoboDataManager : MonoBehaviour {
    private static RoboDataManager _instance;
    public static RoboDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RoboDataManager>();
            }
            return _instance;
        }
    }
    public List<GPSData> GPSDataList;
    public GPSData CurrentGPSData = new GPSData() { Latitude = 0, Longitude = 0 };
    private MoveData _mData = new MoveData { Left = false, Right = false, LeftValue = 0.0f, RightValue = 0.0f };
    public MoveData MData
    {
        get
        {
            return _mData;
        }
        set
        {
            _mData = value;
        }
    }
    public Text Latitude;
    public Text Longitude;
	// Use this for initialization
	void Start () {
        GPSDataList = new List<GPSData>();
	}
	
    public void AddGpsData(string data)
    {
        if (GPSDataList.Count < 15)
        {
            GPSDataList.Add(JsonUtility.FromJson<GPSData>(data));
        }
        else
        {    
            CurrentGPSData.Latitude = GPSDataList.Average<GPSData>(item => item.Latitude);
            CurrentGPSData.Longitude = GPSDataList.Average<GPSData>(item => item.Longitude);
            //Event this
            Latitude.text = CurrentGPSData.Latitude.ToString();
            Longitude.text = CurrentGPSData.Longitude.ToString();
            //
            GPSDataList.Clear();
            GPSDataList.Add(JsonUtility.FromJson<GPSData>(data));
        }
    }
	// Update is called once per frame
	void Update () {
	
	}
}
