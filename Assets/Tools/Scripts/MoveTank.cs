using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTank : MonoBehaviour
{
    public GameObject tank;
    public GameObject cannon;
    public float moveSpeed = 5;

    public void CheckInputs()
    {
        Vector3 inputMoviment = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (inputMoviment != Vector3.zero)
        {
            tank.transform.position += (inputMoviment * moveSpeed * Time.deltaTime);
            tank.transform.rotation = Quaternion.LookRotation(inputMoviment);
        }

        Vector3 inputLookCannon = GetMouseWorldPosition();

        if (inputLookCannon != Vector3.zero)
        {
            cannon.transform.rotation = Quaternion.LookRotation(inputLookCannon);
        }
    }

    public Vector3 GetMouseWorldPosition()
    {
        Plane plane = new Plane(Vector3.down, 0);

        float distance;

        Vector3 mousePostionInWorld = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            mousePostionInWorld = ray.GetPoint(distance);
        }

        return new Vector3(mousePostionInWorld.x, 0, mousePostionInWorld.z);
    }

    private void Update()
    {
        CheckInputs();
    }
}
