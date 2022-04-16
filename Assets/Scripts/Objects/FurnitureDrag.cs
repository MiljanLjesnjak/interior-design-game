using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class FurnitureDrag : ObjectDrag
{

    LayerMask placementRaycast, cellMask;

    Vector3 init_scale;

    float scale_lerp_duration = 0.15f;
    float pos_lerp_duration = 0.15f;

    float hold_time = 0;

    private void Start()
    {
        placementRaycast = 1 << LayerMask.NameToLayer("Placement Raycast");
        cellMask = 1 << LayerMask.NameToLayer("Object Cell");

        init_scale = transform.GetChild(transform.childCount - 1).localScale;
    }

    public void OnMouseDown()
    {
        hold_time = 0;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000, placementRaycast))
        {
            Controller.selected_object = this.gameObject;

            Controller.object_drag_offset = hit.point - transform.position;
            Controller.object_drag_offset.y = 0;
        }


        GetComponent<Animator>().Play("ObjectBounceStart");
    }

    public void OnMouseDrag()
    {
        hold_time += Time.deltaTime;
    }

    private void OnMouseUp()
    {
        if (hold_time <= 0.075f)
        {
            RotateObject();
        }
    }

    public override void RotateObject()
    {
        transform.Rotate(0, -90, 0);
        TranslateIntoGrid();
    }


    public override void PlaceObject()
    {
        if (Controller.selected_object == null)
            return;

        Controller.selected_object = null;

        //Snap object to grid
        Transform furniture_grid = GameObject.Find("Scripts").GetComponent<Grid>().furniture_grid.transform;
        Transform closest_cell = furniture_grid.GetChild(0);
        Transform root_cell = transform.GetChild(0);    //Root cell of the object, not grid

        List<Transform> available_cells = new List<Transform>();
        foreach (Transform cell in furniture_grid)
        {
            if (IsInGrid2(cell.position))
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

        StartCoroutine(LerpPosition(closest_cell.position));
    }

    bool IsInGrid2(Vector3 pos)
    {
        Transform root_cell = transform.GetChild(0);
        Vector3 init_pos = root_cell.position;

        transform.position = pos;

        foreach (Transform child in transform)
        {
            if (child.tag != "ObjectCell")
                continue;

            if (Mathf.Abs(child.position.x) > Grid.furniture_grid_bound + 0.01f)
            {
                transform.position = init_pos;
                return false;
            }

            if (Mathf.Abs(child.position.z) > Grid.furniture_grid_bound + 0.01f)
            {
                transform.position = init_pos;
                return false;
            }
        }

        //Overlap with other objects
        foreach (Transform child in transform)
        {
            if (child.tag != "ObjectCell")
                continue;

            //Instantiate(sphere, child.position, Quaternion.identity, sphere_p.transform);

            foreach (Collider col in Physics.OverlapSphere(child.position, 0.25f, cellMask))
            {
                if (col.transform.parent != transform)
                {
                    transform.position = init_pos;
                    return false;
                }
            }
        }

        transform.position = init_pos;
        return true;
    }


    //---
    bool IsInGrid(Vector3 offset)
    {
        foreach (Transform child in transform)
        {
            if (child.tag != "ObjectCell")
                continue;

            Vector3 pos = child.transform.position + offset;

            if (Mathf.Abs(pos.x) > Grid.furniture_grid_bound + 0.01f)
            {
                return false;
            }

            if (Mathf.Abs(pos.z) > Grid.furniture_grid_bound + 0.01f)
            {
                return false;
            }

        }

        //Overlap with other objects
        foreach (Transform child in transform)
        {
            if (child.tag != "ObjectCell")
                continue;

            foreach (Collider col in Physics.OverlapSphere(child.transform.position + offset, 0.25f, cellMask))
            {
                if (col.transform.parent != transform)
                {
                    return false;
                }
            }
        }


        return true;
    }

    List<Vector3> visited;
    bool found;
    List<Vector3> spaces;
    public override void TranslateIntoGrid()
    {
        visited = new List<Vector3>();
        found = false;
        spaces = new List<Vector3>();

        TranslateBacktrack(visited, transform.position);

        int min = 0;
        for (int i = 1; i < spaces.Count; i++)
        {
            if (Vector3.Magnitude(transform.position - spaces[i]) < Vector3.Magnitude(transform.position - spaces[min]))
                min = i;
        }

        //transform.position = spaces[min];
        StartCoroutine(LerpPosition(spaces[min]));

    }

    void TranslateBacktrack(List<Vector3> visited, Vector3 pos)
    {
        if (visited.Contains(pos))
            return;

        visited.Add(pos);

        if (IsInGrid(pos - transform.position) && !found)
        {
            spaces.Add(pos);
            return;
        }

        if (Mathf.Abs((pos + Vector3.forward).z) <= 4.5f && !found)
            TranslateBacktrack(visited, pos + Vector3.forward);

        if (Mathf.Abs((pos + Vector3.back).z) <= 4.5f && !found)
            TranslateBacktrack(visited, pos + Vector3.back);

        if (Mathf.Abs((pos + Vector3.right).x) <= 4.5f && !found)
            TranslateBacktrack(visited, pos + Vector3.right);

        if (Mathf.Abs((pos + Vector3.left).x) <= 4.5f && !found)
            TranslateBacktrack(visited, pos + Vector3.left);

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
