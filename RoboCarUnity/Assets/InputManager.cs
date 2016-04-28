using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
    float leftTrigger = 0;
    float rightTrigger = 0;
    bool input = false;
	// Update is called once per frame
	void Update () {
        leftTrigger = Input.GetAxis("LeftTrigger");
        rightTrigger = Input.GetAxis("RightTrigger");
        if (leftTrigger == 0 && rightTrigger == 0 && input)
        {
            Debug.Log("Stop");
            ConnectToTcpServer.Instance.SendControllerMove(false, 0.0f, false, 0.0f);
            input = false;
        }
        else if (leftTrigger > 0 || rightTrigger > 0)
        {
            ConnectToTcpServer.Instance.SendControllerMove(leftTrigger > 0, leftTrigger, rightTrigger > 0, rightTrigger);
            Debug.Log("LeftTrigger: " + leftTrigger + " RightTrigger: " + rightTrigger);
            input = true;
        }
        
        


	}
}
