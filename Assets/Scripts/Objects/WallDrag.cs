using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDrag : ObjectDrag
{

    LayerMask placementRaycast, objectCellMask, gridCellMask, wallGridMask;

    float pos_lerp_duration = 0.15f;


    Quaternion rot1, rot2;

    private void Start()
    {
        placementRaycast = 1 << LayerMask.NameToLayer("Placement Raycast");
        objectCellMask = 1 << LayerMask.NameToLayer("Object Cell");
        wallGridMask = 1 << LayerMask.NameToLayer("Wall Placement Raycast");

        rot1 = new Quaternion(0, 0, 0, 1f);
        rot2 = new Quaternion(0, -0.7f, 0, 0.7f);
    }

    public void OnMouseDown()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        if (Physics.Raycast(ray, out hit, 1000, placementRaycast))
        {
            Controller.selected_object = this.gameObject;
            Controller.object_drag_offset = hit.point - transform.position;
        }


        GetComponent<Animator>().Play("ObjectBounceStart");

    }

    public override void RotateObject()
    {
        if (transform.rotation == rot1)
            transform.rotation = rot2;
        else
            transform.rotation = rot1;
    }


    public override void PlaceObject()
    {
        if (Controller.selected_object == null)
            return;

        Controller.selected_object = null;

        //Snap object to grid
        Transform wall_grid = GameObject.Find("Scripts").GetComponent<Grid>().wall_grid.transform;
        Transform closest_cell = wall_grid.GetChild(0);
        Transform root_cell = transform.GetChild(0);

        List<Transform> available_cells = new List<Transform>();
        foreach (Transform cell in wall_grid)
        {
            if (IsInGrid2(cell))
            {
                available_cells.Add(cell);
            }
        }

        closest_cell = available_cells[0];


        foreach (Transform cell in available_cells)
        {
            if (Vector3.Magnitude(root_cell.position - cell.position) < Vector3.Magnitude(root_cell.position - closest_cell.position))
            {
                closest_cell = cell;
            }
        }

        transform.rotation = closest_cell.rotation;
        transform.Rotate(90, 90, 90);

        StartCoroutine(LerpPosition(closest_cell.position)); 
    }


    bool IsCellInGrid(Transform cell)
    {
        Transform wall_grid = GameObject.Find("Scripts").GetComponent<Grid>().wall_grid.transform;

        foreach (Transform grid_cell in wall_grid)
        {
            if (Vector3.SqrMagnitude(grid_cell.position - cell.position) < 0.1f)
            {
                return true;
            }
        }

        return false;
    }

    bool IsInGrid2(Transform cell)
    {
        Transform root_cell = transform.GetChild(0);

        Vector3 init_pos = root_cell.position;
        Quaternion init_rot = transform.rotation;

        transform.position = cell.position;
        transform.rotation = cell.rotation;
        transform.Rotate(90, 90, 90);


        foreach (Transform child in transform)
        {
            if (child.tag != "ObjectCell")
                continue;

            if (!IsCellInGrid(child))
            {
                transform.position = init_pos;
                transform.rotation = init_rot;
                return false;
            }
        }

        //Overlap with other objects
        foreach (Transform child in transform)
        {
            if (child.tag != "ObjectCell")
                continue;

            //Instantiate(sphere, child.position, Quaternion.identity, sphere_p.transform);

            foreach (Collider col in Physics.OverlapSphere(child.position, 0.25f, objectCellMask))
            {
                if (col.transform.parent != transform)
                {
                    transform.position = init_pos;
                    transform.rotation = init_rot;
                    return false;
                }
            }
        }

        transform.position = init_pos;
        transform.rotation = init_rot;
        return true;
    }

    //---
    bool IsInGrid(Vector3 offset)
    {
        foreach (Transform child in transform)
        {
            if (child.tag != "ObjectCell")
                continue;

            if (Physics.OverlapSphere(child.transform.position + offset, 0.25f, wallGridMask).Length == 0)
            {
                return false;
            }

            foreach (Collider col in Physics.OverlapSphere(child.transform.position + offset, 0.25f, objectCellMask))
            {
                if (col.transform.parent != transform)
                {
                    return false;
                }
            }
        }


        return true;
    }


    List<Vector3> spaces;
    List<Vector3> visited;
    bool found = false;
    public override void TranslateIntoGrid()
    {
        spaces = new List<Vector3>();
        visited = new List<Vector3>();
        found = false;

        TranslateBacktrack(visited, transform.position);


        int min = 0;
        for (int i = 1; i < spaces.Count; i++)
        {
            if (Vector3.Magnitude(transform.position - spaces[i]) < Vector3.Magnitude(transform.position - spaces[min]))
                min = i;
        }

        StartCoroutine(LerpPosition(spaces[min]));
    }

    //Assign indice to grid squares - use that instead of overlapping sphere and shi


    void TranslateBacktrack(List<Vector3> visited, Vector3 pos)
    {
        if (visited.Contains(pos))
            return;

        visited.Add(pos);


        if (IsInGrid(pos - transform.position))
        {
            spaces.Add(pos);
            return;
        }

        if (Mathf.Abs((pos + Vector3.forward).z) <= 4.5f && !visited.Contains(pos + Vector3.forward) && spaces.Count < 5)
            TranslateBacktrack(visited, pos + Vector3.forward);

        if (Mathf.Abs((pos + Vector3.back).z) <= 4.5f   && !visited.Contains(pos + Vector3.back) && spaces.Count < 5)
            TranslateBacktrack(visited, pos + Vector3.back);

        if (Mathf.Abs((pos + Vector3.right).x) <= 4.5f   && !visited.Contains(pos + Vector3.right) && spaces.Count < 5)
            TranslateBacktrack(visited, pos + Vector3.right);

        if (Mathf.Abs((pos + Vector3.left).x) <= 4.5f   && !visited.Contains(pos + Vector3.left) && spaces.Count < 5)
            TranslateBacktrack(visited, pos + Vector3.left);

        if (Mathf.Abs((pos + Vector3.up).y) <= 4.5f   && !visited.Contains(pos + Vector3.up) && spaces.Count < 5)
            TranslateBacktrack(visited, pos + Vector3.up);

        if (Mathf.Abs((pos + Vector3.down).y) <= 4.5f   && !visited.Contains(pos + Vector3.down) && spaces.Count < 5)
            TranslateBacktrack(visited, pos + Vector3.down);

    }

    IEnumerator LerpPosition(Vector3 endPos)
    {
        GetComponent<Animator>().Play("ObjectBounceEnd");

        float time = 0;
        Vector3 init_pos = transform.position;


        while (time < pos_lerp_duration)
        {
            transform.position = Vector3.Lerp(init_pos, endPos, time / pos_lerp_duration);

            time += Time.deltaTime;

            yield return null;
        }

        transform.position = endPos;
    }


}
