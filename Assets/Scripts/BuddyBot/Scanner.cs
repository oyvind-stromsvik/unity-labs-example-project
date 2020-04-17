using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Scanner : MonoBehaviour
{
    public Vector3 TargetPoint;

    public float BeamExtension;
    public float BeamWidthExtension;
    public float BeamHeightExtension;

    public float beamAngleHorizontal = 90f;
    public float beamAngleVertical = 45f;

    private Vector3[] m_Vertices;

    public LayerMask BlockingLayers;
    public Material ScannerMaterial;

    public bool SweepHorizontally;

    private Animator _animator;
    private Light _light;

    public float LightRangeMultiplier;

    private Mesh _mesh;
    private CommandBuffer _cbuf;
    private Camera _camera;

    private void OnEnable()
    {
        m_Vertices = new Vector3[3];

        _animator = GetComponent<Animator>();
        _light = GetComponentInChildren<Light>();
        _mesh = new Mesh {name = "Scanner mesh"};
        _cbuf = new CommandBuffer {name = "Draw scanner"};

        _mesh.vertices = m_Vertices;
        _mesh.subMeshCount = 2;
        _mesh.SetIndices(new[] {0, 1, 2}, MeshTopology.Triangles, 0);
        _mesh.SetIndices(new[] {1, 0, 2}, MeshTopology.LineStrip, 1);
    }

    private void OnDisable()
    {
        if (_cbuf != null)
        {
            if (_camera)
                _camera.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, _cbuf);
            _camera = null;

            _cbuf.Dispose();
        }
        _cbuf = null;

        DestroyImmediate(_mesh);
        _mesh = null;
    }

    private void Update()
    {
        if (!_camera)
        {
            _camera = Camera.main;
            if (_camera)
                _camera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, _cbuf);
        }

        var localTargetPoint = transform.InverseTransformPoint(TargetPoint);

        float extensionDist = localTargetPoint.z*BeamExtension;
        

        if (SweepHorizontally)
        {
            float width = Mathf.Tan(Mathf.Deg2Rad * beamAngleHorizontal * BeamHeightExtension / 2f) * extensionDist;
            float height = Mathf.Tan(Mathf.Deg2Rad * beamAngleVertical * BeamWidthExtension / 2f) * extensionDist;
            m_Vertices[1] = new Vector3(width, height, extensionDist);
            m_Vertices[2] = new Vector3(width, -height, extensionDist);
        }
        else
        {
            float width = Mathf.Tan(Mathf.Deg2Rad * beamAngleHorizontal * BeamWidthExtension / 2f) * extensionDist;
            float height = Mathf.Tan(Mathf.Deg2Rad * beamAngleVertical * BeamHeightExtension / 2f) * extensionDist;
            m_Vertices[1] = new Vector3(-width, height, extensionDist);
            m_Vertices[2] = new Vector3(width, height, extensionDist);
        }

        _mesh.vertices = m_Vertices;

        _cbuf.Clear();
        _cbuf.DrawMesh(_mesh, transform.localToWorldMatrix, ScannerMaterial, 0, 0);
        _cbuf.DrawMesh(_mesh, transform.localToWorldMatrix, ScannerMaterial, 1, 1);

        if (SweepHorizontally)
        {
            _light.transform.localRotation = Quaternion.AngleAxis(-90, Vector3.forward) * Quaternion.AngleAxis(beamAngleHorizontal * BeamHeightExtension * 0.5f, -Vector3.right);
        }
        else
        {
            _light.transform.localRotation = Quaternion.AngleAxis(beamAngleVertical * BeamHeightExtension * 0.5f, -Vector3.right);
        }

        if (BeamExtension > 0)
        {
            _light.enabled = true;
            _light.spotAngle = (SweepHorizontally ? beamAngleVertical : beamAngleHorizontal)*BeamWidthExtension;
            _light.range = LightRangeMultiplier*extensionDist;
        }
        else
        {
            _light.enabled = false;
        }
    }

    public void TurnOn(Vector3 position)
    {
        TargetPoint = position;

        _animator.SetBool("On", true);
    }

    public void TurnOff()
    {
        _animator.SetBool("On", false);
    }

    public void BeginScanning()
    {
        _animator.SetBool("Scanning", true);
    }

    public void EndScanning()
    {
        _animator.SetBool("Scanning", false);
    }
}