using Unity.VisualScripting;
using UnityEngine;

public class PlacementSys : MonoBehaviour
{
    [SerializeField] private GameObject objectIndicator, cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    private void Update()
    {
        Vector3 touchPos = inputManager.PosOnGrid();
        Vector3Int cellPos = grid.WorldToCell(touchPos);
        objectIndicator.transform.position = touchPos;
        cellIndicator.transform.position = grid.CellToWorld(cellPos);  

    }
}
