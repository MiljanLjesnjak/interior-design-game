using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelObject
{
    public string name;
    public Vector3 pos;
    public Quaternion rot;

    public LevelObject(string name, Vector3 pos, Quaternion rot)
    {
        this.name = name;
        this.pos = pos;
        this.rot = rot;
    }
}

public class LevelControl : MonoBehaviour
{
    [SerializeField] Transform room;
    [SerializeField] Transform prefabs_play, prefabs_preview;


    [SerializeField] GameObject game_end_panel;
    [SerializeField] Transform grid;

    Animator room_animator;

    List<LevelObject> objs = new List<LevelObject>();

    [SerializeField] Transform canvas;

    private void Awake()
    {
        room_animator = room.GetComponent<Animator>();
        //room_animator = Camera.main.gameObject.GetComponent<Animator>();

        Application.targetFrameRate = 60;
        //Screen.SetResolution(960, 1973, true);   
    }

    public void StartLevel()
    {
        canvas.GetComponent<Animator>().Play("Start Level Canvas");
        //play_button.GetComponent<Animator>().Play("ButtonPush");

        foreach (Transform prefab in prefabs_preview)
        {
            objs.Add(new LevelObject(prefab.name, prefab.localPosition, prefab.rotation));

            foreach (Transform obj_cell in prefab)
            {
                if (obj_cell.tag != "ObjectCell")
                    continue;

                foreach (Transform grid_cell in GameObject.Find("Furniture Grid").transform)
                {
                    if (Vector3.Distance(obj_cell.position, grid_cell.position + Vector3.down * grid.parent.position.y) < 0.1f)
                        grid_cell.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                }

                foreach (Transform grid_cell in GameObject.Find("Wall Grid").transform)
                {
                    if (Vector3.Distance(obj_cell.position, grid_cell.position + Vector3.down * grid.parent.position.y) < 0.1f)
                        grid_cell.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                }

            }
        }


        //GameObject.Find("Grid").transform.parent = GameObject.Find("Room Play").transform;
        //GameObject.Find("Grid").transform.localPosition = Vector3.zero;

        room_animator.Play("RoomPreviewEnd");

    }


    bool ObjectsOverlapping(LevelObject preview_obj)
    {
        foreach (Transform play_obj in prefabs_play.transform)
        {
            if (play_obj.name != preview_obj.name)
                continue;

            if (Vector3.Distance(preview_obj.pos, play_obj.localPosition) <= 0.1f)
            {
                if (play_obj.CompareTag("Ignore Rotation"))
                    return true;
                else if (Mathf.Abs(Quaternion.Dot(preview_obj.rot, play_obj.rotation)) >= 1 - 0.1f)
                        return true;
            }
        }

        //Debug.Log(preview_obj.name);

        return false;
    }

    public bool EndLevel()
    {
        foreach (LevelObject preview_obj in objs)
        {
            if (ObjectsOverlapping(preview_obj) == false)
                return false;

        }


        return true;
    }


    bool levelFinished = false;
    public void RoomPreview()
    {
        if (EndLevel())
        {
            levelFinished = true;
            Invoke("ShowLevelEndPanel", room_animator.GetCurrentAnimatorStateInfo(0).length);
        }

        room_animator.SetBool("Preview", true);
    }

    public void ShowLevelEndPanel()
    {
        game_end_panel.SetActive(true);
    }

    public void RoomPreviewEnd()
    {
        if (levelFinished)
            return;

        room_animator.SetBool("Preview", false);
    }



}
