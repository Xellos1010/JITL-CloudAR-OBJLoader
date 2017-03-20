// ObjReader version 2.4
// ©2015 Starscene Software. All rights reserved. Redistribution without permission not allowed.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

[AddComponentMenu("ObjReader/ObjReader")]
public class ObjReader : MonoBehaviour {
	public int maxPoints = 30000;						// Use this to limit maximum vertices of objects. Clamped to 65534
	public bool combineMultipleGroups = true;			// If true, .obj files containing more than one object will be treated as one object
	public bool useSubmeshesWhenCombining = true;		// If true, combined objects use submeshes, instead of a single mesh with one material
	public bool useFileNameAsObjectName = false;		// If true, the GameObject name is taken from the .obj file name instead of the group name
	public bool computeTangents = false;				// Needed if using normal-mapped shaders
	public bool useSuppliedNormals = false;				// If false, normals are calculated by Unity instead of using the normals in the .obj file
	public bool overrideDiffuse = false;				// If true, the color in the material will be used instead of the Kd color in the MTL file
	public bool overrideSpecular = false;				// If true, the specular color in the material will be used instead of the Ks color in the MTL file
	public bool overrideAmbient = false;				// If true, the emissive color in the material will be used instead of the Ka color in the MTL file
	public bool suppressWarnings = false;				// If true, no warnings will be output, although errors will still be printed
	public bool useMTLFallback = false;					// If true, the standard material will be used in case the mtl file is specified by the obj file but is missing
	public bool autoCenterOnOrigin = false;				// Move meshes so they are centered on (0, 0, 0)
	public Vector3 scaleFactor = new Vector3(1, 1, 1);	// Scale meshes by this amount when they are converted
	public Vector3 objRotation = new Vector3(0, 0, 0);	// Rotate meshes by this amount when they are converted
	public Vector3 objPosition = new Vector3(0, 0, 0);	// Move meshes by this amount when they are converted
	
	private static ObjReader _use = null;
	public static ObjReader use {
		get {
			if (_use == null) {
				_use = FindObjectOfType (typeof(ObjReader)) as ObjReader;
			}
			return _use;
		}
	}
	
	void Awake() {
		// Only one instance of ObjReader should exist
		if (FindObjectsOfType (typeof(ObjReader)).Length > 1) {
			Destroy (this);
			return;
		}
		DontDestroyOnLoad (this);
		
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}
	
	public GameObject[] ConvertString (string objString) {
		var mtlString = "";
		return Convert (ref objString, ref mtlString, null, null, "", false, "");
	}

	public GameObject[] ConvertString (string objString, string mtlString) {
		return Convert (ref objString, ref mtlString, null, null, "", true, "");
	}

	public GameObject[] ConvertString (string objString, Material standardMaterial) {
		var mtlString = "";
		return Convert (ref objString, ref mtlString, standardMaterial, null, "", false, "");
	}
	
	public GameObject[] ConvertString (string objString, string mtlString, Material standardMaterial) {
		return Convert (ref objString, ref mtlString, standardMaterial, null, "", true, "");
	}

	public GameObject[] ConvertString (string objString, string mtlString, Material standardMaterial, Material transparentMaterial) {
		return Convert (ref objString, ref mtlString, standardMaterial, transparentMaterial, "", true, "");
	}
	
	public GameObject[] ConvertFile (string objFilePath, bool useMtl) {
		return ConvertFile (objFilePath, useMtl, null, null, null);
	}
	
	public GameObject[] ConvertFile (string objFilePath, bool useMtl, Material standardMaterial) {
		return ConvertFile (objFilePath, useMtl, standardMaterial, null, null);
	}

	public GameObject[] ConvertFile (string objFilePath, bool useMtl, Material standardMaterial, Material transparentMaterial) {
		return ConvertFile (objFilePath, useMtl, standardMaterial, transparentMaterial, null);
	}
	
	public ObjData ConvertFileAsync (string objFilePath, bool useMtl) {
		return ConvertFileAsync (objFilePath, useMtl, null, null);
	}
	
	public ObjData ConvertFileAsync (string objFilePath, bool useMtl, Material standardMaterial) {
		return ConvertFileAsync (objFilePath, useMtl, standardMaterial, null);
	}
	
	public ObjData ConvertFileAsync (string objFilePath, bool useMtl, Material standardMaterial, Material transparentMaterial) {
		var objData = new ObjData();
		ConvertFile (objFilePath, useMtl, standardMaterial, transparentMaterial, objData);
		return objData;
	}
	
