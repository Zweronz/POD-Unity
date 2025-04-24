/****************************************************************************
** Enumerations
****************************************************************************/
/*!****************************************************************************
 @public unsafe struct      EPODLightType
 @Brief       Enum for the POD format light types
******************************************************************************/
using System.Runtime.InteropServices;

public enum EPODLightType
{
	ePODPoint=0,	 /*!< Point light */
	ePODDirectional, /*!< Directional light */
	ePODSpot,		 /*!< Spot light */
	eNumPODLightTypes
};

/*!****************************************************************************
 @public unsafe struct      EPODPrimitiveType
 @Brief       Enum for the POD format primitive types
******************************************************************************/
public enum EPODPrimitiveType
{
	ePODTriangles=0, /*!< Triangles */
	eNumPODPrimitiveTypes
};

/*!****************************************************************************
 @public unsafe struct      EPODAnimationData
 @Brief       Enum for the POD format animation types
******************************************************************************/
public enum EPODAnimationData
{
	ePODHasPositionAni	= 0x01,	/*!< Position animation */
	ePODHasRotationAni	= 0x02, /*!< Rotation animation */
	ePODHasScaleAni		= 0x04, /*!< Scale animation */
	ePODHasMatrixAni	= 0x08  /*!< Matrix animation */
};

/*!****************************************************************************
 @public unsafe struct      EPODMaterialFlags
 @Brief       Enum for the material flag options
******************************************************************************/
public enum EPODMaterialFlag
{
	ePODEnableBlending	= 0x01,	/*!< Enable blending for this material */
};

/*!****************************************************************************
 @public unsafe struct      EPODBlendFunc
 @Brief       Enum for the POD format blend functions
******************************************************************************/
public enum EPODBlendFunc
{
	ePODBlendFunc_ZERO=0,
	ePODBlendFunc_ONE,
	ePODBlendFunc_BLEND_FACTOR,
	ePODBlendFunc_ONE_MINUS_BLEND_FACTOR,

	ePODBlendFunc_SRC_COLOR = 0x0300,
	ePODBlendFunc_ONE_MINUS_SRC_COLOR,
	ePODBlendFunc_SRC_ALPHA,
	ePODBlendFunc_ONE_MINUS_SRC_ALPHA,
	ePODBlendFunc_DST_ALPHA,
	ePODBlendFunc_ONE_MINUS_DST_ALPHA,
	ePODBlendFunc_DST_COLOR,
	ePODBlendFunc_ONE_MINUS_DST_COLOR,
	ePODBlendFunc_SRC_ALPHA_SATURATE,

	ePODBlendFunc_CONSTANT_COLOR = 0x8001,
	ePODBlendFunc_ONE_MINUS_CONSTANT_COLOR,
	ePODBlendFunc_CONSTANT_ALPHA,
	ePODBlendFunc_ONE_MINUS_CONSTANT_ALPHA
};

/*!****************************************************************************
 @public unsafe struct      EPODBlendOp
 @Brief       Enum for the POD format blend operation
******************************************************************************/
public enum EPODBlendOp
{
	ePODBlendOp_ADD = 0x8006,
	ePODBlendOp_MIN,
	ePODBlendOp_MAX,
	ePODBlendOp_SUBTRACT = 0x800A,
	ePODBlendOp_REVERSE_SUBTRACT,
};

public enum EPVRTDataType {
	EPODDataNone,
	EPODDataFloat,
	EPODDataInt,
	EPODDataUnsignedShort,
	EPODDataRGBA,
	EPODDataARGB,
	EPODDataD3DCOLOR,
	EPODDataUBYTE4,
	EPODDataDEC3N,
	EPODDataFixed16_16,
	EPODDataUnsignedByte,
	EPODDataShort,
	EPODDataShortNorm,
	EPODDataByte,
	EPODDataByteNorm,
	EPODDataUnsignedByteNorm,
	EPODDataUnsignedShortNorm,
	EPODDataUnsignedInt
};

[StructLayout(LayoutKind.Sequential)]
public unsafe struct PVRTMATRIX
{
	public fixed float f[16];	/*!< Array of float */
};

[StructLayout(LayoutKind.Sequential)]
public unsafe struct CPVRTBoneBatches
{
	public int *pnBatches;			/*!< Space for nBatchBoneMax bone indices, per batch */
	public int	*pnBatchBoneCnt;	/*!< Actual number of bone indices, per batch */
	public int	*pnBatchOffset;		/*!< Offset into triangle array, per batch */
	public int nBatchBoneMax;		/*!< Stored value as was passed into Create() */
	public int	nBatchCnt;			/*!< Number of batches to render */
};

/****************************************************************************
** Structures
****************************************************************************/
/*!****************************************************************************
 @public unsafe class      CPODData
 @Brief      A public unsafe class for representing POD data
******************************************************************************/
[StructLayout(LayoutKind.Sequential)]
public unsafe struct CPODData {

