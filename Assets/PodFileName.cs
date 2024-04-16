public enum PODFileName
{
	PODFileVersion				= 1000,
	PODFileScene,
	PODFileExpOpt,
	PODFileHistory,
	PODFileEndiannessMisMatch  = -402456576,

	PODFileColourBackground	= 2000,
	PODFileColourAmbient,
	PODFileNumCamera,
	PODFileNumLight,
	PODFileNumMesh,
	PODFileNumNode,
	PODFileNumMeshNode,
	PODFileNumTexture,
	PODFileNumMaterial,
	PODFileNumFrame,
	PODFileCamera,		// Will come multiple times
	PODFileLight,		// Will come multiple times
	PODFileMesh,		// Will come multiple times
	PODFileNode,		// Will come multiple times
	PODFileTexture,	// Will come multiple times
	PODFileMaterial,	// Will come multiple times
	PODFileFlags,
	PODFileFPS,
	PODFileUserData,

	PODFileMatName				= 3000,
	PODFileMatIdxTexDiffuse,
	PODFileMatOpacity,
	PODFileMatAmbient,
	PODFileMatDiffuse,
	PODFileMatSpecular,
	PODFileMatShininess,
	PODFileMatEffectFile,
	PODFileMatEffectName,
	PODFileMatIdxTexAmbient,
	PODFileMatIdxTexSpecularColour,
	PODFileMatIdxTexSpecularLevel,
	PODFileMatIdxTexBump,
	PODFileMatIdxTexEmissive,
	PODFileMatIdxTexGlossiness,
	PODFileMatIdxTexOpacity,
	PODFileMatIdxTexReflection,
	PODFileMatIdxTexRefraction,
	PODFileMatBlendSrcRGB,
	PODFileMatBlendSrcA,
	PODFileMatBlendDstRGB,
	PODFileMatBlendDstA,
	PODFileMatBlendOpRGB,
	PODFileMatBlendOpA,
	PODFileMatBlendColour,
	PODFileMatBlendFactor,
	PODFileMatFlags,
	PODFileMatUserData,
	PODFileTexName				= 4000,
	PODFileNodeIdx				= 5000,
	PODFileNodeName,
	PODFileNodeIdxMat,
	PODFileNodeIdxParent,
	PODFileNodePos,
	PODFileNodeRot,
	PODFileNodeScale,
	PODFileNodeAnimPos,
	PODFileNodeAnimRot,
	PODFileNodeAnimScale,
	PODFileNodeMatrix,
	PODFileNodeAnimMatrix,
	PODFileNodeAnimFlags,
	PODFileNodeAnimPosIdx,
	PODFileNodeAnimRotIdx,
	PODFileNodeAnimScaleIdx,
	PODFileNodeAnimMatrixIdx,
	PODFileNodeUserData,

	PODFileMeshNumVtx			= 6000,
	PODFileMeshNumFaces,
	PODFileMeshNumUVW,
	PODFileMeshFaces,
	PODFileMeshStripLength,
	PODFileMeshNumStrips,
	PODFileMeshVtx,
	PODFileMeshNor,
	PODFileMeshTan,
	PODFileMeshBin,
	PODFileMeshUVW,			// Will come multiple times
	PODFileMeshVtxCol,
	PODFileMeshBoneIdx,
	PODFileMeshBoneWeight,
	PODFileMeshInterleaved,
	PODFileMeshBoneBatches,
	PODFileMeshBoneBatchBoneCnts,
	PODFileMeshBoneBatchOffsets,
	PODFileMeshBoneBatchBoneMax,
	PODFileMeshBoneBatchCnt,
	PODFileMeshUnpackMatrix,

	PODFileLightIdxTgt			= 7000,
	PODFileLightColour,
	PODFileLightType,
	PODFileLightConstantAttenuation,
	PODFileLightLinearAttenuation,
	PODFileLightQuadraticAttenuation,
	PODFileLightFalloffAngle,
	PODFileLightFalloffExponent,

	PODFileCamIdxTgt			= 8000,
	PODFileCamFOV,
	PODFileCamFar,
	PODFileCamNear,
	PODFileCamAnimFOV,

	PODFileDataType			= 9000,
	PODFileN,
	PODFileStride,
	PODFileData
};

public enum PODLightType
{
	PODPoint=0,	 /*!< Point light */
	PODDirectional, /*!< Directional light */
	PODSpot,		 /*!< Spot light */
	NumPODLightTypes
};

public enum PODPrimitiveType
{
	PODTriangles=0, /*!< Triangles */
	NumPODPrimitiveTypes
};

public enum PODAnimationData
{
	PODHasPositionAni	= 0x01,	/*!< Position animation */
	PODHasRotationAni	= 0x02, /*!< Rotation animation */
	PODHasScaleAni		= 0x04, /*!< Scale animation */
	PODHasMatrixAni	= 0x08  /*!< Matrix animation */
};

public enum PODMaterialFlag
{
	PODEnableBlending	= 0x01,	/*!< Enable blending for this material */
};

public enum PODBlendFunc
{
	PODBlendFunc_ZERO=0,
	PODBlendFunc_ONE,
	PODBlendFunc_BLEND_FACTOR,
	PODBlendFunc_ONE_MINUS_BLEND_FACTOR,

	PODBlendFunc_SRC_COLOR = 0x0300,
	PODBlendFunc_ONE_MINUS_SRC_COLOR,
	PODBlendFunc_SRC_ALPHA,
	PODBlendFunc_ONE_MINUS_SRC_ALPHA,
	PODBlendFunc_DST_ALPHA,
	PODBlendFunc_ONE_MINUS_DST_ALPHA,
	PODBlendFunc_DST_COLOR,
	PODBlendFunc_ONE_MINUS_DST_COLOR,
	PODBlendFunc_SRC_ALPHA_SATURATE,

	PODBlendFunc_CONSTANT_COLOR = 0x8001,
	PODBlendFunc_ONE_MINUS_CONSTANT_COLOR,
	PODBlendFunc_CONSTANT_ALPHA,
	PODBlendFunc_ONE_MINUS_CONSTANT_ALPHA
};

public enum PODBlendOp
{
	PODBlendOp_ADD = 0x8006,
	PODBlendOp_MIN,
	PODBlendOp_MAX,
	PODBlendOp_SUBTRACT = 0x800A,
	PODBlendOp_REVERSE_SUBTRACT,
};

public enum PVRTDataType
{
	PODDataNone,
	PODDataFloat,
	PODDataInt,
	PODDataUnsignedShort,
	PODDataRGBA,
	PODDataARGB,
	PODDataD3DCOLOR,
	PODDataUBYTE4,
	PODDataDEC3N,
	PODDataFixed16_16,
	PODDataUnsignedByte,
	PODDataShort,
	PODDataShortNorm,
	PODDataByte,
	PODDataByteNorm,
	PODDataUnsignedByteNorm,
	PODDataUnsignedShortNorm,
	PODDataUnsignedInt
};