	GameObject[] ConvertFile (string objFilePath, bool useMtl, Material standardMaterial, Material transparentMaterial, ObjData objData) {
		objFilePath = objFilePath.Replace ('\\', '/');
		
		string objFile = "";
		if (objData == null) {
#if !UNITY_WEBPLAYER && !UNITY_WEBGL
			if (!File.Exists (objFilePath)) {
				Debug.LogError ("File not found: " + objFilePath);
				return null;
			}
			objFile = File.ReadAllText (objFilePath);
			if (objFile == null) {
				Debug.LogError ("File not usable: " + objFilePath);
				return null;
			}
			
#else
			Debug.LogWarning ("ConvertFile is not supported in the web player. Use ConvertFileAsync, or switch to a different platform.");
			return null;
#endif
		}
		else {
			if (objFilePath.StartsWith("http://") || objFilePath.StartsWith("https://") || objFilePath.StartsWith("file://") || objFilePath.StartsWith("ftp://")) {
				StartCoroutine (GetWWWFiles (objFilePath, useMtl, standardMaterial, transparentMaterial, objData));
			}
			else {
				Debug.LogError ("File path must start with http://, https://, ftp://, or file://");
			}
			return null;
		}
		
		string mtlFile = "";
		string filePath = "";
		if (useMtl) {
#if !UNITY_WEBPLAYER && !UNITY_WEBGL
			string mtlFileName = GetMTLFileName (ref objFilePath, ref objFile, ref filePath);
			// Read in mtl file
			if (File.Exists (filePath + mtlFileName)) {
				mtlFile = File.ReadAllText (filePath + mtlFileName);
			}
			else {
				Debug.LogWarning ("MTL file not found: " + (filePath + mtlFileName));
			}
#endif
		}
		return Convert (ref objFile, ref mtlFile, standardMaterial, transparentMaterial, filePath, useMtl, Path.GetFileNameWithoutExtension (objFilePath));
	}
	
	private GameObject[] Convert (ref string objFile, ref string mtlFile, Material standardMaterial, Material transparentMaterial, string filePath, bool useMtl, string fileName) {
		// Parse MTL file, if present
		Dictionary<string, Material> materials = null;
		if (useMtl && mtlFile != "") {
			string[] lines;
			Dictionary<string, Texture2D> textures = GetTextures (ref mtlFile, out lines, filePath);
			materials = ParseMTL (lines, standardMaterial, transparentMaterial, filePath, textures);
			if (materials == null) {
				useMtl = false;
			}
		}
		return CreateObjects (ref objFile, useMtl, materials, standardMaterial, null, fileName);
	}
	
