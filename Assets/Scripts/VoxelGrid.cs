
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VoxelGrid
{
	private int size;
	private float[,,] voxels;

	public VoxelGrid(uint size) {
		this.size = (int)size;
		this.voxels = new float[this.size, this.size, this.size];
	}

	public int getSize() {
		return this.size;
	}

	public float getVoxel(int x, int y, int z) {
		return this.voxels[x, y, z];
	}

	public void setVoxel(int x, int y, int z, float val) {
		this.voxels[x, y, z] = val;
	}

	private ushort getIndex(ushort[,,] table, List<Vector3> verts, int i, int j, int k) {
		ushort index = table[i, j, k];

		if (true || index == 0) {
			verts.Add(new Vector3((float)i, (float)j, (float)k));
			index = (ushort)verts.Count;
			table[i, j, k] = index;
		}

		return (ushort)(index - 1);
	}

	private void calcUVs(Mesh mesh) {
		Vector2[] expanded = Unwrapping.GeneratePerTriangleUV(mesh);
		Vector2[] merged = new Vector2[mesh.vertices.Length];

		for (int i = 0; i < expanded.Length; ++i) {
			int j = mesh.triangles[i];
			merged[j] = expanded[i];
		}

		mesh.uv = merged;
	}

	private void calcFaces(int axis, int nCells, ushort[,,] indexTable, List<Vector3> verts, List<int> tris) {
		int[] df;
		int[,] df2;
		switch (axis) {
			case 0:
				df = new int[]{1, 0, 0};
				df2 = new int[,]{{0, -1, 0}, {0, -1, -1}, {0, 0, -1}};
				break;
			case 1:
				df = new int[]{0, 1, 0};
				df2 = new int[,]{{0, 0, -1}, {-1, 0, -1}, {-1, 0, 0}};
				break;
			case 2:
				df = new int[]{0, 0, 1};
				df2 = new int[,]{{-1, 0, 0}, {-1, -1, 0}, {0, -1, 0}};
				break;
			default:
				throw new ArgumentException("Invalid Argument axis: " + axis);
		}

		for (int i = 1; i < nCells; ++i) {
			for (int j = 1; j < nCells; ++j) {
				for (int k = 1; k < nCells; ++k) {
					float a = this.voxels[i, j, k];
					float b = this.voxels[i+df[0], j+df[1], k+df[2]];

					if ((a < 0) != (b < 0)) {
						ushort l1 = this.getIndex(indexTable, verts, i, j, k);
						ushort l2 = this.getIndex(indexTable, verts, i+df2[0,0], j+df2[0,1], k+df2[0,2]);
						ushort l3 = this.getIndex(indexTable, verts, i+df2[1,0], j+df2[1,1], k+df2[1,2]);
						ushort l4 = this.getIndex(indexTable, verts, i+df2[2,0], j+df2[2,1], k+df2[2,2]);
						if (a < 0) {
							tris.Add(l1);
							tris.Add(l2);
							tris.Add(l3);

							tris.Add(l3);
							tris.Add(l4);
							tris.Add(l1);
						} else {
							tris.Add(l1);
							tris.Add(l3);
							tris.Add(l2);

							tris.Add(l3);
							tris.Add(l1);
							tris.Add(l4);
						}
					}
				}
			}
		}
	}

	public Mesh genMesh() {
		int nCells = this.size - 1;

		ushort[,,] indexTable = new ushort[nCells, nCells, nCells];

		List<Vector3> verts = new List<Vector3>();
		List<int> tris = new List<int>();

		for (int axis = 0; axis < 3; ++axis) {
			this.calcFaces(axis, nCells, indexTable, verts, tris);
		}

		Mesh mesh = new Mesh();
		mesh.vertices = verts.ToArray();
		mesh.triangles = tris.ToArray();
		this.calcUVs(mesh);
		mesh.RecalculateNormals();
		
		return mesh;
	}
}