	public EPVRTDataType	eType;		/*!< Type of data stored */
	public uint		n;			/*!< Number of values per vertex */
	public uint		nStride;	/*!< Distance in bytes from one array entry to the next */
	public byte		*pData;		/*!< Actual data (array of values); if mesh is interleaved, this is an OFFSET from pInterleaved */
};

/*!****************************************************************************
 @public unsafe struct      SPODCamera
 @Brief       public unsafe struct for storing POD camera data
******************************************************************************/
[StructLayout(LayoutKind.Sequential)]
public unsafe struct SPODCamera {
	public int			nIdxTarget;			/*!< Index of the target object */
	public float	fFOV;				/*!< Field of view */
	public float	fFar;				/*!< Far clip plane */
	public float	fNear;				/*!< Near clip plane */
	public float	*pfAnimFOV;			/*!< 1 float per frame of animation. */
};

/*!****************************************************************************
 @public unsafe struct      SPODLight
 @Brief       public unsafe struct for storing POD light data
******************************************************************************/
[StructLayout(LayoutKind.Sequential)]
public unsafe struct SPODLight {
	public int			nIdxTarget;		/*!< Index of the target object */
	public fixed float			pfColour[3];	/*!< Light colour (0.0f -> 1.0f for each channel) */
	public EPODLightType		eType;			/*!< Light type (point, directional, spot etc.) */
	public float			fConstantAttenuation;	/*!< Constant attenuation */
	public float			fLinearAttenuation;		/*!< Linear atternuation */
	public float			fQuadraticAttenuation;	/*!< Quadratic attenuation */
	public float			fFalloffAngle;			/*!< Falloff angle (in radians) */
	public float			fFalloffExponent;		/*!< Falloff exponent */
};

/*!****************************************************************************
 @public unsafe struct      SPODMesh
 @Brief       public unsafe struct for storing POD mesh data
******************************************************************************/
[StructLayout(LayoutKind.Sequential)]
public unsafe struct SPODMesh {
	public uint			nNumVertex;		/*!< Number of vertices in the mesh */
	public uint			nNumFaces;		/*!< Number of triangles in the mesh */
	public uint			nNumUVW;		/*!< Number of texture coordinate channels per vertex */
	public CPODData			sFaces;			/*!< List of triangle indices */
	public uint			*pnStripLength;	/*!< If mesh is stripped: number of tris per strip. */
	public uint			nNumStrips;		/*!< If mesh is stripped: number of strips, length of pnStripLength array. */
	public CPODData			sVertex;		/*!< List of vertices (x0, y0, z0, x1, y1, z1, x2, etc...) */
	public CPODData			sNormals;		/*!< List of vertex normals (Nx0, Ny0, Nz0, Nx1, Ny1, Nz1, Nx2, etc...) */
	public CPODData			sTangents;		/*!< List of vertex tangents (Tx0, Ty0, Tz0, Tx1, Ty1, Tz1, Tx2, etc...) */
	public CPODData			sBinormals;		/*!< List of vertex binormals (Bx0, By0, Bz0, Bx1, By1, Bz1, Bx2, etc...) */
	public CPODData			*psUVW;			/*!< List of UVW coordinate sets; size of array given by 'nNumUVW' */
	public CPODData			sVtxColours;	/*!< A colour per vertex */
	public CPODData			sBoneIdx;		/*!< nNumBones*nNumVertex ints (Vtx0Idx0, Vtx0Idx1, ... Vtx1Idx0, Vtx1Idx1, ...) */
	public CPODData			sBoneWeight;	/*!< nNumBones*nNumVertex floats (Vtx0Wt0, Vtx0Wt1, ... Vtx1Wt0, Vtx1Wt1, ...) */

	public byte			*pInterleaved;	/*!< Interleaved vertex data */

	public CPVRTBoneBatches	sBoneBatches;	/*!< Bone tables */

	public EPODPrimitiveType	ePrimitiveType;	/*!< Primitive type used by this mesh */

	public PVRTMATRIX			mUnpackMatrix;	/*!< A matrix used for unscaling scaled vertex data created with PVRTModelPODScaleAndConvertVtxData*/
};

/*!****************************************************************************
 @public unsafe struct      SPODNode
 @Brief       public unsafe struct for storing POD node data
******************************************************************************/
[StructLayout(LayoutKind.Sequential)]
public unsafe struct SPODNode {
	public int			nIdx;				/*!< Index into mesh, light or camera array, depending on which object list contains this Node */
	public char			*pszName;			/*!< Name of object */
	public int			nIdxMaterial;		/*!< Index of material used on this mesh */

	public int			nIdxParent;		/*!< Index into MeshInstance array; recursively apply ancestor's transforms after this instance's. */

	public uint			nAnimFlags;		/*!< Stores which animation arrays the POD Node contains */

	public uint			*pnAnimPositionIdx;
	public float			*pfAnimPosition;	/*!< 3 floats per frame of animation. */

	public uint			*pnAnimRotationIdx;
	public float			*pfAnimRotation;	/*!< 4 floats per frame of animation. */