	private GameObject[] CreateObjects (ref string objFile, bool useMtl, Dictionary<string, Material> materials, Material standardMaterial, ObjData objData, string fileName) {
		// Set up obj file
		string[] fileLines = objFile.Split ('\n');
		
		int totalVertices = 0;
		int totalUVs = 0;
		int totalNormals = 0;
	
		var foundF = false;
		var foundGroupStart = false;
		int firstFCount = 0;
		int groupCount = 0;
		var originalGroupIndices = new List<int>();
		int faceCount = 0;
		maxPoints = Mathf.Clamp (maxPoints, 0, 65534);
		
		// Find number of groups in file; also find total number of vertices, normals, and uvs
		for (int i = 0; i < fileLines.Length; i++) {
			if (fileLines[i].Length < 2) continue;
			
			char char1 = System.Char.ToLower(fileLines[i][0]);
			char char2 = System.Char.ToLower(fileLines[i][1]);
			if (char1 == 'f' && char2 == ' ') {
				if (!foundF) {
					firstFCount = faceCount;
				}
				faceCount++;
				if (foundGroupStart && !foundF) {
					groupCount++;
					originalGroupIndices.Add (firstFCount);
					foundGroupStart = false;
				}
				foundF = true;
			}
			else if ((char1 == 'o' && char2 == ' ') || (char1 == 'g' && char2 == ' ') || (char1 == 'u' && char2 == 's')) {	// "us" == "usemtl"
				foundGroupStart = true;
				foundF = false;
			}
			else if (char1 == 'v' && char2 == ' ') {
				totalVertices++;
			}
			else if (char1 == 'v' && char2 == 't') {
				totalUVs++;
			}
			else if (useSuppliedNormals && char1 == 'v' && char2 == 'n') {
				totalNormals++;
			}
		}
		
		if (groupCount == 0) {
			originalGroupIndices.Add (firstFCount);
			groupCount = 1;
		}
		
		if (totalVertices == 0) {
			Debug.LogError ("No vertices found in file");
			return null;
		}
		if (faceCount == 0) {
			Debug.LogError ("No face data found in file");
			return null;
		}
				
		originalGroupIndices.Add (-1);
		
		int verticesCount = 0;
		int uvsCount = 0;
		int normalsCount = 0;
		
		var objVertices = new Vector3[totalVertices];
		var objUVs = new Vector2[totalUVs];
		var objNormals = new Vector3[totalNormals];
		var triData = new List<string>();
		var quadWarning = false;
		var polyWarning = false;
		var objectNames = new string[groupCount];
		var materialNames = new string[groupCount];
		int index = 0;
		var lineInfo = new string[0];
		var groupIndices = new int[groupCount+1];
		int numberOfGroupsUsed = 0;
		faceCount = 0;
		groupCount = 0;
		int mtlCount = 0;
		int objectNamesCount = 0;
		
		try {
			while (index < fileLines.Length) {
				var line = fileLines[index++];
				
				// Skip over comments and short lines
				if (line.Length < 3 || line[0] == '#') continue;
				
				// Remove excess whitespace
				CleanLine (ref line);
				// Skip over short lines (again, in the off chance the above line made this line too short)
				if (line.Length < 3) continue;
				
				// If line ends with "\" then combine with the next line, assuming there is one (should be, but just in case)
				while (line[line.Length-1] == '\\' && index < fileLines.Length) {
					line = line.Substring (0, line.Length-1) + " " + fileLines[index++].TrimEnd();
					CleanLine (ref line);
				}
				
				char char1 = System.Char.ToLower(line[0]);
				char char2 = System.Char.ToLower(line[1]);
				// Get material name from usemtl line, plus object name if it doesn't have one from g or o lines
				if (char1 == 'u' && char2 == 's') {
					if (useMtl && line.StartsWith ("usemtl") && mtlCount++ == 0) {
						lineInfo = line.Split (' ');
						if (lineInfo.Length > 1) {
							materialNames[groupCount] = lineInfo[1];
							if (objectNamesCount++ == 0) {
								if (useFileNameAsObjectName && fileName != "") {
									objectNames[groupCount] = fileName;
								}
								else {
									objectNames[groupCount] = lineInfo[1];
								}
							}
						}
					}
				}
				// Get object name
				else if (((char1 == 'o' && char2 == ' ') || (char1 == 'g' && char2 == ' ')) && objectNamesCount++ == 0) {
					if (useFileNameAsObjectName && fileName != "") {
						objectNames[groupCount] = fileName;
					}
					else {
						objectNames[groupCount] = line.Substring (2, line.Length-2);
					}
				}
				// Read vertices
				else if (char1 == 'v' && char2 == ' ') {
					lineInfo = line.Split (' ');
					if (lineInfo.Length != 4) {
						throw new System.Exception ("Incorrect number of points while trying to read vertices:\n" + line + "\n");
					}
					else {
						// Invert x value so it works properly in Unity (left-handed)
						objVertices[verticesCount++] = new Vector3(-float.Parse (lineInfo[1], CultureInfo.InvariantCulture), float.Parse (lineInfo[2], CultureInfo.InvariantCulture), float.Parse (lineInfo[3], CultureInfo.InvariantCulture));
					}
				}
				// Read UVs
				else if (char1 == 'v' && char2 == 't') {
					lineInfo = line.Split (' ');
					if (lineInfo.Length > 4 || lineInfo.Length < 3) {
						throw new System.Exception ("Incorrect number of points while trying to read UV data:\n" + line + "\n");
					}
					else {
						objUVs[uvsCount++] = new Vector2(float.Parse (lineInfo[1], CultureInfo.InvariantCulture), float.Parse (lineInfo[2], CultureInfo.InvariantCulture));
					}
				}
				// Read normals
				else if (useSuppliedNormals && char1 == 'v' && char2 == 'n') {
					lineInfo = line.Split (' ');
					if (lineInfo.Length != 4) {
						throw new System.Exception ("Incorrect number of points while trying to read normals:\n" + line + "\n");
					}
					else {
						// Invert x value so it works properly in Unity
						objNormals[normalsCount++] = new Vector3(-float.Parse (lineInfo[1], CultureInfo.InvariantCulture), float.Parse (lineInfo[2], CultureInfo.InvariantCulture), float.Parse (lineInfo[3], CultureInfo.InvariantCulture));
					}
				}
				// Read triangle face info
				else if (char1 == 'f' && char2 == ' ') {
					lineInfo = line.Split (' ');
					if (lineInfo.Length >= 4 && lineInfo.Length <= 5) {
						// If data is relative offset, dissect it and replace it with calculated absolute data
						if (lineInfo[1].Substring (0, 1) == "-") {
							for (int i = 1; i < lineInfo.Length; i++) {
								var lineInfoParts = lineInfo[i].Split ('/');
								lineInfoParts[0] = (verticesCount - -int.Parse (lineInfoParts[0])+1).ToString();
								if (lineInfoParts.Length > 1) {
									if (lineInfoParts[1] != "") {
										lineInfoParts[1] = (uvsCount - -int.Parse (lineInfoParts[1])+1).ToString();
									}
									if (lineInfoParts.Length == 3) {
										lineInfoParts[2] = (normalsCount - -int.Parse (lineInfoParts[2])+1).ToString();
									}
								}
								lineInfo[i] = System.String.Join ("/", lineInfoParts);
							}
						}
						// Triangle
						for (int i = 1; i < 4; i++) {
							triData.Add (lineInfo[i]);
						}
						// Quad -- split by adding another triangle
						if (lineInfo.Length == 5) {
							quadWarning = true;
							triData.Add (lineInfo[1]);
							triData.Add (lineInfo[3]);
							triData.Add (lineInfo[4]);
						}
					}
					// Line describes polygon containing more than 4 or fewer than 3 points, which are not supported
					else {
						polyWarning = true;
					}
					// Store index for face group start locations
					if (++faceCount == originalGroupIndices[groupCount+1]) {
						groupIndices[++groupCount] = triData.Count;
						mtlCount = 0;
						objectNamesCount = 0;
					}
				}
			}
		}
		catch (System.Exception err) {
			Debug.LogError (err.Message);
			return null;
		}
		
		if (combineMultipleGroups && !useSubmeshesWhenCombining) {
			numberOfGroupsUsed = 1;
			groupIndices[1] = triData.Count;
		}
		else {
			groupIndices[groupCount+1] = triData.Count;
			numberOfGroupsUsed = groupIndices.Length-1;
		}
		
		// Parse vert/uv/normal index data from triangle face lines
		var triVerts = new int[triData.Count];
		var triUVs = new int[triData.Count];
		var triNorms = new int[triData.Count];
		var lengthCount = 3;
		for (int i = 0; i < triData.Count; i++) {
			string triString = triData[i];
			lineInfo = triString.Split ('/');
			
			triVerts[i] = int.Parse (lineInfo[0])-1;
			if (lineInfo.Length > 1) {
				if (lineInfo[1] != "") {
					triUVs[i] = int.Parse (lineInfo[1])-1;
				}
				if (lineInfo.Length == lengthCount && useSuppliedNormals) {
					triNorms[i] = int.Parse (lineInfo[2])-1;	
				}
			}
		}
		
		var objVertList = new List<Vector3>(objVertices);
		if (totalUVs > 0) {
			SplitOnUVs (triData, triVerts, triUVs, objVertList, objUVs, objVertices, ref verticesCount);
		}
		
		// Warnings
		if (quadWarning && !suppressWarnings) {
			Debug.LogWarning ("At least one object uses quads...automatic triangle conversion is being used, which may not produce best results");
		}
		if (polyWarning && !suppressWarnings) {
			Debug.LogWarning ("Polygons which are not quads or triangles have been skipped");
		}
		if (totalUVs == 0 && !suppressWarnings) {
			Debug.LogWarning ("At least one object does not seem to be UV mapped...any textures used will appear as a solid color");
		}
		if (totalNormals == 0 && !suppressWarnings) {
			Debug.LogWarning ("No normal data found for at least one object...automatically computing normals instead");
		}
		
		// Errors
		if (totalVertices == 0 && triData.Count == 0) {
			Debug.LogError ("No objects seem to be present...possibly the .obj file is damaged or could not be read");
			return null;
		}
		else if (totalVertices == 0) {
			Debug.LogError ("The .obj file does not contain any vertices");
			return null;
		}
		else if (triData.Count == 0) {
			Debug.LogError ("The .obj file does not contain any polygons");
			return null;
		}
		
		// Set up GameObject array...only 1 object if combining groups
		var gameObjects = new GameObject[combineMultipleGroups? 1 : numberOfGroupsUsed];
		for (int i = 0; i < gameObjects.Length; i++) {
			gameObjects[i] = new GameObject(objectNames[i], typeof(MeshFilter), typeof(MeshRenderer));
		}

		// --------------------------------
		// Create meshes from the .obj data	
		GameObject go = null;
		Mesh mesh = null;
		Vector3[] newVertices = null;
		Vector2[] newUVs = null;
		Vector3[] newNormals = null;
		int[] newTriangles = null;
		var useSubmesh = (combineMultipleGroups && useSubmeshesWhenCombining && numberOfGroupsUsed > 1)? true : false;
		Material[] newMaterials = null;
		if (useSubmesh) {
			newMaterials = new Material[numberOfGroupsUsed];
		}
		int lastUsedMaterialIndex = 0;
		bool hasUsedMaterial = false;
		
		for (int i = 0; i < numberOfGroupsUsed; i++) {
			if (!useSubmesh || (useSubmesh && i == 0)) {
				go = gameObjects[i];
				mesh = new Mesh();
				
				// Find the number of unique vertices used by this group, also used to map original vertices into 0..thisVertices.Count range
				var vertHash = new Dictionary<int, int>();
				var thisVertices = new List<Vector3>();
				int counter = 0;
				int vertHashValue = 0;
				int triStart = groupIndices[i];
				int triEnd = groupIndices[i + 1];
				if (useSubmesh) {
					triStart = groupIndices[0];
					triEnd = groupIndices[numberOfGroupsUsed];
				}
				
				for (int j = triStart; j < triEnd; j++) {
					if (!vertHash.TryGetValue (triVerts[j], out vertHashValue)) {
						vertHash[triVerts[j]] = counter++;
						thisVertices.Add (objVertList[triVerts[j]]);
					}
				}
				
				if (thisVertices.Count > maxPoints) {
					Debug.LogError ("The number of vertices in the object " + objectNames[i] + " exceeds the maximum allowable limit of " + maxPoints);
					return null;
				}
				
				newVertices = new Vector3[thisVertices.Count];
				newUVs = new Vector2[thisVertices.Count];
				newNormals = new Vector3[thisVertices.Count];
				newTriangles = new int[triEnd - triStart];
				
				// Copy .obj mesh data for vertices and triangles to arrays of the correct size
				if (scaleFactor == Vector3.one && objRotation == Vector3.zero && objPosition == Vector3.zero) {
					for (int j = 0; j < thisVertices.Count; j++) {
						newVertices[j] = thisVertices[j];
					}
				}
				else {
					transform.eulerAngles = objRotation;
					transform.position = objPosition;
					transform.localScale = scaleFactor;
					var thisMatrix = transform.localToWorldMatrix;
					for (int j = 0; j < thisVertices.Count; j++) {
						newVertices[j] = thisMatrix.MultiplyPoint3x4 (thisVertices[j]);
					}
					transform.position = Vector3.zero;
					transform.rotation = Quaternion.identity;
					transform.localScale = Vector3.one;
				}	
				
				// Arrange UVs and normals so they match up with vertices
				if (uvsCount > 0 && normalsCount > 0 && useSuppliedNormals) {
					for (int j = triStart; j < triEnd; j++) {
						newUVs[vertHash[triVerts[j]]] = objUVs[triUVs[j]];
						// Needs to be normalized or lighting is whacked (especially with specular), and some apps don't output normalized normals
						newNormals[vertHash[triVerts[j]]] = objNormals[triNorms[j]].normalized;
					}
				}
				else {
					// Arrange UVs so they match up with vertices
					if (uvsCount > 0) {
						for (int j = triStart; j < triEnd; j++) {
							newUVs[vertHash[triVerts[j]]] = objUVs[triUVs[j]];
						}
					}
					// Arrange normals so they match up with vertices
					if (normalsCount > 0 && useSuppliedNormals) {
						for (int j = triStart; j < triEnd; j++) {
							newNormals[vertHash[triVerts[j]]] = objNormals[triNorms[j]];
						}
					}
				}
				
				// Since we flipped the normals, swap triangle points 2 & 3
				counter = 0;
				for (int j = triStart; j < triEnd; j += 3) {
					newTriangles[counter  ] = vertHash[triVerts[j  ]];
					newTriangles[counter+1] = vertHash[triVerts[j+2]];
					newTriangles[counter+2] = vertHash[triVerts[j+1]];
					counter += 3;
				}
				
				mesh.vertices = newVertices;
				mesh.uv = newUVs;
				
				if (autoCenterOnOrigin) {
					var offset = mesh.bounds.center;
					int end = newVertices.Length;
					for (int j = 0; j < end; j++) {
						newVertices[j] -= offset;
					}
					mesh.vertices = newVertices;
				}
				
				if (useSuppliedNormals) {
					mesh.normals = newNormals;
				}
				if (useSubmesh) {
					mesh.subMeshCount = numberOfGroupsUsed;
				}
			}
			
			if (useSubmesh) {
				int thisLength = groupIndices[i + 1] - groupIndices[i];
				var thisTriangles = new int[thisLength];
				System.Array.Copy (newTriangles, groupIndices[i], thisTriangles, 0, thisLength);
				mesh.SetTriangles (thisTriangles, i);
				if (materialNames[i] != null) {
					if (useMtl && materials.ContainsKey (materialNames[i])) {
						newMaterials[i] = materials[materialNames[i]];
						if (materials[materialNames[i]]) {
							
						}
						hasUsedMaterial = true;
						lastUsedMaterialIndex = i;
					}
				}
				else {
					if (hasUsedMaterial) {
						newMaterials[i] = materials[materialNames[lastUsedMaterialIndex]];
					}
					else {
						newMaterials[i] = standardMaterial;
					}
				}
			}
			else {
				mesh.triangles = newTriangles;
			}
			
			// Stuff that's done for each object, or at the end if using submeshes
			if (!useSubmesh || (useSubmesh && i == numberOfGroupsUsed-1) ) {
				if (normalsCount == 0 || !useSuppliedNormals) {
					mesh.RecalculateNormals();
					if (computeTangents) {
						newNormals = mesh.normals;
					}
				}
				if (computeTangents) {
					var newTangents = new Vector4[newVertices.Length];
					CalculateTangents (newVertices, newNormals, newUVs, newTriangles, newTangents);
					mesh.tangents = newTangents;
				}
				
				mesh.RecalculateBounds();
				go.GetComponent<MeshFilter>().mesh = mesh;
				if (!useSubmesh) {
					if (materialNames[i] != null) {
						if (useMtl && materials.ContainsKey (materialNames[i])) {
							go.GetComponent<Renderer>().material = materials[materialNames[i]];
							hasUsedMaterial = true;
							lastUsedMaterialIndex = i;
						}
					}
					else {
						if (hasUsedMaterial) {
							go.GetComponent<Renderer>().material = materials[materialNames[lastUsedMaterialIndex]];
						}
						else {
							go.GetComponent<Renderer>().material = standardMaterial;
						}
					}
				}
				else {
					go.GetComponent<Renderer>().materials = newMaterials;
				}
			}
		}
		
		if (objData != null) {
			objData.SetDone();
			objData.gameObjects = gameObjects;
			return null;
		}
		
		return gameObjects;
	}
	
