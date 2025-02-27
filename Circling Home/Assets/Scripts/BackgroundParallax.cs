using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    private float length;
    private float startPos;
    [SerializeField] public GameObject camera;
    [SerializeField] public float parallaxEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // relative temporary position to camera
        float tempPos = (camera.transform.position.x * (1 - parallaxEffect));
        float distance = camera.transform.position.x * parallaxEffect;
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        // if statements to make background repeat
        if (tempPos > (startPos + length)) 
        {
            startPos += length * 2;
        }
        else if (tempPos < (startPos - length))
        {
            startPos -= length * 2;
        }
    }
}
