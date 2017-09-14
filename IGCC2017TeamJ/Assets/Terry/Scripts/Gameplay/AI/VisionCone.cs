using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class VisionCone {

    [SerializeField]
    private List<string> obstacleTags;

    [SerializeField, Range(0.0f, 180.0f)]
    private float viewAngle = 45.0f; //角度
    [SerializeField, Range(0, 9999999)]
    private float viewDistance = 5.0f; //距離

    [SerializeField, Range(6, 999999)]
    private int numSlices = 12; // For the sides of the cone.
    [SerializeField, Range(3, 999999)]
    private int numDivisions = 4; // For the base of the cone.
    [SerializeField]
    private Material coneMaterial = null;
    [SerializeField, Range(0.0f, 1.0f)]
    private float coneAlpha = 0.4f;

    private MeshFilter coneMeshFilter = null;
    private MeshRenderer coneMeshRenderer = null;

    public VisionCone(MeshFilter _meshFilter, MeshRenderer _meshRender, bool _createVisionConeMesh = false) {
        coneMeshFilter = _meshFilter;
        coneMeshRenderer = _meshRender;

        if (_createVisionConeMesh) {
            CreateVisionConeMesh();
        }
    }

    public VisionCone() {
    }

    public List<string> GetObstacleTags() {
        return obstacleTags;
    }

    public void SetViewAngle(float _viewAngle, bool _createVisionConeMesh = false) {
        viewAngle = Mathf.Clamp(_viewAngle, 0.0f, 180.0f);

        if (_createVisionConeMesh) {
            CreateVisionConeMesh();
        }
    }

    public float GetViewAngle() {
        return viewAngle;
    }

    public void SetViewDistance(float _viewDistance, bool _createVisionConeMesh = false) {
        viewDistance = Mathf.Max(0.0f, _viewDistance);

        if (_createVisionConeMesh) {
            CreateVisionConeMesh();
        }
    }

    public float GetViewDistance() {
        return viewDistance;
    }

    public void SetMeshFilter(MeshFilter _meshFilter, bool _createVisionConeMesh = false) {
        coneMeshFilter = _meshFilter;

        if (_createVisionConeMesh) {
            CreateVisionConeMesh();
        }
    }

    public MeshFilter GetMeshFilter() {
        return coneMeshFilter;
    }

    public void SetMeshRenderer(MeshRenderer _meshRenderer, bool _createVisionConeMesh = false) {
        coneMeshRenderer = _meshRenderer;

        if (_createVisionConeMesh) {
            CreateVisionConeMesh();
        }
    }

    public MeshRenderer GetMeshRenderer() {
        return coneMeshRenderer;
    }

    public void SetNumSlices(int _numSlices, bool _createVisionConeMesh = false) {
        numSlices = Mathf.Max(6, _numSlices);

        if (_createVisionConeMesh) {
            CreateVisionConeMesh();
        }
    }
    
    public int GetNumSlices() {
        return numSlices;
    }

    public void SetNumDivisions(int _numDivision, bool _createVisionConeMesh = false) {
        numDivisions = Mathf.Max(3, _numDivision);

        if (_createVisionConeMesh) {
            CreateVisionConeMesh();
        }
    }

    public int GetNumDivisions() {
        return numDivisions;
    }

    public void SetMaterial(Material _material, bool _createVisionConeMesh = false) {
        coneMaterial = _material;

        if (_createVisionConeMesh) {
            CreateVisionConeMesh();
        }
    }

    public Material GetMaterial() {
        return coneMaterial;
    }

    public float GetConeAlpha() {
        return coneAlpha;
    }

    public float AngleToTarget(GameObject source, GameObject target) {
        if (target == null || source == null) {
            return -1.0f;
        }

        Vector3 targetDirection = target.transform.position - source.transform.position;
        Vector3.Normalize(targetDirection);

        return Mathf.Acos(Vector3.Dot(source.transform.forward, targetDirection)) * Mathf.Rad2Deg;
    }

    public float DistanceToTarget(GameObject source, GameObject target) {
        if (target == null || source == null) {
            return -1.0f;
        }

        return (source.transform.position - target.transform.position).magnitude;
    }

    public float DistanceToTargetSqr(GameObject source, GameObject target) {
        if (target == null || source == null) {
            return -1.0f;
        }

        return (source.transform.position - target.transform.position).sqrMagnitude;
    }

    public bool IsTargetInVisionCone(GameObject source, GameObject target) {
		if (source == null || target == null) {
            //Debug.Log("VisionCone::IsTargetInVisionCone - Source or Target is null!");
            return false;
        }

        // Distance Check 距離
        Vector3 targetDirection = target.transform.position - source.transform.position;
        if (targetDirection.sqrMagnitude > viewDistance * viewDistance) {
            //Debug.Log("VisionCone::IsTargetInVisionCone - Target is too far!");
            return false;
        } else {
            //Debug.Log(targetDirection.magnitude);
        }

        // Angle Check 角度
        targetDirection.Normalize();
        float dotProduct = Vector3.Dot(source.transform.forward, targetDirection);
        float angleTotarget = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;
        if (angleTotarget > viewAngle) {
            //Debug.Log("VisionCone::IsTargetInVisionCone - Target's name is " + target.name + ".");
            //Debug.Log("VisionCone::IsTargetInVisionCone - Target is outside view angle!");
            //Debug.Log("VisionCone::IsTargetInVisionCone - Current Angle To Target is " + angleTotarget + " Degrees");
            //Debug.Log("VisionCone::IsTargetInVisionCone - Direction To Target is " + targetDirection);
            //Debug.Log("VisionCone::IsTargetInVisionCone - Source's Forward Vector is " + source.transform.forward);
            return false;
        }

        // Check that there is nothing blocking the view.
        float raycastDistance = (target.transform.position - source.transform.position).magnitude;
        RaycastHit[] result = Physics.RaycastAll(source.transform.position, targetDirection, raycastDistance);
        for (int i = 0; i < result.Length; ++i) {
            Collider hitCollider = result[i].collider;
            GameObject hitGameObject = hitCollider.gameObject;

            for (int j = 0; j < obstacleTags.Count; ++j) {
                if (hitGameObject.tag == obstacleTags[j]) {
                    return false;
                }
            }
        }

        return true;
	}

    public void CreateVisionConeMesh() {
        CreateVisionConeMesh(coneMeshFilter, coneMeshRenderer);
    }

    public void CreateVisionConeMesh(MeshFilter _meshFilter, MeshRenderer _meshRenderer) {
        //Debug.Log("VisionCone::CreateVisionConeMesh");

        if (_meshRenderer == null || _meshFilter == null) {
            Debug.Log("VisionCone::CreateVisionConeMesh - MeshRenderer or MeshFilter is missing!");
            return;
        }

        if (numSlices < 0) {
            Debug.Log("VisionCone::CreateVisionConeMesh - Seriously? " + numSlices + " slices? Nope! It's gonna be... erm... 12 slices. Yep. 12. Cause I said so.");
            numSlices = 12;
        } else if (numSlices < 3) {
            Debug.Log("VisionCone::CreateVisionConeMesh - What the fuck? How can I possible create a cone looking thing with only " + numSlices + " slices!");
            Debug.Log("VisionCone::CreateVisionConeMesh - I need at least 3 slices to make a pyramid looking thing. Defaulting to 3 slices.");
            numSlices = 3;
        }

        //Vector2[] uvs = { new Vector2() }; // No UVs. No fucking way I'm supporting textures for this shit.
        List<Vector3> vertexList = new List<Vector3>();
        List<int> triangleList = new List<int>();

        // From the side view, View Distance is our radius.
        // Starting Vertex
        vertexList.Add(new Vector3(0.0f, 0.0f, viewDistance));

        // This is how much the angle increases with each ring of the base.
        float angleDifference = (viewAngle * Mathf.Deg2Rad) / numDivisions;
        // What we're gonna do is just draw the base of the cone with decreasing radius length until we hit the middle.
        // We have to start from one because we will always need 1 more ring than division.
        // We start from 1 because the first ring is at the exact middle.
        for (int i = 1; i < numDivisions + 1; ++i) {
            // CAREFUL! Everything beyond this point is in radians and not degrees!
            // Don't fuck it up!

            // This angle is the angle from the side.
            float angle = angleDifference * (float)i;

            // This radius is the radius from the front.
            float radius = viewDistance * Mathf.Sin(angle);
            float z = viewDistance * Mathf.Cos(angle);
            float sliceAngle = (2 * Mathf.PI) / numSlices;

            // Drawing the circle (Front view).
            for (int j = 0; j < numSlices; ++j) {
                float currentSliceAngle = sliceAngle * (float)j;
                Vector3 vertex = new Vector3(Mathf.Cos(currentSliceAngle) * radius,
                                             Mathf.Sin(currentSliceAngle) * radius,
                                             z);

                vertexList.Add(vertex);
            }
        }
        
        // The final vertex.
        vertexList.Add(new Vector3());

        /*for (int i = 0; i < vertexList.Count; ++i) {
            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.AddComponent<VertexIndex>().vertexIndex = i;
            point.transform.position = vertexList[i];
            point.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        }*/

        // Time to do the triangles.
        // Inner most ring of the front.
        for (int i = 0; i < numSlices; ++i) {
           triangleList.Add(0);
           triangleList.Add((i + 1) % numSlices + 1);
           triangleList.Add((i + 2) % numSlices + 1);
        }

        // The subsequent frontal layers.
        for (int i = 0; i < numDivisions - 1; ++i) {
            for (int j = 0; j < numSlices; ++ j) {
                int vertex0 = j + 1 + (numSlices * i);
                int vertex1 = vertex0 + numSlices;
                int vertex2 = vertex1 + 1;
                if (vertex2 > (i + 2) * numSlices) {
                    vertex2 -= numSlices;
                }

                triangleList.Add(vertex0);
                triangleList.Add(vertex1);
                triangleList.Add(vertex2);

                int vertex3 = vertex0;
                int vertex4 = vertex2;
                int vertex5 = vertex3 + 1;
                if (vertex5 > (i + 1) * numSlices) {
                    vertex5 -= numSlices;
                }

                triangleList.Add(vertex3);
                triangleList.Add(vertex4);
                triangleList.Add(vertex5);
            }
        }

        // Now finally on to the final part. The cone sides.
        // What we need is the last vertex + the numSlices of vertex before it.
        {
            int startVertex = vertexList.Count - (numSlices + 1);
            int lastVertex = vertexList.Count - 1;

            for (int i = 0; i < numSlices; ++i) {
                triangleList.Add(lastVertex);

                int vertex = startVertex + 1 + i;
                if (vertex == lastVertex) {
                    vertex = startVertex;
                }
                triangleList.Add(vertex);
                triangleList.Add(startVertex + i);
            }
        }

        // Clear the current mesh.
        _meshFilter.mesh.Clear();

        // Copy our vertexList into a non dynamic array.
        Vector3[] vertices = new Vector3[vertexList.Count];
        for (int i = 0; i < vertexList.Count; ++i) {
            vertices[i] = vertexList[i];
        }

        // Copy our triangleList into a non dynamic array.
        int[] triangles = new int[triangleList.Count];
        for (int i = 0; i < triangleList.Count; ++i) {
            triangles[i] = triangleList[i];
        }

        //_meshFilter.mesh.uv = uvs;
        _meshFilter.mesh.vertices = vertices;
        _meshFilter.mesh.triangles = triangles;

        _meshRenderer.material = coneMaterial;

        Color coneColor = _meshRenderer.material.color;
        coneColor.a = coneAlpha;
        _meshRenderer.material.color = coneColor;
    }

    public GameObject CreateVisionConeObject(GameObject _source, bool _setAsMeshFilter = true, bool _setAsMeshRenderer = true) {
        GameObject visionConeObject = new GameObject();
        visionConeObject.name = _source.name + "のVision Cone";

        visionConeObject.transform.position = _source.transform.position;
        visionConeObject.transform.rotation = _source.transform.rotation;

        visionConeObject.transform.parent = _source.transform;

        visionConeObject.AddComponent<MeshFilter>();
        visionConeObject.AddComponent<MeshRenderer>();
        CreateVisionConeMesh(visionConeObject.GetComponent<MeshFilter>(), visionConeObject.GetComponent<MeshRenderer>());

        if (_setAsMeshFilter) {
            coneMeshFilter = visionConeObject.GetComponent<MeshFilter>();
        }

        if (_setAsMeshRenderer) {
            coneMeshRenderer = visionConeObject.GetComponent<MeshRenderer>();
        }

        return visionConeObject;
    }

}