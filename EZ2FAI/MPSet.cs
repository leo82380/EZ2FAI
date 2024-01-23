using System;
using UnityEngine;

public class MPSet : MonoBehaviour
{
    private bool isDeleteCheck = false;
    void Update()
    {
        try
        {
            if (scrConductor.instance == null || Main.mp == null)
            {
                return;
            }
            if (scrConductor.instance.isGameWorld && !scrConductor.isEditingLevel && !scrConductor.isOfficialLevel)
            {
                Main.mp.SetActive(true);
                
                Main.mp.transform.GetChild(0).position = new Vector2(Main.X * Screen.width, Main.Y * Screen.height);
                Main.mp.transform.GetChild(0).localScale = new Vector3(Main.S, Main.S, 1f);
                DeleteCheckPoint();

            }
            else
            {
                Main.mp.SetActive(false);
                isDeleteCheck = false;
            }
        }
        catch (Exception e) 
        {
            Debug.Log("=======================================");
            Debug.Log(e.Message);
            return;
        }

    }

    void DeleteCheckPoint()
    {
        if (isDeleteCheck) return;
        var checkPoints = FindObjectsOfType<ffxCheckpoint>();
        if (checkPoints == null) return;
        foreach (var checkPoint in checkPoints)
        {
            checkPoint.floor.floorIcon = FloorIcon.None;
        }
        isDeleteCheck = true;
    }
}