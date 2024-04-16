using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodFile
{
    public const uint  PVRTMODELPOD_TAG_MASK = 0x80000000;
    public const uint  PVRTMODELPOD_TAG_START = 0x00000000;
    public const uint  PVRTMODELPOD_TAG_END = 0x80000000;

    public bool Read(ref PodScene scene, string path)
    {
        PodReader reader = new PodReader(path);

        uint name = 0, len = 0;

        while (reader.ReadMarker(ref name, ref len))
        {
            switch (name)
            {
                case (uint)PODFileName.PODFileScene:
                    if (!ReadScene(ref reader, ref scene))
                    {
                        return false;
                    }
                    break;

                //case (uint)PODFileName.PODFileScene | PVRTMODELPOD_TAG_END:
                //    return true;

                default:
                    if (!reader.Skip(len))
                    {
                        return false;
                    }
                    break;
            }
        }

		reader.Dispose();

        return true;
    }

    public bool ReadScene(ref PodReader reader, ref PodScene scene)
    {
        uint name = 0, len = 0;
        uint cameras = 0, lights = 0, materials= 0, meshes = 0, nodes = 0, textures = 0;

        scene = new PodScene();

        while (reader.ReadMarker(ref name, ref len))
        {
            switch (name)
            {
                case (uint)PODFileName.PODFileScene | PVRTMODELPOD_TAG_END:
                    return true;

		        case (uint)PODFileName.PODFileColourBackground:	if(!reader.ReadFloatArray(ref scene.colourBackground, 3))return false;           break;
		        case (uint)PODFileName.PODFileColourAmbient:	if(!reader.ReadFloatArray(ref scene.colourAmbient, 3))return false;                break;

		        case (uint)PODFileName.PODFileNumCamera:		if(!reader.ReadUInt(ref scene.numCamera))return false; scene.camera = new PodCamera[scene.numCamera];                         break;
		        case (uint)PODFileName.PODFileNumLight:			if(!reader.ReadUInt(ref scene.numLight))return false;     scene.light = new PodLight[scene.numLight];                            break;
		        case (uint)PODFileName.PODFileNumMesh:			if(!reader.ReadUInt(ref scene.numMesh))return false;       scene.mesh = new PodMesh[scene.numMesh];                           break;
		        case (uint)PODFileName.PODFileNumNode:			if(!reader.ReadUInt(ref scene.numNode))return false;               scene.node = new PodNode[scene.numNode];                   break;
		        case (uint)PODFileName.PODFileNumMeshNode:		if(!reader.ReadUInt(ref scene.numMeshNode))return false;                           break;
		        case (uint)PODFileName.PODFileNumTexture:		if(!reader.ReadUInt(ref scene.numTexture))return false;       scene.texture = new PodTexture[scene.numTexture];                        break;
		        case (uint)PODFileName.PODFileNumMaterial:		if(!reader.ReadUInt(ref scene.numMaterial))return false;       scene.material = new PodMaterial[scene.numMaterial];                       break;
		        case (uint)PODFileName.PODFileNumFrame:			if(!reader.ReadUInt(ref scene.numFrame))return false;                             break;

		        case (uint)PODFileName.PODFileFPS:				if(!reader.ReadUInt(ref scene.fPS))return false;                                   break;
		        case (uint)PODFileName.PODFileFlags:			if(!reader.ReadUInt(ref scene.flags))return false;	                                break;

		        case (uint)PODFileName.PODFileCamera:	    if(!ReadCamera(ref reader, ref scene.camera[cameras++]))return false;		        break;
		        case (uint)PODFileName.PODFileLight:	    if(!ReadLight(ref reader, ref scene.light[lights++]))return false;			        break;
		        case (uint)PODFileName.PODFileMaterial:	    if(!ReadMaterial(ref reader, ref scene.material[materials++]))return false;	    break;
		        case (uint)PODFileName.PODFileMesh:		    if(!ReadMesh(ref reader, ref scene.mesh[meshes++]))return false;			        break;
		        case (uint)PODFileName.PODFileNode:		    if(!ReadNode(ref reader, ref scene.node[nodes++]))return false;				    break;
		        case (uint)PODFileName.PODFileTexture:	    if(!ReadTexture(ref reader, ref scene.texture[textures++]))return false;	        break;

                case (uint)PODFileName.PODFileMatUserData:
			        if(!reader.ReadString(ref scene.userData, len))
                    {
			        	return false;
                    }
			        else
			        {
			        	scene.userDataSize = len;
			        	break;
			        }

                default:
                    if (!reader.Skip(len))
                    {
                        return false;
                    }
                    break;
            }
        }

        return false;
    }

    public bool ReadCamera(ref PodReader reader, ref PodCamera camera)
    {
        uint name = 0, len = 0;

        camera = new PodCamera();

        while (reader.ReadMarker(ref name, ref len))
        {
            switch(name)
		    {
		    case (uint)PODFileName.PODFileCamera | PVRTMODELPOD_TAG_END:			return true;

		    case (uint)PODFileName.PODFileCamIdxTgt:		if(!reader.ReadUInt(ref camera.idxTarget)) return false;					break;
		    case (uint)PODFileName.PODFileCamFOV:		if(!reader.ReadFloat(ref camera.fOV)) return false;							break;
		    case (uint)PODFileName.PODFileCamFar:		if(!reader.ReadFloat(ref camera.far)) return false;							break;
		    case (uint)PODFileName.PODFileCamNear:		if(!reader.ReadFloat(ref camera.near)) return false;						break;
		    case (uint)PODFileName.PODFileCamAnimFOV:	if(!reader.ReadFloatArrayAlloc(ref camera.animFOV, len)) return false;	break;

            default:
                if (!reader.Skip(len))
                {
                    return false;
                }
                break;
		    }
        }

        return false;
    }

    public bool ReadLight(ref PodReader reader, ref PodLight light)
    {
        uint name = 0, len = 0;

        light = new PodLight();

        while (reader.ReadMarker(ref name, ref len))
        {
            switch (name)
            {
            case (uint)PODFileName.PODFileLight | PVRTMODELPOD_TAG_END:			return true;

		    case (uint)PODFileName.PODFileLightIdxTgt:	if(!reader.ReadUInt(ref light.idxTarget)) return false;	break;
		    case (uint)PODFileName.PODFileLightColour:	if(!reader.ReadFloatArray(ref light.colour, 3)) return false;		break;
		    case (uint)PODFileName.PODFileLightType:		if(!reader.ReadUInt(ref light.type)) return false;		break;
		    case (uint)PODFileName.PODFileLightConstantAttenuation: 		if(!reader.ReadFloat(ref light.constantAttenuation))	return false;	break;
		    case (uint)PODFileName.PODFileLightLinearAttenuation:		if(!reader.ReadFloat(ref light.linearAttenuation))		return false;	break;
		    case (uint)PODFileName.PODFileLightQuadraticAttenuation:		if(!reader.ReadFloat(ref light.quadraticAttenuation))	return false;	break;
		    case (uint)PODFileName.PODFileLightFalloffAngle:				if(!reader.ReadFloat(ref light.falloffAngle))			return false;	break;
		    case (uint)PODFileName.PODFileLightFalloffExponent:			if(!reader.ReadFloat(ref light.falloffExponent))		return false;	break;

            default:
                if (!reader.Skip(len))
                {
                    return false;
                }
                break;
            }
        }

        return false;
    }

    public bool ReadMaterial(ref PodReader reader, ref PodMaterial material)
    {        
        uint name = 0, len = 0;
        
        material = new PodMaterial();

        while (reader.ReadMarker(ref name, ref len))
        {
            switch (name)
            {
            case (uint)PODFileName.PODFileMaterial | PVRTMODELPOD_TAG_END:			return true;

            case (uint)PODFileName.PODFileMatFlags:					if(!reader.ReadUInt(ref material.flags)) return false;				break;
		    case (uint)PODFileName.PODFileMatName:					if(!reader.ReadString(ref material.name, len)) return false;		break;
		    case (uint)PODFileName.PODFileMatIdxTexDiffuse:			if(!reader.ReadUInt(ref material.idxTexDiffuse)) return false;				break;
		    case (uint)PODFileName.PODFileMatIdxTexAmbient:			if(!reader.ReadUInt(ref material.idxTexAmbient)) return false;				break;
		    case (uint)PODFileName.PODFileMatIdxTexSpecularColour:	if(!reader.ReadUInt(ref material.idxTexSpecularColour)) return false;		break;
		    case (uint)PODFileName.PODFileMatIdxTexSpecularLevel:	if(!reader.ReadUInt(ref material.idxTexSpecularLevel)) return false;			break;
		    case (uint)PODFileName.PODFileMatIdxTexBump:				if(!reader.ReadUInt(ref material.idxTexBump)) return false;					break;
		    case (uint)PODFileName.PODFileMatIdxTexEmissive:			if(!reader.ReadUInt(ref material.idxTexEmissive)) return false;				break;
		    case (uint)PODFileName.PODFileMatIdxTexGlossiness:		if(!reader.ReadUInt(ref material.idxTexGlossiness)) return false;			break;
		    case (uint)PODFileName.PODFileMatIdxTexOpacity:			if(!reader.ReadUInt(ref material.idxTexOpacity)) return false;				break;
		    case (uint)PODFileName.PODFileMatIdxTexReflection:		if(!reader.ReadUInt(ref material.idxTexReflection)) return false;			break;
		    case (uint)PODFileName.PODFileMatIdxTexRefraction:		if(!reader.ReadUInt(ref material.idxTexRefraction)) return false;			break;
		    case (uint)PODFileName.PODFileMatOpacity:		if(!reader.ReadFloat(ref material.matOpacity)) return false;						break;
		    case (uint)PODFileName.PODFileMatAmbient:		if(!reader.ReadFloatArray(ref material.matAmbient,  3)) return false;		break;
		    case (uint)PODFileName.PODFileMatDiffuse:		if(!reader.ReadFloatArray(ref material.matDiffuse,  3)) return false;		break;
		    case (uint)PODFileName.PODFileMatSpecular:		if(!reader.ReadFloatArray(ref material.matSpecular, 3)) return false;		break;
		    case (uint)PODFileName.PODFileMatShininess:		if(!reader.ReadFloat(ref material.matShininess)) return false;					break;
		    case (uint)PODFileName.PODFileMatEffectFile:		if(!reader.ReadString(ref material.effectFile, len)) return false;	break;
		    case (uint)PODFileName.PODFileMatEffectName:		if(!reader.ReadString(ref material.effectName, len)) return false;	break;
		    case (uint)PODFileName.PODFileMatBlendSrcRGB:	if(!reader.ReadUInt(ref material.blendSrcRGB))	return false;	break;
		    case (uint)PODFileName.PODFileMatBlendSrcA:		if(!reader.ReadUInt(ref material.blendSrcA))		return false;	break;
		    case (uint)PODFileName.PODFileMatBlendDstRGB:	if(!reader.ReadUInt(ref material.blendDstRGB))	return false;	break;
		    case (uint)PODFileName.PODFileMatBlendDstA:		if(!reader.ReadUInt(ref material.blendDstA))		return false;	break;
		    case (uint)PODFileName.PODFileMatBlendOpRGB:		if(!reader.ReadUInt(ref material.blendOpRGB))	return false;	break;
		    case (uint)PODFileName.PODFileMatBlendOpA:		if(!reader.ReadUInt(ref material.blendOpA))		return false;	break;
		    case (uint)PODFileName.PODFileMatBlendColour:	if(!reader.ReadFloatArray(ref material.blendColour, 4))	return false;	break;
		    case (uint)PODFileName.PODFileMatBlendFactor:	if(!reader.ReadFloatArray(ref material.blendFactor, 4))	return false;	break;

            case (uint)PODFileName.PODFileMatUserData:
			    if(!reader.ReadString(ref material.userData, len))
			    	return false;
			    else
			    {
			    	material.userDataSize = len;
			    	break;
			    }

            default:
                if (!reader.Skip(len))
                {
                    return false;
                }
                break;
            }
            
        }

        return false;
    }

    public bool ReadMesh(ref PodReader reader, ref PodMesh mesh)
    {
        uint name = 0, len = 0, uvws = 0;

        mesh = new PodMesh();

        while (reader.ReadMarker(ref name, ref len))
        {
            switch (name)
            {
            case (uint)PODFileName.PODFileMesh | PVRTMODELPOD_TAG_END:
				DeinterleaveMesh(ref mesh);
			    return true;

		    case (uint)PODFileName.PODFileMeshNumVtx:			if(!reader.ReadUInt(ref mesh.numVertex)) return false;													break;
		    case (uint)PODFileName.PODFileMeshNumFaces:			if(!reader.ReadUInt(ref mesh.numFaces)) return false;													break;
		    case (uint)PODFileName.PODFileMeshNumUVW:			if(!reader.ReadUInt(ref mesh.numUVW)) return false; mesh.uVW = new PodData[mesh.numUVW];	break;
		    case (uint)PODFileName.PODFileMeshStripLength:		if(!reader.ReadUIntArrayAlloc(ref mesh.stripLength, len)) return false;								break;
		    case (uint)PODFileName.PODFileMeshNumStrips:			if(!reader.ReadUInt(ref mesh.numStrips)) return false;													break;
		    case (uint)PODFileName.PODFileMeshInterleaved:		if(!reader.ReadByteArray(ref mesh.interleaved, len)) return false;									break;
		    case (uint)PODFileName.PODFileMeshBoneBatches:		if(!reader.ReadIntArrayAlloc(ref mesh.boneBatches.batches, len)) return false;						break;
		    case (uint)PODFileName.PODFileMeshBoneBatchBoneCnts:	if(!reader.ReadIntArrayAlloc(ref mesh.boneBatches.batchBoneCnt, len)) return false;					break;
		    case (uint)PODFileName.PODFileMeshBoneBatchOffsets:	if(!reader.ReadIntArrayAlloc(ref mesh.boneBatches.batchOffset, len)) return false;					break;
		    case (uint)PODFileName.PODFileMeshBoneBatchBoneMax:	if(!reader.ReadInt(ref mesh.boneBatches.batchBoneMax)) return false;									break;
		    case (uint)PODFileName.PODFileMeshBoneBatchCnt:		if(!reader.ReadInt(ref mesh.boneBatches.batchCnt)) return false;										break;
		    case (uint)PODFileName.PODFileMeshUnpackMatrix:		if(!reader.ReadFloatArray(ref mesh.unpackMatrix, 16)) return false;										break;

		    case (uint)PODFileName.PODFileMeshFaces:			if(!ReadPodData(ref reader, ref mesh.faces, PODFileName.PODFileMeshFaces)) return false;							break;
		    case (uint)PODFileName.PODFileMeshVtx:			if(!ReadPodData(ref reader, ref mesh.vertex, PODFileName.PODFileMeshVtx)) return false;			break;
		    case (uint)PODFileName.PODFileMeshNor:			if(!ReadPodData(ref reader, ref mesh.normals, PODFileName.PODFileMeshNor)) return false;			break;
		    case (uint)PODFileName.PODFileMeshTan:			if(!ReadPodData(ref reader, ref mesh.tangents, PODFileName.PODFileMeshTan)) return false;			break;
		    case (uint)PODFileName.PODFileMeshBin:			if(!ReadPodData(ref reader, ref mesh.binormals, PODFileName.PODFileMeshBin)) return false;			break;
		    case (uint)PODFileName.PODFileMeshUVW:			if(!ReadPodData(ref reader, ref mesh.uVW[uvws++], PODFileName.PODFileMeshUVW)) return false;		break;
		    case (uint)PODFileName.PODFileMeshVtxCol:		if(!ReadPodData(ref reader, ref mesh.vtxColours, PODFileName.PODFileMeshVtxCol)) return false;		break;
		    case (uint)PODFileName.PODFileMeshBoneIdx:		if(!ReadPodData(ref reader, ref mesh.boneIdx, PODFileName.PODFileMeshBoneIdx)) return false;		break;
		    case (uint)PODFileName.PODFileMeshBoneWeight:	if(!ReadPodData(ref reader, ref mesh.boneWeight, PODFileName.PODFileMeshBoneWeight)) return false;	break;

            default:
                if (!reader.Skip(len))
                {
                    return false;
                }
                break;
            }
        }

        return false;
    }

    public bool ReadNode(ref PodReader reader, ref PodNode node)
    {
        uint name = 0, len = 0;
        bool oldNodeFormat = false;

        node = new PodNode();

        float[] pos = new float[3];
        float[] rot = new float[4];
        float[] scale = new float[3];

        while (reader.ReadMarker(ref name, ref len))
        {
            switch (name)
            {
		        case (uint)PODFileName.PODFileNode | PVRTMODELPOD_TAG_END:
		        	if(oldNodeFormat)
		        	{
		        		if(node.animPosition != null)
		        			node.animFlags |= (uint)PODAnimationData.PODHasPositionAni;
		        		else
		        		{
		        			node.animPosition = pos;
		        		}

		        		if(node.animRotation != null)
		        			node.animFlags |= (uint)PODAnimationData.PODHasRotationAni;
		        		else
		        		{
		        			node.animRotation = rot;
		        		}

		        		if(node.animScale != null)
		        			node.animFlags |= (uint)PODAnimationData.PODHasScaleAni;
		        		else
		        		{
		        			node.animScale = scale;
		        		}
		        	}
		        	return true;

		        case (uint)PODFileName.PODFileNodeIdx:		if(!reader.ReadUInt(ref node.idx)) return false;								break;
		        case (uint)PODFileName.PODFileNodeName:		if(!reader.ReadString(ref node.name, len)) return false;			break;
		        case (uint)PODFileName.PODFileNodeIdxMat:	if(!reader.ReadUInt(ref node.idxMaterial)) return false;						break;
		        case (uint)PODFileName.PODFileNodeIdxParent:	if(!reader.ReadUInt(ref node.idxParent)) return false;						break;
		        case (uint)PODFileName.PODFileNodeAnimFlags:if(!reader.ReadUInt(ref node.animFlags))return false;							break;

		        case (uint)PODFileName.PODFileNodeAnimPosIdx:	if(!reader.ReadUIntArrayAlloc(ref node.animPositionIdx, len)) return false;	break;
		        case (uint)PODFileName.PODFileNodeAnimPos:	if(!reader.ReadFloatArrayAlloc(ref node.animPosition, len)) return false;	break;

		        case (uint)PODFileName.PODFileNodeAnimRotIdx:	if(!reader.ReadUIntArrayAlloc(ref node.animRotationIdx, len)) return false;	break;
		        case (uint)PODFileName.PODFileNodeAnimRot:	if(!reader.ReadFloatArrayAlloc(ref node.animRotation, len)) return false;	break;

		        case (uint)PODFileName.PODFileNodeAnimScaleIdx:	if(!reader.ReadUIntArrayAlloc(ref node.animScaleIdx, len)) return false;	break;
		        case (uint)PODFileName.PODFileNodeAnimScale:	if(!reader.ReadFloatArrayAlloc(ref node.animScale, len)) return false;		break;

		        case (uint)PODFileName.PODFileNodeAnimMatrixIdx:	if(!reader.ReadUIntArrayAlloc(ref node.animMatrixIdx, len)) return false;	break;
		        case (uint)PODFileName.PODFileNodeAnimMatrix:if(!reader.ReadFloatArrayAlloc(ref node.animMatrix, len)) return false;	break;

		        case (uint)PODFileName.PODFileNodeUserData:
		        	if(!reader.ReadString(ref node.userData, len))
		        		return false;
		        	else
		        	{
		        		node.userDataSize = len;
		        		break;
		        	}

		        // Parameters from the older pod format
		        case (uint)PODFileName.PODFileNodePos:		if(!reader.ReadFloatArray(ref pos, 3))   return false;		oldNodeFormat = true;		break;
		        case (uint)PODFileName.PODFileNodeRot:		if(!reader.ReadFloatArray(ref rot, 4))  return false;		oldNodeFormat = true;		break;
		        case (uint)PODFileName.PODFileNodeScale:		if(!reader.ReadFloatArray(ref scale,3)) return false;		oldNodeFormat = true;		break;

                default:
                    if (!reader.Skip(len))
                    {
                        return false;
                    }
                    break;
            }
        }

        return false;
    }

    public bool ReadTexture (ref PodReader reader, ref PodTexture texture)
    {
        uint name = 0, len = 0;

        texture = new PodTexture();

	    while(reader.ReadMarker(ref name, ref len))
	    {
	    	switch(name)
	    	{
	    	case (uint)PODFileName.PODFileTexture | PVRTMODELPOD_TAG_END:			return true;

	    	case (uint)PODFileName.PODFileTexName:		if(!reader.ReadString(ref texture.name, len)) return false;			break;

            default:
                if (!reader.Skip(len))
                {
                    return false;
                }
                break;
	    	}
	    }

        return false;
    }

    public bool ReadPodData (ref PodReader reader, ref PodData data, PODFileName spec)
    {
        uint name = 0, len = 0;

        data = new PodData();

        while (reader.ReadMarker(ref name, ref len))
        {
            if (name == ((uint)spec | PVRTMODELPOD_TAG_END)) return true;

            switch (name)
            {
            case (uint)PODFileName.PODFileDataType:	if(!reader.ReadUInt(ref data.type)) return false;					break;
		    case (uint)PODFileName.PODFileN:			if(!reader.ReadUInt(ref data.n)) return false;						break;
		    case (uint)PODFileName.PODFileStride:	if(!reader.ReadUInt(ref data.stride)) return false;					break;
		    case (uint)PODFileName.PODFileData:
		    	switch(PVRTModelPODDataTypeSize((PVRTDataType)data.type))
		    	{
                    case 4:
                    case 2:
		    		case 1: if(!reader.ReadByteArray(ref data.data, len)) return false; break;
		    		//case 2:
		    		//	{ // reading 16bit data but have 8bit pointer
		    		//		//PVRTuint16 *p16Pointer=NULL;
		    		//		//if(!reader.ReadAfterAlloc16(p16Pointer, nLen)) return false;
		    		//		//ref data.pData = (unsigned char*)p16Pointer;
		    		//		break;
		    		//	}
		    		//case 4:
		    		//	{ // reading 32bit data but have 8bit pointer
		    		//		PVRTuint32 *p32Pointer=NULL;
		    		//		if(!reader.ReadAfterAlloc32(p32Pointer, nLen)) return false;
		    		//		ref data.pData = (unsigned char*)p32Pointer;
		    		//		break;
		    		//	}
		    		//default:
		    		//	{ _ASSERT(false);}
		    	}
		    break;
            }
        }

        return false;
    }

	public void DeinterleaveMesh(ref PodMesh mesh)
	{
		DeinterleaveArray(ref mesh.interleaved, ref mesh.vertex, mesh.numVertex);
		DeinterleaveArray(ref mesh.interleaved, ref mesh.normals, mesh.numVertex);
		DeinterleaveArray(ref mesh.interleaved, ref mesh.tangents, mesh.numVertex);
		DeinterleaveArray(ref mesh.interleaved, ref mesh.binormals, mesh.numVertex);

		for (int i = 0; i < mesh.numUVW; i++)
		{
			DeinterleaveArray(ref mesh.interleaved, ref mesh.uVW[i], mesh.numVertex);
		}

		DeinterleaveArray(ref mesh.interleaved, ref mesh.vtxColours, mesh.numVertex);
		DeinterleaveArray(ref mesh.interleaved, ref mesh.boneIdx, mesh.numVertex);
		DeinterleaveArray(ref mesh.interleaved, ref mesh.boneWeight, mesh.numVertex);
	}

	public void DeinterleaveArray(ref byte[] interleaved, ref PodData data, uint length)
	{
		if (data.n == 0 || data.stride == 0)
		{
			return;
		}

		uint offset = BitConverter.ToUInt32(data.data);
		long len = length * data.n * PVRTModelPODDataTypeSize((PVRTDataType)data.type);

		uint index = 0;

		data.data = new byte[len];

		for (uint i = offset; i < interleaved.Length; i+=data.stride)
		{
			for (int j = 0; j < data.n * PVRTModelPODDataTypeSize((PVRTDataType)data.type); j++)
			{
				data.data[index++] = interleaved[i + j];
			}
		}
	}

    public uint PVRTModelPODDataTypeSize(PVRTDataType type)
    {
    	switch(type)
    	{
    	default:
    		return 0;
    	case PVRTDataType.PODDataFloat:
    		return sizeof(float);
    	case PVRTDataType.PODDataInt:
    	case PVRTDataType.PODDataUnsignedInt:
    		return sizeof(int);
    	case PVRTDataType.PODDataShort:
    	case PVRTDataType.PODDataShortNorm:
    	case PVRTDataType.PODDataUnsignedShort:
    	case PVRTDataType.PODDataUnsignedShortNorm:
    		return sizeof(ushort);
    	case PVRTDataType.PODDataRGBA:
    		return sizeof(uint);
    	case PVRTDataType.PODDataARGB:
    		return sizeof(uint);
    	case PVRTDataType.PODDataD3DCOLOR:
    		return sizeof(uint);
    	case PVRTDataType.PODDataUBYTE4:
    		return sizeof(uint);
    	case PVRTDataType.PODDataDEC3N:
    		return sizeof(uint);
    	case PVRTDataType.PODDataFixed16_16:
    		return sizeof(uint);
    	case PVRTDataType.PODDataUnsignedByte:
    	case PVRTDataType.PODDataUnsignedByteNorm:
    	case PVRTDataType.PODDataByte:
    	case PVRTDataType.PODDataByteNorm:
    		return sizeof(char);
    	}
    }
}