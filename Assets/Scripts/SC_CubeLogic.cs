using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SC_CubeLogic;

public class SC_CubeLogic : MonoBehaviour
{
    #region delegates
    public delegate void RowDone(float row);
    public static event RowDone OnRowDone;
    #endregion

    #region MonoBehaviour
    void OnEnable()
    {
        SC_BlockMovments.OnImDoneFalling += CheckRowDone;
        OnRowDone += KillCube;
    }
    void OnDisable()
    {
        SC_BlockMovments.OnImDoneFalling -= CheckRowDone;
        OnRowDone -= KillCube;
    }
    #endregion
    #region event logic
    private void KillCube(float row)
    {
        int y = Mathf.RoundToInt(transform.position.y);
        if (y == row)
        {
            transform.Translate(transform.forward * 5);
            Destroy(gameObject);
        }
    }
    private void CheckRowDone()
    {
        int row = Mathf.RoundToInt(transform.position.y);
        for (int x = -4; x <= 4; x++)
        {
            RaycastHit hit;
            if (!Physics.Raycast(new Vector3(x, row, -1), Vector3.forward, out hit, 2f))
            {
                return;
            }
            if (hit.transform.gameObject.name == "BottomWall")
            {
                return;
            }
        }
        Debug.Log("row " + row + " is done");
        if (OnRowDone != null)
        {
            OnRowDone(row);
        }
    }
    #endregion
}
