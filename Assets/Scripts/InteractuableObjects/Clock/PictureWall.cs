using UnityEngine;

public class PictureWall : MonoBehaviour
{
    public GameObject note;
    public Clock clock;

    private void Awake()
    {
        note.GetComponent<Collider>().enabled = false;
    }
    private void Update()
    {
        if (clock.getPuzzleResuelto())
        {
            note.GetComponent<Collider>().enabled = true;
        }
        else
        {
            note.GetComponent<Collider>().enabled = false;
        }
    }
}

