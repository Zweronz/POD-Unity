using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PodScene
{
    public float[] colourBackground;		/*!< Background colour */
	public float[] colourAmbient;			/*!< Ambient colour */

	public uint numCamera;				/*!< The length of the array pCamera */
	public PodCamera[] camera;				/*!< Camera nodes array */

	public uint numLight;				/*!< The length of the array pLight */
	public PodLight[] light;				/*!< Light nodes array */

	public uint numMesh;				/*!< The length of the array pMesh */
	public PodMesh[] mesh;					/*!< Mesh array. Meshes may be instanced several times in a scene; i.e. multiple Nodes may reference any given mesh. */

	public uint numNode;		/*!< Number of items in the array pNode */
	public uint numMeshNode;	/*!< Number of items in the array pNode which are objects */
	public PodNode[] node;			/*!< Node array. Sorted as such: objects, lights, cameras, Everything Else (bones, helpers etc) */

	public uint numTexture;	/*!< Number of textures in the array pTexture */
	public PodTexture[] texture;		/*!< Texture array */

	public uint numMaterial;	/*!< Number of materials in the array pMaterial */
	public PodMaterial[] material;		/*!< Material array */

	public uint numFrame;		/*!< Number of frames of animation */
	public uint fPS;			/*!< The frames per second the animation should be played at */

	public uint flags;			/*!< PVRTMODELPODSF_* bit-flags */

	public uint userDataSize;
	public string userData;
}

[System.Serializable]
public class PodCamera
{
	public uint idxTarget;			/*!< Index of the target object */
	public float fOV;				/*!< Field of view */
	public float far;				/*!< Far clip plane */
	public float near;				/*!< Near clip plane */
	public float[] animFOV;			/*!< 1 VERTTYPE per frame of animation. */
};

[System.Serializable]
public class PodLight
{
	public uint idxTarget;		/*!< Index of the target object */
	public float[] colour;	/*!< Light colour (0.0f -> 1.0f for each channel) */
	public /*PODLightType thanks c# */ uint type;			/*!< Light type (point, directional, spot etc.) */
	public float constantAttenuation;	/*!< Constant attenuation */
	public float linearAttenuation;		/*!< Linear atternuation */
	public float quadraticAttenuation;	/*!< Quadratic attenuation */
	public float falloffAngle;			/*!< Falloff angle (in radians) */
	public float falloffExponent;		/*!< Falloff exponent */
};

[System.Serializable]
public class PodMesh
{
	public uint numVertex;		/*!< Number of vertices in the mesh */
	public uint numFaces;		/*!< Number of triangles in the mesh */
	public uint numUVW;		/*!< Number of texture coordinate channels per vertex */
	public PodData faces;			/*!< List of triangle indices */
	public uint[] stripLength;	/*!< If mesh is stripped: number of tris per strip. */
	public uint numStrips;		/*!< If mesh is stripped: number of strips, length of pnStripLength array. */
	public PodData vertex;		/*!< List of vertices (x0, y0, z0, x1, y1, z1, x2, etc...) */
	public PodData normals;		/*!< List of vertex normals (Nx0, Ny0, Nz0, Nx1, Ny1, Nz1, Nx2, etc...) */
	public PodData tangents;		/*!< List of vertex tangents (Tx0, Ty0, Tz0, Tx1, Ty1, Tz1, Tx2, etc...) */
	public PodData binormals;		/*!< List of vertex binormals (Bx0, By0, Bz0, Bx1, By1, Bz1, Bx2, etc...) */
	public PodData[] uVW;			/*!< List of UVW coordinate sets; size of array given by 'nNumUVW' */
	public PodData vtxColours;	/*!< A colour per vertex */
	public PodData boneIdx;		/*!< nNumBones*nNumVertex ints (Vtx0Idx0, Vtx0Idx1, ... Vtx1Idx0, Vtx1Idx1, ...) */
	public PodData boneWeight;	/*!< nNumBones*nNumVertex floats (Vtx0Wt0, Vtx0Wt1, ... Vtx1Wt0, Vtx1Wt1, ...) */

	public byte[] interleaved;	/*!< Interleaved vertex data */

	public PVRTBoneBatches	boneBatches = new PVRTBoneBatches();	/*!< Bone tables */

	public PODPrimitiveType	primitiveType;	/*!< Primitive type used by this mesh */

