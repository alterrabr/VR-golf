using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generate a certain number of balls at a point
/// </summary>
public class BallsGenerator : MonoBehaviour
{
    [Header("Generate settings")] 
    [Tooltip("Number of balls which will be created")] 
    [SerializeField]
    private int numberOfBalls = 10;

    [Tooltip("Ball that will be instanstiate by generator")] 
    [SerializeField] 
    private GameObject ballPrefab;

    [Tooltip("Point which generation occurs around")] 
    [SerializeField]
    private Transform generatePoint;

    private List<GameObject> balls;
    private float ballDiameter;

    private int circleNumber = 1;
    private float radius;

    private const float OffsetForGenerateBalls = 0.005f;

    private void Start()
    {
        ballDiameter = ballPrefab.GetComponent<SphereCollider>().radius * 2 *
                       ballPrefab.transform.lossyScale.x;
        balls = new List<GameObject>();
        GenerateFirstBall();
        GenerateBallsToBasket();
    }

    private void GenerateFirstBall()
    {
        balls.Add(Instantiate(ballPrefab, generatePoint));
    }

    private void GenerateBallsToBasket()
    {
        int currentNumberOfBalls = CountMaxBallsInCircle();
        for (int i = 0; i < currentNumberOfBalls; i++)
        {
            if (balls.Count >= numberOfBalls)
            {
                return;
            }

            float angle = i * Mathf.PI * 2f / currentNumberOfBalls;

            Vector3 newPos = new Vector3(generatePoint.position.x + Mathf.Cos(angle) * radius,
                generatePoint.position.y, generatePoint.position.z + Mathf.Sin(angle) * radius);

            balls.Add(Instantiate(ballPrefab, newPos, Quaternion.identity));
        }

        circleNumber++;
        GenerateBallsToBasket();
    }

    private int CountMaxBallsInCircle()
    {
        radius = (ballDiameter+OffsetForGenerateBalls) * circleNumber;
        float lengthOfCircle = 2 * Mathf.PI * radius;
        int ballsInCircle = Mathf.FloorToInt(lengthOfCircle / (ballDiameter+OffsetForGenerateBalls) );
        return ballsInCircle;
    }
}