	private void CleanLine (ref string line) {
		// This fixes lines so that any instance of at least two spaces is replaced with one
		// Using System.Text.RegularExpressions adds 900K to the web player, so instead this is done a little differently
		while (line.IndexOf ("  ") != -1) {
			line = line.Replace ("  ", " ");
		}
		line = line.Trim();
	}

	private void CalculateTangents (Vector3[] vertices, Vector3[] normals, Vector2[] uv, int[] triangles, Vector4[] tangents) {
		Vector3[] tan1 = new Vector3[vertices.Length];
		Vector3[] tan2 = new Vector3[vertices.Length];
		int triCount = triangles.Length;
		int tangentCount = tangents.Length;
		Vector3 sdir;
		Vector3 tdir;
		
		for (int i = 0; i < triCount; i += 3) {
			int i1 = triangles[i];
			int i2 = triangles[i+1];
			int i3 = triangles[i+2];
			
			Vector3 v1 = vertices[i1];
			Vector3 v2 = vertices[i2];
			Vector3 v3 = vertices[i3];
			
			Vector2 w1 = uv[i1];
			Vector2 w2 = uv[i2];
			Vector2 w3 = uv[i3];
			
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
			
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
	
			float div = s1 * t2 - s2 * t1;
			float r = (div == 0.0f)? 0.0f : 1.0f / div;
			sdir.x = (t2 * x1 - t1 * x2) * r;
			sdir.y = (t2 * y1 - t1 * y2) * r;
			sdir.z = (t2 * z1 - t1 * z2) * r;
			tdir.x = (s1 * x2 - s2 * x1) * r;
			tdir.y = (s1 * y2 - s2 * y1) * r;
			tdir.z = (s1 * z2 - s2 * z1) * r;
			
			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;
			
			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}
	
		for (int i = 0; i < tangentCount; i++) {
			Vector3 n = normals[i];
			Vector3 t = tan1[i];
			
			Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
			tangents[i] = new Vector4(tmp.x, tmp.y, tmp.z);
			tangents[i].w = (Vector3.Dot (Vector3.Cross (n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f;
		}
		
		tan1 = null;
		tan2 = null;
	}
	
	private void SplitOnUVs (List<string> triData, int[] triVerts, int[] triUVs, List<Vector3> objVertList, Vector2[] objUVs, Vector3[] objVertices,
							 ref int verticesCount) {
		var triHash = new Dictionary<int, Vector2>();
		var uvHash = new Dictionary<Int2, int>();
		var triIndex = new Int2();
		var triHashValue = Vector2.zero;
		var uvHashValue = 0;
		for (int i = 0; i < triData.Count; i++) {
			if (!triHash.TryGetValue (triVerts[i], out triHashValue)) {
				triHash[triVerts[i]] = objUVs[triUVs[i]];
			}
			// If it's the same vertex but the UVs are different...
			else if (triHash[triVerts[i]] != objUVs[triUVs[i]]) {
				triIndex = new Int2(triVerts[i], triUVs[i]);
				// If the UV index of a previously added vertex already exists, use that vertex
				if (uvHash.TryGetValue (triIndex, out uvHashValue)) {
					triVerts[i] = uvHashValue;
				}
				// Otherwise make a new vertex and keep the UV reference count to the vertex count
				else {
					objVertList.Add (objVertices[triVerts[i]]);
					triVerts[i] = verticesCount++;
					triHash[triVerts[i]] = objUVs[triUVs[i]];
					uvHash[triIndex] = triUVs[i];
				}
			}
		}
		triHash = null;
		uvHash = null;
	}
	
	private string GetMTLFileName (ref string objFilePath, ref string objFile, ref string filePath) {
		filePath = objFilePath.Substring (0, objFilePath.LastIndexOf ("/") + 1);
		string mtlFileName = "";
		// Extract mtl file name, if there is one
		int idx = objFile.IndexOf ("mtllib");
		if (idx != -1) {
			int idx2 = objFile.IndexOf ('\n', idx);
			if (idx2 != -1) {
				mtlFileName = GetFileName (objFile.Substring (idx, idx2-idx), "mtllib");
			}
		}
		return mtlFileName;
	}
	
	private string GetFileName (string line, string token) {
		string fileName = "";
		if (line.Length > token.Length + 2) {
			int idx = token.Length + 1;
			fileName = line.Substring (idx, line.Length - idx).Replace ("\r", "");
		}
		return fileName;
	}
	
	private bool IsTextureLine (string line) {
		return (line.StartsWith ("map_Kd") || line.StartsWith ("map_bump") || line.StartsWith ("bump"));
	}
	
	private Dictionary<string, Texture2D> GetTextures (ref string mtlFile, out string[] lines, string filePath) {
		lines = new string[0];
#if !UNITY_WEBPLAYER && !UNITY_WEBGL
		var textures = new Dictionary<string, Texture2D>();
		Texture2D diffuseTexture = null;
#endif
		lines = mtlFile.Split ('\n');

		for (int i = 0; i < lines.Length; i++) {
			var line = lines[i];
			CleanLine (ref line);
			lines[i] = line;
			
			if (line.Length < 3 || line[0] == '#') {
				continue;
			}
#if !UNITY_WEBPLAYER && !UNITY_WEBGL
			// Get diffuse/bump texture
			if (IsTextureLine (line) && filePath != "") {
				var textureFilePath = GetFileName (line, GetToken (line));
				if (textureFilePath != "") {
					var completeFilePath = filePath + textureFilePath;
					if (!File.Exists (completeFilePath)) {
						throw new System.Exception("Texture file not found: " + completeFilePath);
					}
					diffuseTexture = new Texture2D (4, 4);
					diffuseTexture.LoadImage (File.ReadAllBytes (completeFilePath));
					if (line.StartsWith ("map_bump") || line.StartsWith ("bump")) {
						ConvertToNormalmap (diffuseTexture);
					}
					textures[textureFilePath] = diffuseTexture;
				}
			}
#endif
		}
		
#if !UNITY_WEBPLAYER && !UNITY_WEBGL
		return textures;
#else
		return null;
#endif
	}
	
	private string GetToken (string line) {
		var token = "map_Kd";
		if (line.StartsWith ("map_bump")) {
			token = "map_bump";
		}
		else if (line.StartsWith ("bump")) {
			token = "bump";
		}
		return token;
	}
	
	private void ConvertToNormalmap (Texture2D tex) {
		var colors = tex.GetPixels32();
		for (int j = 0; j < colors.Length; j++){
			colors[j].a = colors[j].r;
			colors[j].r = colors[j].g;
			colors[j].b = colors[j].g;
		}
		tex.SetPixels32 (colors);
		tex.Apply();
	}
	
	private IEnumerator GetWWWFiles (string objFilePath, bool useMtl, Material standardMaterial, Material transparentMaterial, ObjData objData) {
		var www = new WWW(objFilePath);
		while (!www.isDone) {
			objData.SetProgress (www.progress * (useMtl? .5f : 1.0f));
			if (objData.cancel) {
				yield break;
			}
			yield return null;
		}
		if (www.error != null) {
			Debug.LogError ("Error loading " + objFilePath + ": " + www.error);
			objData.SetDone();
			yield break;
		}
		
		string objFile = www.text;
		string filePath = "";
		Dictionary<string, Material> materials = null;
		
		if (useMtl) {
			string mtlFileName = GetMTLFileName (ref objFilePath, ref objFile, ref filePath);
			if (mtlFileName != "") {
				// Read in mtl file
				www = new WWW(filePath + mtlFileName);
				while (!www.isDone) {
					if (objData.cancel) {
						yield break;
					}
					yield return null;
				}
				if (www.error != null) {
					if (!useMTLFallback) {
						Debug.LogError ("Error loading " + (filePath + mtlFileName) + ": " + www.error);
						objData.SetDone();
						yield break;
					}
					else {
						useMtl = false;
					}
				}
				if (useMtl && www.text != "") {
					// Get textures and parse MTL file
					var linesRef = new LinesRef();
					var textures = new Dictionary<string, Texture2D>();
					var loadError = new BoolRef(false);
					yield return StartCoroutine (GetTexturesAsync (www.text, linesRef, filePath, textures, objData, loadError));
					if (loadError.b == true) {
						yield break;
					}
					materials = ParseMTL (linesRef.lines, standardMaterial, transparentMaterial, filePath, textures);
					if (materials == null) {
						useMtl = false;
					}
				}
			}
			else {
				useMtl = false;
			}
		}
		
		CreateObjects (ref objFile, useMtl, materials, standardMaterial, objData, Path.GetFileNameWithoutExtension (objFilePath));
	}
	
	private IEnumerator GetTexturesAsync (string mtlFile, LinesRef linesRef, string filePath, Dictionary<string, Texture2D> textures, ObjData objData, BoolRef loadError) {
		Texture2D diffuseTexture = null;
		string[] lines = mtlFile.Split ('\n');

		int numberOfTextures = 0;
		
		// See how many textures there are (to use for progress)
		for (int i = 0; i < lines.Length; i++) {
			var line = lines[i];
			CleanLine (ref line);
			lines[i] = line;
			
			if (line.Length < 7 || line[0] == '#') {
				continue;
			}
			
			if (IsTextureLine (line) && filePath != "") {
				numberOfTextures++;
			}
		}
		
		float progress = .5f;
		for (int i = 0; i < lines.Length; i++) {	
			if (lines[i].Length < 7 || lines[i][0] == '#') {
				continue;
			}
			
			// Get diffuse/bump texture
			if (IsTextureLine (lines[i]) && filePath != "") {
				var textureFilePath = GetFileName (lines[i], GetToken (lines[i]));
				if (textureFilePath != "") {
					var completeFilePath = filePath + textureFilePath;
					var www = new WWW(completeFilePath);
					while (!www.isDone) {
						objData.SetProgress (progress + (www.progress / numberOfTextures) * .5f);
						if (objData.cancel) {
							loadError.b = true;
							yield break;
						}
						yield return null;
					}
					if (www.error != null) {
						Debug.LogError ("Error loading " + completeFilePath + ": " + www.error);
						loadError.b = true;
						objData.SetDone();
						yield break;
					}
					progress += (1.0f / numberOfTextures) * .5f;
					diffuseTexture = new Texture2D (4, 4);
					www.LoadImageIntoTexture (diffuseTexture);
					if (lines[i].StartsWith ("map_bump") || lines[i].StartsWith ("bump")) {
						ConvertToNormalmap (diffuseTexture);
					}
					textures[textureFilePath] = diffuseTexture;
				}
			}
		}
		linesRef.lines = lines;
	}

	private Dictionary<string, Material> ParseMTL (string[] lines, Material standardMaterial, Material transparentMaterial, string filePath, Dictionary<string, Texture2D> textures) {
		var mtlDictionary = new Dictionary<string, Material>();
		
		try {
			var mtlName = "";
			float aR = 0, aG = 0, aB = 0,  dR = 0, dG = 0, dB = 0,  sR = 0, sG = 0, sB = 0;
			float dVal = 1.0f;
			float specularHighlight = 0.0f;
			int count = 0;
			Texture2D diffuseTexture = null;
			Texture2D bumpTexture = null;
			
			for (int i = 0; i < lines.Length; i++) {
				var line = lines[i];
				
				if (line.Length < 3 || line[0] == '#') {
					continue;
				}
				
				if (line.StartsWith ("newmtl")) {
					if (count++ > 0) {
						SetMaterial (mtlDictionary, mtlName, aR, aG, aB, dR, dG, dB, sR, sG, sB, standardMaterial, transparentMaterial, dVal,
									specularHighlight, diffuseTexture, bumpTexture);
						aR = 0; aG = 0; aB = 0;  dR = 0; dG = 0; dB = 0;  sR = 0; sG = 0; sB = 0;
						dVal = 1.0f;
						specularHighlight = 0.0f;
					}
					diffuseTexture = null;
					var lineInfo = line.Split (' ');
					if (lineInfo.Length > 1) {
						mtlName = lineInfo[1];
					}
					continue;
				}
				if (line.StartsWith ("map_Kd") && filePath != "") {
					var textureFilePath = GetFileName (line, "map_Kd");
					if (textureFilePath != "") {
						if (textures.ContainsKey (textureFilePath)) {
							diffuseTexture = textures[textureFilePath];
						}
					}
					continue;
				}
				if ((line.StartsWith ("map_bump") || line.StartsWith ("bump")) && filePath != "") {
					var bumpFilePath = GetFileName (line, line.StartsWith ("map_bump")? "map_bump" : "bump");
					if (bumpFilePath != "") {
						if (textures.ContainsKey (bumpFilePath)) {
							bumpTexture = textures[bumpFilePath];
						}
					}
					continue;
				}
								
				var lineStart = line.Substring (0, 2).ToLower();
				if (lineStart == "ka") {
					ParseKLine (ref line, ref aR, ref aG, ref aB);
				}
				else if (lineStart == "kd") {
					ParseKLine (ref line, ref dR, ref dG, ref dB);
				}
				else if (lineStart == "ks") {
					ParseKLine (ref line, ref sR, ref sG, ref sB);
				}
				else if (lineStart == "d " || lineStart == "tr") {
					var lineInfo = line.Split (' ');
					if (lineInfo.Length > 1) {
						if (lineInfo[1] == "-halo") {
							if (lineInfo.Length > 2) {
								dVal = float.Parse (lineInfo[2], CultureInfo.InvariantCulture);
							}
						}
						else {
							dVal = float.Parse (lineInfo[1], CultureInfo.InvariantCulture);
						}
					}
				}
				else if (lineStart == "ns") {
					var lineInfo = line.Split (' ');
					if (lineInfo.Length > 1) {
						specularHighlight = float.Parse (lineInfo[1], CultureInfo.InvariantCulture);
					}
				}
			}
			SetMaterial (mtlDictionary, mtlName, aR, aG, aB, dR, dG, dB, sR, sG, sB, standardMaterial, transparentMaterial, dVal,
						specularHighlight, diffuseTexture, bumpTexture);
		}
		catch (System.Exception err) {
			Debug.LogError (err.Message);
			return null;
		}
			
		return mtlDictionary;
	}
	
	private void SetMaterial (Dictionary<string, Material> mtlDictionary, string mtlName, float aR, float aG, float aB, float dR, float dG, float dB,
						float sR, float sG, float sB, Material standardMaterial, Material transparentMaterial, float transparency, float specularHighlight,
						Texture2D diffuseTexture, Texture2D bumpTexture) {
		Material mat = null;
		if (transparency == 1.0f) {
			if (standardMaterial == null) {
				mat = new Material(Shader.Find ("VertexLit"));
			}
			else {
				mat = Instantiate (standardMaterial) as Material;
			}
		}
		else {
			if (transparentMaterial == null) {
				mat = new Material(Shader.Find ("Transparent/VertexLit"));
			}
			else {
				mat = Instantiate (transparentMaterial) as Material;
			}
		}
		if (mat.HasProperty ("_Emission") && !overrideAmbient) {
			mat.SetColor ("_Emission", new Color(aR, aG, aB, 1.0f));
		}
		if (mat.HasProperty ("_Color") && !overrideDiffuse) {
			mat.SetColor ("_Color", new Color(dR, dG, dB, transparency));
		}
		if (mat.HasProperty ("_SpecColor") && !overrideSpecular) {
			mat.SetColor ("_SpecColor", new Color(sR, sG, sB, 1.0f));
		}
		if (mat.HasProperty ("_Shininess")) {
			mat.SetFloat ("_Shininess", specularHighlight / 1000.0f);
		}
		if (mat.HasProperty ("_MainTex")) {
			mat.mainTexture = diffuseTexture;
		}
		if (mat.HasProperty ("_BumpMap")) {
			mat.SetTexture ("_BumpMap", bumpTexture);
		}
		mat.name = mtlName;
		mtlDictionary[mtlName] = mat;
	}
	
	private void ParseKLine (ref string line, ref float r, ref float g, ref float b) {
		if (line.Contains (".rfl") && !suppressWarnings) {
			Debug.LogWarning (".rfl files not supported");
			return;
		}
		if (line.Contains ("xyz") && !suppressWarnings) {
			Debug.LogWarning ("CIEXYZ color not supported");
			return;
		}
		try {
			var lineInfo = line.Split (' ');
			if (lineInfo.Length > 1) {
				r = float.Parse (lineInfo[1], CultureInfo.InvariantCulture);
			}
			if (lineInfo.Length > 3) {
				g = float.Parse (lineInfo[2], CultureInfo.InvariantCulture);
				b = float.Parse (lineInfo[3], CultureInfo.InvariantCulture);
			}
			else {
				g = r;
				b = r;
			}
		}
		catch (System.Exception err) {
			Debug.LogWarning ("Incorrect number format when parsing MTL file: " + err.Message);
		}
	}

	public class Int2 {
		public int a;
		public int b;
		
		public Int2 () {
			a = 0;
			b = 0;
		}
		
		public Int2 (int a, int b) {
			this.a = a;
			this.b = b;
		}
	}

	public class ObjData {
		bool _isDone = false;
		public bool isDone {
			get {return _isDone;}
		}
		float _progress = 0.0f;
		public float progress {
			get {return _progress;}
		}
		public GameObject[] gameObjects = null;
		bool _cancel = false;
		public bool cancel {
			get {return _cancel;}
		}
		
		public void SetDone () {
			_isDone = true;
		}
		public void SetProgress (float p) {
			_progress = p;
		}
		public void Cancel () {
			_cancel = true;
		}
	}
	
	public class LinesRef {
		public string[] lines;
	}
	
	public class BoolRef {
		public bool b;
		public BoolRef (bool b) {
			this.b = b;
		}
	}
}