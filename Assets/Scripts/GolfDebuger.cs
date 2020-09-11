using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfDebuger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("It is used to access to Ball events")]
    private Ball ballScript;

    [Tooltip("This materials will be used in debug mode for visualize ball state. 0 - ball near with hole, 1 - default ball")]
    [SerializeField]
    private List<Material> ballMaterials;

    [Tooltip("Round zone where the ball will not be exactly teleported to the starting point")]
    [SerializeField]
    private GameObject ballNoTeleportZoneRenderObject;

    [Tooltip("Round zone where the ball may be teleported to the starting point")]
    [SerializeField]
    private GameObject ballCheckTeleporttZoneRenderObject;

    [SerializeField]
    [Tooltip("This object will be disabled in debug mode")]
    private List<GameObject> obstaclesForDisabled;

    private MeshRenderer ballRenderer;
    
    private bool debugMode;

    private void Start()
    {
        ballRenderer = ballScript.GetComponent<MeshRenderer>();
        ballScript.OnBallHoleZoneEnter += SetNearHoleMaterialForBall;
        ballScript.OnBallHoleZoneExit += ReturnDefaultMaterialForBall;

        SetDebugState(false);
    }

    private void Update()
    {
        DebugActiveToggle();
        ReturnBallInput();
    }

    private void OnDestroy()
    {
        ballScript.OnBallHoleZoneEnter -= SetNearHoleMaterialForBall;
        ballScript.OnBallHoleZoneExit -= ReturnDefaultMaterialForBall;
    }

    private void DebugActiveToggle()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SetDebugState(!debugMode);
        }
    }

    private void ReturnBallInput()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            ballScript.ForceReturnBallToReturnPoint();
        }
    }

    private void SetDebugState(bool value)
    {
        debugMode = value;

        ballCheckTeleporttZoneRenderObject.SetActive(debugMode);
        ballNoTeleportZoneRenderObject.SetActive(debugMode);

        for (int i = 0; i < obstaclesForDisabled.Count; i++)
        {
            obstaclesForDisabled[i].SetActive(!debugMode);
        }

        if (debugMode)
        {
            ballCheckTeleporttZoneRenderObject.transform.localScale =
                new Vector3(
                    ballScript.BallForceReturnDistance * 2,
                    ballCheckTeleporttZoneRenderObject.transform.localScale.y,
                    ballScript.BallForceReturnDistance * 2);

            ballNoTeleportZoneRenderObject.transform.localScale =
                new Vector3(
                    ballScript.BallReturnDistance * 2,
                    ballNoTeleportZoneRenderObject.transform.localScale.y,
                    ballScript.BallReturnDistance * 2);
        }
        else
        {
            ReturnDefaultMaterialForBall();
        }
    }

    private void SetNearHoleMaterialForBall()
    {
        if(debugMode)
        {
            ballRenderer.material = ballMaterials[0];
        }
    }

    private void ReturnDefaultMaterialForBall()
    {
        ballRenderer.material = ballMaterials[1];
    }
}
