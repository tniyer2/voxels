
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VoxelMap : MonoBehaviour
{
	private const uint BLOCK_SIZE = 16;

	public float originX = 0;
	public float originY = 0;
	public float scale = 1;

	private VoxelGrid grid;

	public void initVoxelMap()
	{
		this.grid = new VoxelGrid(BLOCK_SIZE);

		int size = this.grid.getSize();
		for (int i = 0; i < size; ++i) {
			for (int j = 0; j < size; ++j) {
				/*
				float xCoord = originX + (((float)i / size) * scale);
				float yCoord = originY + (((float)j / size) * scale);
				float val = Mathf.PerlinNoise(xCoord, yCoord) * size;
				*/
				for (int k = 0; k < size; ++k) {
					float val = Mathf.Pow(i-(scale/2), 2) + Mathf.Pow(j-(scale/2), 2) + Mathf.Pow(k-(scale/2), 2) - scale;
					this.grid.setVoxel(i, j, k, val);
				}
			}
		}

		GetComponent<MeshFilter>().mesh = this.grid.genMesh();
	}
}