	public float[] unpackMatrix;	/*!< A matrix used for unscaling scaled vertex data created with PVRTModelPODScaleAndConvertVtxData*/
};

[System.Serializable]
public class PodNode
{
	public uint idx;				/*!< Index into mesh, light or camera array, depending on which object list contains this Node */
	public string name;			/*!< Name of object */
	public uint idxMaterial;		/*!< Index of material used on this mesh */

	public uint idxParent;		/*!< Index into MeshInstance array; recursively apply ancestor's transforms after this instance's. */

	public uint animFlags;		/*!< Stores which animation arrays the POD Node contains */

	public uint[] animPositionIdx;
	public float[] animPosition;	/*!< 3 floats per frame of animation. */

	public uint[] animRotationIdx;
	public float[] animRotation;	/*!< 4 floats per frame of animation. */

	public uint[] animScaleIdx;
	public float[] animScale;		/*!< 7 floats per frame of animation. */

	public uint[] animMatrixIdx;
	public float[] animMatrix;		/*!< 16 floats per frame of animation. */

	public uint userDataSize;
	public string userData;
};

[System.Serializable]
public class PodTexture
{
	public string name;			/*!< File-name of texture */
};

[System.Serializable]
public class PodMaterial
{
	public string name;				/*!< Name of material */
	public uint	idxTexDiffuse;			/*!< Idx into pTexture for the diffuse texture */
	public uint	idxTexAmbient;			/*!< Idx into pTexture for the ambient texture */
	public uint	idxTexSpecularColour;	/*!< Idx into pTexture for the specular colour texture */
	public uint	idxTexSpecularLevel;	/*!< Idx into pTexture for the specular level texture */
	public uint	idxTexBump;			/*!< Idx into pTexture for the bump map */
	public uint	idxTexEmissive;		/*!< Idx into pTexture for the emissive texture */
	public uint	idxTexGlossiness;		/*!< Idx into pTexture for the glossiness texture */
	public uint	idxTexOpacity;			/*!< Idx into pTexture for the opacity texture */
	public uint	idxTexReflection;		/*!< Idx into pTexture for the reflection texture */
	public uint	idxTexRefraction;		/*!< Idx into pTexture for the refraction texture */
	public float matOpacity;			/*!< Material opacity (used with vertex alpha ?) */
	public float[] matAmbient;		/*!< Ambient RGB value */
	public float[] matDiffuse;		/*!< Diffuse RGB value */
	public float[] matSpecular;		/*!< Specular RGB value */
	public float matShininess;			/*!< Material shininess */
	public string effectFile;			/*!< Name of effect file */
	public string effectName;			/*!< Name of effect in the effect file */

	public /*PODBlendFunc thanks c# */ uint	blendSrcRGB;		/*!< Blending RGB source value */
	public /*PODBlendFunc thanks c# */ uint	blendSrcA;			/*!< Blending alpha source value */
	public /*PODBlendFunc thanks c# */ uint	blendDstRGB;		/*!< Blending RGB destination value */
	public /*PODBlendFunc thanks c# */ uint	blendDstA;			/*!< Blending alpha destination value */
	public /*PODBlendOp thanks c# */ uint blendOpRGB;		/*!< Blending RGB operation */
	public /*PODBlendOp thanks c# */ uint blendOpA;			/*!< Blending alpha operation */
    public float[] blendColour;	/*!< A RGBA colour to be used in blending */
    public float[] blendFactor;	/*!< An array of blend factors, one for each RGBA component */

	public uint flags;				/*!< Stores information about the material e.g. Enable blending */

	public uint userDataSize;
	public string userData;
};

[System.Serializable]
public class PodData
{
	public /*PVRTDataType thanks c# */ uint	type;		/*!< Type of data stored */
	public uint n;			/*!< Number of values per vertex */
	public uint stride;	/*!< Distance in bytes from one array entry to the next */
	public byte[] data;		/*!< Actual data (array of values); if mesh is interleaved, this is an OFFSET from pInterleaved */
};

[System.Serializable]
public class PVRTBoneBatches
{
	public int[] batches;			/*!< Space for nBatchBoneMax bone indices, per batch */
	public int[] batchBoneCnt;	/*!< Actual number of bone indices, per batch */
	public int[] batchOffset;		/*!< Offset into triangle array, per batch */
	public int batchBoneMax;		/*!< Stored value as was passed into Create() */
	public int batchCnt;
}