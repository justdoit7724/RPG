using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(MeshFilter))]
public class WeaponTrail : MonoBehaviour
{
    [SerializeField] Material trailMat;
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    [SerializeField] float lifeTime = 0.20f;

    private Mesh mesh;
    private const int maxVertCount = 320;
    private Vector3[] vertices = new Vector3[maxVertCount];
    private Color[] colors = new Color[maxVertCount];
    private Vector2[] uv = new Vector2[maxVertCount];
    private int[] triangles = new int[(maxVertCount - 2) * 3];

    private MeshRenderer meshRenderer;
    private bool isOn = false;

    struct TronTrailSection
    {
        public Vector3 startP, endP;
        public float time;
        public TronTrailSection(Vector3 sp, Vector3 ep, float t)
        {
            startP = sp;
            endP = ep;
            time = t;
        }
    }
    // 빠른 데이터 삭제를 위한 링크드리스트 사용
    private LinkedList<TronTrailSection> sections = new LinkedList<TronTrailSection>();

    void Awake()
    {

        MeshFilter meshF = GetComponent(typeof(MeshFilter)) as MeshFilter;
        mesh = meshF.mesh;
        meshRenderer = GetComponent(typeof(MeshRenderer)) as MeshRenderer;
        trailMat = new Material(trailMat);
        meshRenderer.material = trailMat;
        for (int i = 0; i < maxVertCount; ++i)
        {
            colors[i] = new Color(1, 1, 1, 0.2f);
        }

        SetColor(Color.white);
    }
    public void StartTrail()
    {

        isOn = true;
    }
    public void EndTrail()
    {
        isOn = false;
    }
    public void SetColor(Color c)
    {
        trailMat.SetColor("_TintColor", c);
    }


    void Update()
    {
        if(isOn)
            sections.AddLast(new TronTrailSection(startPoint.position, endPoint.position, Time.time));

        while (sections.Count > 0 && (Time.time - sections.First.Value.time) > lifeTime)
        {
            sections.RemoveFirst();
        }

        if (sections.Count < 2)
        {
            meshRenderer.enabled = false;
            return;
        }
        else
        {
            meshRenderer.enabled = true;
        }

        //
        // Use matrix instead of transform.TransformPoint for performance reasons
        Matrix4x4 localSpaceTransform = transform.worldToLocalMatrix;

        var itr = sections.First;
        for (var i = 0; i < sections.Count; i++)
        {

            TronTrailSection currentSection = itr.Value;

            float u = Mathf.Clamp01((Time.time - currentSection.time) / lifeTime);

            vertices[i * 2 + 0] = localSpaceTransform.MultiplyPoint(currentSection.startP);
            vertices[i * 2 + 1] = localSpaceTransform.MultiplyPoint(currentSection.endP);

            uv[i * 2 + 0] = new Vector2(u, 0);
            uv[i * 2 + 1] = new Vector2(u, 1);

            itr = itr.Next; 
        }

        int activeTriangleCount = sections.Count - 1;
        for (int i = 0; i < activeTriangleCount; i++)
        {
            triangles[i * 6 + 0] = i * 2;
            triangles[i * 6 + 1] = i * 2 + 1;
            triangles[i * 6 + 2] = i * 2 + 2;

            triangles[i * 6 + 3] = i * 2 + 2;
            triangles[i * 6 + 4] = i * 2 + 1;
            triangles[i * 6 + 5] = i * 2 + 3;
        }
        int lastActiveTriIdx = (sections.Count - 1) * 6 - 1;
        for (int i = lastActiveTriIdx + 1; i < triangles.Length; ++i) // hide spare trail spaces
        {
            triangles[i] = triangles[lastActiveTriIdx];
        }

        // Assign to mesh	
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
    public void ClearTrail()
    {
        if (mesh != null)
        {
            mesh.Clear();
            sections.Clear();
        }
    }
}


