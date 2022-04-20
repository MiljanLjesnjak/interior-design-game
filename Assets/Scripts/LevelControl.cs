using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class LevelObject
{
    public string name;
    public Vector3 pos;
    public Quaternion rot;
    public List<Transform> cells;

    public LevelObject(string name, Vector3 pos, Quaternion rot)
    {
        this.name = name;
        this.pos = pos;
        this.rot = rot;

        cells = new List<Transform>();
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
        room_animator = Camera.main.gameObject.GetComponent<Animator>();

        Application.targetFrameRate = 60;
        //Screen.SetResolution(960, 1973, true);   
    }

    public void StartLevel()
    {
        canvas.GetComponent<Animator>().Play("Start Level Canvas");
        //play_button.GetComponent<Animator>().Play("ButtonPush");

        foreach (Transform prefab in prefabs_preview)
        {
            LevelObject lvl_obj = new LevelObject(prefab.name, prefab.localPosition, prefab.rotation);

            foreach (Transform obj_cell in prefab)
            {
                if (obj_cell.tag != "ObjectCell")
                    continue;

                lvl_obj.cells.Add(obj_cell);

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

            objs.Add(lvl_obj);
        }


        //GameObject.Find("Grid").transform.parent = GameObject.Find("Room Play").transform;
        //GameObject.Find("Grid").transform.localPosition = Vector3.zero;

        room_animator.Play("RoomPreviewEnd");

    }


    bool CellsOverlapping(LevelObject preview_obj, Transform play_obj)
    {
        bool found_match;

        foreach (Transform play_obj_cell in play_obj.transform)  //Check position
        {
            if (play_obj_cell.tag != "ObjectCell")
                continue;

            found_match = false;

            foreach (Transform preview_obj_cell in preview_obj.cells)
            {
                if (Vector3.Distance(play_obj_cell.position + 40 * Vector3.down, preview_obj_cell.position) <= 0.1f)
                    found_match = true;
            }

            if (found_match)
                continue;


            //Debug.LogError("Wrong position: " + play_obj.name);
            return false;
        }

        return true;
    }

    bool RotationsMatching(LevelObject preview_obj, Transform play_obj)
    {
        if (play_obj.CompareTag("Ignore Rotation")) //Completely ignore rotation
        {
            return true;
        }
        else if (play_obj.CompareTag("Ignore Rotation Partly"))   //Ignore rotation around y axis for 180deg, eg. a 2x1 table
        {
            //float deg_diff = Quaternion.Angle(preview_obj.rot, play_obj.rotation);

            //if (deg_diff == 0 || deg_diff == 180)
            //    return true;

            if (Mathf.Abs(Quaternion.Dot(preview_obj.rot, play_obj.rotation)) >= 1 - 0.1f)
                return true;

            if (Mathf.Abs(Quaternion.Dot(preview_obj.rot, play_obj.rotation * Quaternion.Euler(0, 180, 0))) >= 1 - 0.1f)
                return true;

        }
        else if (Mathf.Abs(Quaternion.Dot(preview_obj.rot, play_obj.rotation)) >= 1 - 0.1f)
        {
            return true;
        }

        //Debug.LogError("Wrong rotation: " + play_obj.name);

        return false;
    }


    bool ObjectPlacedCorrectly(Transform play_obj)
    {
        foreach (LevelObject preview_obj in objs)
        {
            if (preview_obj.name != play_obj.name)
                continue;

            if (CellsOverlapping(preview_obj, play_obj) && RotationsMatching(preview_obj, play_obj))
                return true;
        }


        Debug.Log(play_obj.name);
        return false;
    }


    public bool LevelIsCompleted()
    {
        foreach (Transform play_obj in prefabs_play.transform)
        {
            if (!ObjectPlacedCorrectly(play_obj))
                return false;
        }


        return true;
    }


    bool levelFinished = false;
    public void RoomPreview()
    {
        if (LevelIsCompleted())
        {
            levelFinished = true;
            Invoke("ShowLevelEndPanel", room_animator.GetCurrentAnimatorStateInfo(0).length);
        }

        room_animator.SetBool("Preview", true);
    }

    public void ShowLevelEndPanel()
    {
        game_end_panel.SetActive(true);
        GameObject.Find("Sound Manager").GetComponent<SoundManager>().PlayLevelEnd();
    }

    public void RoomPreviewEnd()
    {
        if (levelFinished)
            return;

        room_animator.SetBool("Preview", false);
    }



}
