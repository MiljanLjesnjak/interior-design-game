using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelLoading : MonoBehaviour
{
    [SerializeField] Transform prefabs_play, prefabs_preview;

    private void Awake()
    {
        InstantiatePlaceables();
    }

    public void InstantiatePlaceables()
    {
        string[] all_levels = ((TextAsset)Resources.Load("_levels")).text.Split('\n');


        int level_index = PlayerPrefs.GetInt("level_index", 0);

        string level_parsed = all_levels[level_index];
        level_parsed = level_parsed.Substring(0, level_parsed.Length - 1);
        string[] level_objects = level_parsed.Split('\\');

        foreach (string obj in level_objects)
        {
            //Name
            string name = obj.Split('/')[0];

            //Position
            string pos_string = obj.Split('/')[1];
            string[] pos_coords = pos_string.Substring(1, pos_string.Length - 2).Split(',');
            Vector3 pos = new Vector3(
                float.Parse(pos_coords[0]),
                float.Parse(pos_coords[1]),
                float.Parse(pos_coords[2]));

            //Rotation
            string rot_string = obj.Split('/')[2];
            string[] rot_coords = rot_string.Substring(1, rot_string.Length - 2).Split(',');
            Quaternion rot = new Quaternion(
                float.Parse(rot_coords[0]),
                float.Parse(rot_coords[1]),
                float.Parse(rot_coords[2]),
                float.Parse(rot_coords[3]));

            GameObject game_obj_play = (GameObject)Instantiate(Resources.Load("Prefabs/" + name));
            game_obj_play.transform.parent = prefabs_play;
            game_obj_play.name = name;
            game_obj_play.transform.position = Vector3.one * 20f;
            game_obj_play.transform.rotation = Quaternion.Euler(0, 0, 0);
            game_obj_play.SetActive(false);



            GameObject game_obj_preview = (GameObject)Instantiate(Resources.Load("Prefabs/" + name));
            game_obj_preview.transform.parent = prefabs_preview;
            game_obj_preview.name = name;
            game_obj_preview.transform.position = pos;
            game_obj_preview.transform.rotation = rot;

            foreach (Collider col in game_obj_preview.GetComponents(typeof(Collider)))
                col.enabled = false;

            if (game_obj_preview.GetComponent<FurnitureDrag>() != null)
                game_obj_preview.GetComponent<FurnitureDrag>().enabled = false;

            if (game_obj_preview.GetComponent<WallDrag>() != null)
                game_obj_preview.GetComponent<WallDrag>().enabled = false;

            game_obj_preview.GetComponent<Animator>().enabled = false;
        }


    }



}
