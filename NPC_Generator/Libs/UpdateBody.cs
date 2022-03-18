using JetBrains.Annotations;
using UnityEngine;

namespace NPC_Generator.Libs
{
	public static class UpdateBody
	{
		public static void UpdateBodyModel(VisEquipment visE)
		{
			NPC_Generator.DebugLog(NPC_Generator.DebugLevel.All,"Updating body model");
			NPC_Generator.DebugLog(NPC_Generator.DebugLevel.All,$"Model Index: {visE.m_modelIndex}");
			if (visE != null)
			{
				if (visE.m_currentModelIndex == 0)
				{
					visE.m_bodyModel.sharedMesh = NPC_Generator.maleSmr?.sharedMesh;
					visE.m_bodyModel.materials[0].shader = Shader.Find("Custom/Player");
					visE.m_bodyModel.materials[0].SetTexture("_MainTex", NPC_Generator.maleSkin);
					visE.m_bodyModel.materials[0].SetTexture("_SkinBumpMap", NPC_Generator.maleSkinBump);
					visE.m_bodyModel.materials[0].SetFloat("_Glossiness", 0f);
					visE.m_bodyModel.materials[0].SetFloat("_MetalGlossiness", 0f);
				}
				if (visE.m_currentModelIndex == 1)
				{
					visE.m_bodyModel.sharedMesh = NPC_Generator.femaleSmr?.sharedMesh;
					visE.m_bodyModel.materials[0].shader = Shader.Find("Custom/Player");
					visE.m_bodyModel.materials[0].SetTexture("_MainTex", NPC_Generator.femaleSkin);
					visE.m_bodyModel.materials[0].SetTexture("_SkinBumpMap", NPC_Generator.femaleSkinBump);
					visE.m_bodyModel.materials[0].SetFloat("_Glossiness", 0f);
					visE.m_bodyModel.materials[0].SetFloat("_MetalGlossiness", 0f);
				}
			}
		}
	}
}

