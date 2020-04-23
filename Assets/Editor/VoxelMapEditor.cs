
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VoxelMap))]
public class VoxelMapEditor : Editor
{
    private VoxelMap m_target;

    void OnEnable()
    {
        m_target = (VoxelMap)target;
    }
 
    public override void OnInspectorGUI()
    {
		DrawDefaultInspector();
		if (GUILayout.Button("Generate VoxelMap")) {
			m_target.initVoxelMap();
		}
    }
}
