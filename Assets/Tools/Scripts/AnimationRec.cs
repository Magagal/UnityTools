using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AnimationRec : MonoBehaviour
{
    string pathSave;

    [Header("Press I to Start Record", order = 0)]
    [Header("Press O to Save Record", order = 1)]
    [Header("Press P to Cancel Record", order = 2)]
    [Space]
    [Header("Save: Application.dataPath/FolderPathSave/NameAnimClip.anim", order = 3)]
    public string folderPathSave = "AnimationClips";
    public string nameAnimClip = "animation";

    [SerializeField] [Range(0.1f, 1f)]float recordFrequency = 0.25f;

    private GameObjectRecorder m_Record;

    void Start()
    {
        pathSave = Application.dataPath + "/" + folderPathSave + "/";

        if (!Directory.Exists(pathSave))
        {
            Debug.Log($"CreateDirectory: {folderPathSave}");
            Directory.CreateDirectory(pathSave);
        }
    }

    public void StartRecord()
    {
        Debug.Log($"Start Recorder");
        m_Record = new GameObjectRecorder(gameObject);
        m_Record.BindComponentsOfType<Transform>(gameObject, true);
    }

    public void CancelRecord()
    {
        Debug.Log($"Cancel Recorder");

        m_Record = null;
    }

    public void SaveName()
    {
        if (m_Record != null && m_Record.isRecording)
        {
            int incrementNumeber = 0;

            AnimationClip animationClip = new AnimationClip();
            m_Record.SaveToClip(animationClip);
            m_Record = null;         

            if (System.IO.File.Exists(pathSave + nameAnimClip + ".anim"))
            {
                for (int i = 0; i < Mathf.Infinity; i++)
                {
                    incrementNumeber++;

                    if (!System.IO.File.Exists(pathSave + nameAnimClip + incrementNumeber.ToString("D3") + ".anim"))
                    {
                        animationClip.name = nameAnimClip + incrementNumeber.ToString("D3");
                        break;
                    }
                }
            }
            else
            {
                animationClip.name = nameAnimClip;
            }

            string relativepath = "Assets" + pathSave.Substring(Application.dataPath.Length);
            AssetDatabase.CreateAsset(animationClip, relativepath + animationClip.name + ".anim");

            Debug.Log($"Save Record: {animationClip.name + ".anim"}");
        }
    }

    void LateUpdate()
    {
        if (m_Record == null)
            return;

        m_Record.TakeSnapshot(Time.deltaTime * recordFrequency);
    }

    public void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            StartRecord();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveName();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            CancelRecord();
        }
    }

    private void Update()
    {
        CheckInputs();
    }
}
