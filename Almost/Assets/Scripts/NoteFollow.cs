using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NoteFollow : MonoBehaviour
{
    public GameObject noteImage;
    public GameObject stickyNotePrefab;
    public Transform canvasTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        // hide the system default cursor
        Cursor.visible = false;

        noteImage = GameObject.Find("StickyNoteMain");
        stickyNotePrefab = Resources.Load<GameObject>("Prefabs/StickyNote");
        canvasTransform = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // let the image follow the cursor
        noteImage.transform.position = Input.mousePosition;
        
        if (Input.GetMouseButtonDown(0))
        {
            SpawnStickyNote();
        }
    }

    private void SpawnStickyNote()
    {
        // step 0: generate a new note
        GameObject newNote = Instantiate(stickyNotePrefab, canvasTransform);

        // step 1: assign the position
        newNote.transform.position = Input.mousePosition;
        
        // step 2: rotate a little bit to make it cute
        newNote.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-15f, 15f));
        
        // step 3: make sure the new note is on top of all other notes
        transform.SetAsLastSibling();
    }
}
