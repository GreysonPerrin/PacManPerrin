using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 4.0f; // how fast the ghost moves
    public float delay = 0; // amount of time before the ghost is released
    public bool active = false; // has the ghost been released yet?

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!active && delay >= 0) // if the ghost has not been released and the delay is not finished
        {
            delay -= Time.deltaTime; // decrease the delay time
            if (delay < 0)
            {
                // when the delay time is over, release the ghost and have it start moving
                active = true;
                gameObject.transform.SetPositionAndRotation(new Vector3(0, 0, 1.5f), Quaternion.identity);
                StartCoroutine("RandomDirections");
            }
        }
        
        // if the ghost approaches the exits, have it change direction
        if (gameObject.transform.position.x >= 7)
        {
            rb.velocity = Vector3.left * speed;
        }
        if (gameObject.transform.position.x <= -7)
        {
            rb.velocity = Vector3.right * speed;
        }
    }

    private bool CheckDirection(Vector3 direction)
    {
        // send a ray in the specified direction
        Ray ray = new Ray(gameObject.transform.position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.75f))
        {
            if (hit.collider.tag == "Wall")
            {
                return false; // if the ray hits a wall, do not change direction
            }
            else
            {
                rb.velocity = direction * speed;
                return true; // if the ray does not hit a wall, go in the specified direction
            }
        }
        else
        {
            rb.velocity = direction * speed;
            return true; // if the ray does not hit anything, go in the specified direction
        }
    }

    IEnumerator RandomDirections()
    {
        while (true)
        {
            bool canMove = false;
            while (!canMove) // repeat until a possible direction is found
            {
                int direction = Random.Range(0, 4); // pick a random direction
                switch (direction)
                {
                    case (0): //left
                        if (rb.velocity != (Vector3.right * speed)) // if the direction is not right
                        {
                            canMove = CheckDirection(Vector3.left); // check that there is not a wall in the way
                        }
                        break;
                    case (1): //up
                        if (rb.velocity != (Vector3.back * speed)) // if the direction is not down
                        {
                            canMove = CheckDirection(Vector3.forward); // check that there is not a wall in the way
                        }
                        break;
                    case (2): //right
                        if (rb.velocity != (Vector3.left * speed)) // if the direction is not left
                        {
                            canMove = CheckDirection(Vector3.right); // check that there is not a wall in the way
                        }
                        break;
                    case (3): //down
                        if (rb.velocity != (Vector3.forward * speed)) // if the direction is not up
                        {
                            canMove = CheckDirection(Vector3.back); // check that there is not a wall in the way
                        }
                        break;
                    default:
                        Debug.Log("Error: direction value in RandomDirections coroutine in Ghost script should be between 0 and 3.");
                        break;
                }
            }
            
            yield return new WaitForSeconds(0.5f); // check every half second
        }
    }
}
