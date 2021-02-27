using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public MeshRenderer Renderer;
    private Material material;

    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;

    private Vector3 directionVector;
    private Vector3 destinationVector = new Vector3(0, 0, 0);

    private Color[] defaultColours = {Color.red, Color.green, Color.blue, Color.magenta, Color.yellow, Color.cyan};
    private Color startColour = new Color(0.5f, 1.0f, 0.3f, 1.0f);
    private Color nextColour;

    void Start()
    {
        material = Renderer.material;
        material.color = startColour;

        InvokeRepeating("PickNextColour", 0.0f, 5.0f); // every 5 seconds pick a new colour

        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        objectWidth = 0.5f;
        objectHeight = 0.5f;
    }

    void Update()
    {
        // Generate new point, if already reached destination
        if (transform.position == destinationVector)
        {
            GenerateDestination();
            // Keep generating so long the generated point is within boundaries
            while (!IsWithinBoundaries()) {
                GenerateDestination();
            }
        }
        else
        {
            // Move the cube
            MoveCube();
            // Rotate cube
            RotateCube();
        }

        // Change colour of the cube
        ChangeColour();
    }

    private void GenerateDestination()
    {
        float randomAngle = Random.Range(0, 2 * Mathf.PI); // in radians
        float magnitude = 5.0f; 
        //Gets an XY direction of magnitude from a radian angle relative to the x axis        
        directionVector = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * magnitude;
        destinationVector = transform.position + directionVector;
    }

    private void MoveCube()
    {
        float magnitude = 5.0f;
        float step = magnitude * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, destinationVector, step);
    }

    private void RotateCube()
    {
        float speed = 50.0f;
        // rotate towards the direction of the destination vector.
        transform.Rotate(directionVector.y * Time.deltaTime * speed, -directionVector.x * Time.deltaTime * speed, 0.0f);
    }

    private bool IsWithinBoundaries()
    {
        // if within min_x, max_x, min_y, max_y
        if (destinationVector.x > screenBounds.x + objectWidth
            && destinationVector.x < screenBounds.x * -1 - objectWidth
            && destinationVector.y > screenBounds.y + objectHeight
            && destinationVector.y < screenBounds.y * -1 - objectHeight)
        {
            return true;
        }
        return false;
    }

    private void PickNextColour()
    {
        // random number exclusive the max
        int index = Random.Range(0, defaultColours.Length);
        nextColour = defaultColours[index];
    }

    private void ChangeColour()
    {
        material.color = Color.Lerp(startColour, nextColour, Mathf.PingPong(Time.time, 1));
    }
}