	public uint			*pnAnimScaleIdx;
	public float			*pfAnimScale;		/*!< 7 floats per frame of animation. */

	public uint			*pnAnimMatrixIdx;
	public float			*pfAnimMatrix;		/*!< 16 floats per frame of animation. */

	public uint			nUserDataSize;
	public char			*pUserData;
};

/*!****************************************************************************
 @public unsafe struct      SPODTexture
 @Brief       public unsafe struct for storing POD texture data
******************************************************************************/
[StructLayout(LayoutKind.Sequential)]
public unsafe struct SPODTexture {
	public char	*pszName;			/*!< File-name of texture */
};

/*!****************************************************************************
 @public unsafe struct      SPODMaterial
 @Brief       public unsafe struct for storing POD material data
******************************************************************************/
[StructLayout(LayoutKind.Sequential)]
public unsafe struct SPODMaterial {
	public char		*pszName;				/*!< Name of material */
	public int		nIdxTexDiffuse;			/*!< Idx into pTexture for the diffuse texture */
	public int		nIdxTexAmbient;			/*!< Idx into pTexture for the ambient texture */
	public int		nIdxTexSpecularColour;	/*!< Idx into pTexture for the specular colour texture */
	public int		nIdxTexSpecularLevel;	/*!< Idx into pTexture for the specular level texture */
	public int		nIdxTexBump;			/*!< Idx into pTexture for the bump map */
	public int		nIdxTexEmissive;		/*!< Idx into pTexture for the emissive texture */
	public int		nIdxTexGlossiness;		/*!< Idx into pTexture for the glossiness texture */
	public int		nIdxTexOpacity;			/*!< Idx into pTexture for the opacity texture */
	public int		nIdxTexReflection;		/*!< Idx into pTexture for the reflection texture */
	public int		nIdxTexRefraction;		/*!< Idx into pTexture for the refraction texture */
	public float		fMatOpacity;			/*!< Material opacity (used with vertex alpha ?) */
	public fixed float		pfMatAmbient[3];		/*!< Ambient RGB value */
	public fixed float		pfMatDiffuse[3];		/*!< Diffuse RGB value */
	public fixed float		pfMatSpecular[3];		/*!< Specular RGB value */
	public float		fMatShininess;			/*!< Material shininess */
	public char		*pszEffectFile;			/*!< Name of effect file */
	public char		*pszEffectName;			/*!< Name of effect in the effect file */

	public EPODBlendFunc	eBlendSrcRGB;		/*!< Blending RGB source value */
	public EPODBlendFunc	eBlendSrcA;			/*!< Blending alpha source value */
	public EPODBlendFunc	eBlendDstRGB;		/*!< Blending RGB destination value */
	public EPODBlendFunc	eBlendDstA;			/*!< Blending alpha destination value */
	public EPODBlendOp		eBlendOpRGB;		/*!< Blending RGB operation */
	public EPODBlendOp		eBlendOpA;			/*!< Blending alpha operation */
	public fixed float		pfBlendColour[4];	/*!< A RGBA colour to be used in blending */
	public fixed float		pfBlendFactor[4];	/*!< An array of blend factors, one for each RGBA component */

	public uint		nFlags;				/*!< Stores information about the material e.g. Enable blending */

	public uint		nUserDataSize;
	public char		*pUserData;
};

/*!****************************************************************************
 @public unsafe struct      SPODScene
 @Brief       public unsafe struct for storing POD scene data
******************************************************************************/
[StructLayout(LayoutKind.Sequential)]
public unsafe struct SPODScene {
	public fixed float	pfColourBackground[3];		/*!< Background colour */
	public fixed float	pfColourAmbient[3];			/*!< Ambient colour */

	public uint		nNumCamera;				/*!< The length of the array pCamera */
	public SPODCamera		*pCamera;				/*!< Camera nodes array */

	public uint		nNumLight;				/*!< The length of the array pLight */
	public SPODLight		*pLight;				/*!< Light nodes array */

	public uint		nNumMesh;				/*!< The length of the array pMesh */
	public SPODMesh		*pMesh;					/*!< Mesh array. Meshes may be instanced several times in a scene; i.e. multiple Nodes may reference any given mesh. */

	public uint		nNumNode;		/*!< Number of items in the array pNode */
	public uint		nNumMeshNode;	/*!< Number of items in the array pNode which are objects */
	public SPODNode		*pNode;			/*!< Node array. Sorted as such: objects, lights, cameras, Everything Else (bones, helpers etc) */

	public uint		nNumTexture;	/*!< Number of textures in the array pTexture */
	public SPODTexture		*pTexture;		/*!< Texture array */

	public uint		nNumMaterial;	/*!< Number of materials in the array pMaterial */
	public SPODMaterial	*pMaterial;		/*!< Material array */

	public uint		nNumFrame;		/*!< Number of frames of animation */
	public uint		nFPS;			/*!< The frames per second the animation should be played at */

	public uint		nFlags;			/*!< PVRTMODELPODSF_* bit-flags */

	public uint		nUserDataSize;
	public char		*pUserData;
};