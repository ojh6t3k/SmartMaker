using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class GraphDrawer : MonoBehaviour
{
    [Serializable]
    public class Graph
    {
        public string name;
        public float[] data = null;
        public int start = 0;
        public int end = 0;
        public float min = 0;
        public float max = 0;
        public float cutoff = 0;
        public Color color = Color.white;
    }

    public bool display = true;
    public Material material;
    public List<Graph> graphs = new List<Graph>();

    private Camera _camera;    

    void Awake()
    {
        _camera = GetComponent<Camera>();
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnPostRender()
    {
        DrawGraph();
    }

    private void DrawGraph()
    {
        if (!display || graphs.Count == 0)
            return;

        float height = _camera.orthographicSize * 2f;
        float width = height * Screen.width / Screen.height;        
        Vector3 basePos = transform.position - (transform.right * width * 0.5f) - (transform.up * height * 0.5f) + (transform.forward * 10f);
        float unitY = height / graphs.Count;

        GL.Begin(GL.LINES);
        material.SetPass(0);

        for (int i = 0; i < graphs.Count; i++)
        {
            if (graphs[i].data == null)
                continue;

            int length = graphs[i].data.Length;
            if (length == 0)
                continue;

            GL.Color(graphs[i].color);

            float y = i * unitY;            
            int start = Mathf.Max(graphs[i].start, 0);
            int end = Mathf.Min(graphs[i].end, length - 1);
            if (start == end)
                end = length - 1;
            else
                length = end - start + 1;
            float unitX = width / length;

            float cutoff = Mathf.Abs(graphs[i].cutoff);
            float max = -10000f;
            float min = 10000f;
            for (int j = start; j <= end; j++)
            {
                float data = graphs[i].data[j];
                if (Mathf.Abs(data) >= cutoff)
                {
                    max = Mathf.Max(max, graphs[i].data[j]);
                    min = Mathf.Min(min, graphs[i].data[j]);
                }
            }
            graphs[i].min = Mathf.Min(graphs[i].min, min);
            graphs[i].max = Mathf.Max(graphs[i].max, max);
            float bias = min;
            float amplitude = Mathf.Abs(graphs[i].max - graphs[i].min);

            float x = 0f;
            for (int j= start; j<= end; j++)
            {
                float data = graphs[i].data[j];                
                Vector3 pos = (transform.right * x) + (transform.up * y);
                x += unitX;
                if (Mathf.Abs(data) >= cutoff)
                {
                    float yDiff = 0f;
                    if (amplitude > 0f)
                        yDiff = unitY * (data - bias) / amplitude;
                    pos.y += yDiff;
                }                    
                pos += basePos;

                GL.Vertex(pos);
                if(j > start && j < end)
                    GL.Vertex(pos);
            }
        }

        GL.End();        
    }
}
