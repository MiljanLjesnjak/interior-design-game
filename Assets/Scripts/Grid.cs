using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid : MonoBehaviour
{
    public GameObject furniture_grid, furniture_cell_prefab;
    public static float furniture_grid_bound;


    public GameObject wall_grid, wall_cell_prefab;
    public static float wall_grid_bound;
    public static float wall_grid_bound_y;

    private void Start()
    {
        Vector3 grid_scale = GameObject.Find("Floor").transform.localScale;
        Vector3 cell_scale = furniture_cell_prefab.transform.localScale;
        furniture_grid_bound = (int)(grid_scale.x - cell_scale.x) / 2;

        grid_scale = GameObject.Find("Wall").transform.localScale;
        grid_scale.x -= 0.5f;
        cell_scale = wall_cell_prefab.transform.localScale;
        wall_grid_bound = (int)(grid_scale.x - cell_scale.x) - 1;
        wall_grid_bound_y = 4.5f;


        CreateFurnitureGrid();
        CreateWallGrid();

    }


    public void CreateFurnitureGrid()
    {
        Vector3 root_pos = furniture_cell_prefab.transform.position;

        Vector3 current_pos = furniture_cell_prefab.transform.position;
        current_pos.z += 1;


        while (current_pos.x >= -root_pos.x)
        {
            while (current_pos.z <= -root_pos.z)
            {
                Instantiate(furniture_cell_prefab, current_pos, Quaternion.identity, furniture_grid.transform);
                current_pos.z += 1;

            }

            current_pos.x -= 1;
            current_pos.z = root_pos.z;
        }


    }

    void CreateWallGrid()
    {
        Vector3 root_pos = wall_cell_prefab.transform.position;

        Vector3 current_pos = root_pos;
        current_pos.y += 1;

        while (current_pos.x >= -root_pos.x)
        {
            while (current_pos.y <= root_pos.y + wall_grid_bound_y)
            {
                Instantiate(wall_cell_prefab, current_pos, wall_cell_prefab.transform.rotation, wall_grid.transform);
                current_pos.y += 1;
            }

            current_pos.x -= 1;
            current_pos.y = root_pos.y;

        }

        GameObject wall_grid2 = Instantiate(wall_grid, wall_grid.transform.parent);
        wall_grid2.transform.Rotate(0, -90, 0);

        int n = wall_grid2.transform.childCount;
        for (int i = 0; i < n; i++)
        {
            wall_grid2.transform.GetChild(0).parent = wall_grid.transform;
        }

        Destroy(wall_grid2);

    }



}
