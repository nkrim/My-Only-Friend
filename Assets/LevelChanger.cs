using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[ExecuteInEditMode]
public class LevelChanger : MonoBehaviour
{
    public int firstLevelIndex = 0;

    [HideInInspector]
    public LevelMarker prev_level = null;
    [HideInInspector]
    public LevelMarker cur_level = null;

    private Transform level_list = null;
    private int prev_first_level_index = 0;

    private void Awake () {
        level_list = GameObject.FindWithTag("LevelList").transform;
        prev_level = level_list.GetChild(firstLevelIndex).GetComponent<LevelMarker>();
        cur_level = prev_level;
    }

    private void Start () {
        prev_first_level_index = firstLevelIndex;
        cur_level.GetComponent<CinemachineVirtualCamera>().Priority = 100;
        cur_level.Restart();
    }

    public void Update () {
        // EDITOR ONLY
        if(firstLevelIndex != prev_first_level_index) {
            prev_first_level_index = firstLevelIndex;
            prev_level = cur_level;
            cur_level = level_list.GetChild(firstLevelIndex).GetComponent<LevelMarker>();
            cur_level.GetComponent<CinemachineVirtualCamera>().Priority = 100;
            prev_level.GetComponent<CinemachineVirtualCamera>().Priority = 10;
            cur_level.Restart();
        }

        if (Input.GetKeyDown(KeyCode.R))
            cur_level.Restart();
    }
}
