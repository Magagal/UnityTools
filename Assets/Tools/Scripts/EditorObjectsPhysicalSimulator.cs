using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class EditorObjectsPhysicalSimulator : EditorWindow
{
#if UNITY_EDITOR

    bool canBeClosed = true;

    static bool cachedAutoSimulation;

    static SimulatedBody[] simulatedBodies;

    static bool active = false;

    Editor editor;

    [SerializeField] List<GameObject> ListGameObjectsToSimulate = new List<GameObject>();

    static List<GameObject> staticListGameObjects = new List<GameObject>();
    static List<Rigidbody> staticWorkList = new List<Rigidbody>();
    static List<Component> staticAddList = new List<Component>();

    static bool registered = false;

    static EditorObjectsPhysicalSimulator()
    {
        if (!registered)
        {
            EditorApplication.update += Update;

            registered = true;
        }

        StopSimulation();

        EditorWindow w = GetWindow<EditorObjectsPhysicalSimulator>();
        w.Close();
    }

    static void OnPlayModeStateChange(PlayModeStateChange state)
    {
        StopSimulation();
        GetWindow<EditorObjectsPhysicalSimulator>().Close();
    }

    [MenuItem("Automation/Objects Physical Simulator")]
    public static void Open()
    {
        GetWindow<EditorObjectsPhysicalSimulator>("Objects Physical Simulator", true);
       
    }

    void OnGUI()
    {
        staticListGameObjects = ListGameObjectsToSimulate;

        if (!editor) { editor = Editor.CreateEditor(this); }
        if (editor) { editor.OnInspectorGUI(); }

        Color cacheColor = GUI.color;

        GUI.color = active == true ? Color.green : Color.red;
        GUILayout.Label("Simulating Physics: " + (active == true ? "ENABLED" : "DISABLED"));

        GUI.color = cacheColor;

        GUI.enabled = (!active && ListGameObjectsToSimulate.Count > 0);

        if (GUILayout.Button("Run"))
        {
            StartSimulation();

        }

        GUI.enabled = active;

        if (GUILayout.Button("Stop"))
        {
            StopSimulation();

            GUI.enabled = active;
        }

        if (GUILayout.Button("Reset"))
        {
            StopSimulation();

            ResetSimulation();

            GUI.enabled = active;
        }

        

        Repaint();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChange;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
    }


    static void StartSimulation()
    {
        Debug.Log("StartSimulation");

        staticWorkList.Clear();
        staticAddList.Clear();

        for (int i = 0; i < staticListGameObjects.Count; i++)
        {
            if (staticListGameObjects[i] != null)
            {
                Rigidbody rb = staticListGameObjects[i].GetComponent<Rigidbody>();

                if (rb == null)
                {
                    rb = staticListGameObjects[i].AddComponent<Rigidbody>();
                    staticAddList.Add(rb);
                }
                if (staticListGameObjects[i].GetComponent<Collider>() == false)
                {
                    MeshCollider meshCollider = staticListGameObjects[i].AddComponent<MeshCollider>();
                    meshCollider.convex = true;
                    staticAddList.Add(meshCollider);
                }

                staticWorkList.Add(rb);
            }
        }

        for (int i = 0; i < staticWorkList.Count; i++)
        {
            staticWorkList[i].WakeUp();
        }

        simulatedBodies = FindObjectsOfType<Rigidbody>().Select(rb => new SimulatedBody(rb, staticListGameObjects.Contains(rb.gameObject))).ToArray();

        active = true;
        Physics.autoSimulation = false;
    }

    static void StopSimulation()
    {
        Debug.Log("StopSimulation");

        active = false;
        Physics.autoSimulation = true;

        for (int i = 0; i < staticWorkList.Count; i++)
        {
            if (staticWorkList[i] != null)
            {
                staticWorkList[i].Sleep();
            }
        }

        for (int i = 0; i < staticAddList.Count; i++)
        {
            DestroyImmediate(staticAddList[i]);
        }

        staticWorkList.Clear();
        staticAddList.Clear();
    }

    public void ResetSimulation()
    {
        Debug.Log("ResetSimulation");

        foreach (SimulatedBody body in simulatedBodies)
        {
            if (body.inList == true)
            {
                body.Reset();
            }
        }
    }

    static void Update()
    {
        UnityEditor.EditorApplication.QueuePlayerLoopUpdate();

        if (active)
        {
            Physics.Simulate(Time.fixedDeltaTime);

            foreach (SimulatedBody body in simulatedBodies)
            {
                if (body.inList == false)
                {
                    body.Reset();
                }
            }
        }
    }

    static double editorDeltaTime = 0f;
    static double lastTimeSinceStartup = 0f;

    static void SetEditorDeltaTime()
    {
        if (lastTimeSinceStartup == 0f)
        {
            lastTimeSinceStartup = EditorApplication.timeSinceStartup;
        }
        editorDeltaTime = EditorApplication.timeSinceStartup - lastTimeSinceStartup;
        lastTimeSinceStartup = EditorApplication.timeSinceStartup;
    }


    struct SimulatedBody
    {
        public readonly Rigidbody rigidbody;
        readonly Transform transform;
        public readonly Vector3 originalPosition;
        public readonly Quaternion originalRotation;
        public readonly bool inList;

        public SimulatedBody(Rigidbody rigidbody, bool _inList) : this()
        {
            this.rigidbody = rigidbody;
            transform = rigidbody.transform;
            originalPosition = rigidbody.position;
            originalRotation = rigidbody.rotation;
            inList = _inList;
        }

        public void Reset()
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;

            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
        }
    }

    void OnDestroy()
    {
        if (!canBeClosed)
        {
            RescueContent();
        }
            
        StopSimulation();
    }
    void RescueContent()
    {
        var newWin = Instantiate<EditorObjectsPhysicalSimulator>(this);
        newWin.ListGameObjectsToSimulate = ListGameObjectsToSimulate;
        newWin.Show();
    }
}

#endif