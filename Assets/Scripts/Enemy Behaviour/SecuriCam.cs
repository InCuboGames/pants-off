using UnityEngine;
using System.Collections;

[System.Serializable]
public class SecuriCam : MonoBehaviour
{
	#region Variáveis
	[SerializeField, HideInInspector]
	public int
		videoResolution;
	
	[System.NonSerialized]
	public float
		alcance, danoPorSegundo = 0;
	
	private GameObject player;
	
	public Transform camBase;
	public Transform eyes;
	
	delegate IEnumerator AIState();
	AIState ExecuteAIState;
	
	public float cycleTime = 5;
	private bool isWaiting, isPlayerSeen, isPantsSeen, isFaceSeen = false;

//	public GameObject FOV;
//	public Texture FOVexposureTex;
//	public Texture FOVocclusionTex;

	public Light2D fov;
	public Color exposureColor;
	public Color idleColor;

	Material FOVmat;
	[SerializeField] Material invisibleFOVmat;
	
	const float maxPlayerProximity = 2f;
	
	public Transform leftPos, rightPos, centerPos;
	
	bool movingLeft;
	bool tweening;
	
	public bool camoCam = false;
	bool followingPlayer = false;

	StageManager stageManager;

	bool playerInsideFOV = false;
	
	#endregion
	
	#region Behaviours do Unity
	// Use this for initialization
	void Awake()
	{
		#region Atribuição das Variáveis		
		
		#region Alcance (Visão)
		switch (videoResolution)
		{
		case 0:
			alcance = 5;
			break;
		case 1:
			alcance = 8;
			break;
		case 2:
			alcance = 11;
			break;
		case 3:
			alcance = 14;
			break;
		case 4:
			alcance = 17;
			break;
		}
		#endregion
		
		if(!camoCam)
		{
			danoPorSegundo = 10;
		}
		else
		{
			danoPorSegundo = 15;
		}
		
		#endregion
		
		
		#endregion
		player = GameObject.Find("Player");
		stageManager = GameObject.Find("Stage Manager").GetComponent<StageManager>();
		
//		FOV.transform.localScale = new Vector3(alcance * 3.845f, alcance * 3.845f, 1);
//		FOV.transform.localPosition = new Vector3(0, 0, eyes.transform.localPosition.z + (alcance * 3.845f) / 2);

		Light2D.RegisterEventListener(LightEventListenerType.OnStay, OnLightStay);
		Light2D.RegisterEventListener(LightEventListenerType.OnExit, OnLightExit);

		fov.LightRadius = alcance;

		FOVmat = fov.GetComponent<Light2D>().LightMaterial;
		
		if(stageManager.VA == 1)
		{
			fov.GetComponent<Light2D>().LightMaterial = invisibleFOVmat;
		}
		
		ExecuteAIState = Idle;
		return;
	}

	void OnDisable()
	{
		Light2D.UnregisterEventListener(LightEventListenerType.OnStay, OnLightStay);
		Light2D.UnregisterEventListener(LightEventListenerType.OnExit, OnLightExit);
	}
	
	// Update is called once per frame
	void Update()
	{
		VisualDetection();
		StartCoroutine(ExecuteAIState());
		
		if(!tweening && !followingPlayer)
		{
			if(movingLeft)
			{
				RotateCam(leftPos);
			}
			else
			{
				RotateCam(rightPos);
			}
		}
	}

	#region Eventos de FOV da Luz
	void OnLightStay(Light2D _light, GameObject _go)
	{
		/* Function called every LateUpdate if an object is in the light.
         * Use the _go object that is passed to determin if the current
         * gameObject is equal to the one this script is in (if needed) */
		if(player)
		{
			if (_go.tag == "Player" && _light.GetInstanceID() == fov.GetInstanceID())
			{
				playerInsideFOV = true;
				if(stageManager.VA == 1)
				{
					fov.GetComponent<Light2D>().LightMaterial = FOVmat;
				}
			}
		}
	}
	
	void OnLightExit(Light2D _light, GameObject _go)
	{
		/* Function called every LateUpdate if an object is in the light.
         * Use the _go object that is passed to determin if the current
         * gameObject is equal to the one this script is in (if needed) */
		if(player)
		{
			if (_go.tag == "Player" && _light.GetInstanceID() == fov.GetInstanceID())
			{
				playerInsideFOV = false;
				if(stageManager.VA == 1)
				{
					fov.GetComponent<Light2D>().LightMaterial = invisibleFOVmat;
				}
			}
		}
	}
	#endregion

	void LateUpdate()
	{
		if(isPlayerSeen)
		{
			player.GetComponent<PlayerControl>().beingSeen = true;
		}
	}

	
	#region Máquina de Estado
	
	#region Detecção Visual
	private void VisualDetection()
	{
		if (player)
		{
//			Vector3 forward = eyes.transform.TransformDirection(Vector3.forward);
//			Vector3 toOther = player.transform.position - eyes.transform.position;
//			float distance = Vector3.Distance(transform.position, player.transform.position);
			
//			if (Vector3.Dot(forward.normalized, toOther.normalized) > 0.8 && distance < alcance)
			if(playerInsideFOV)
			{
				Debug.DrawLine(eyes.position, GameObject.Find("Head").transform.position, Color.cyan);
				Debug.DrawLine(eyes.position, GameObject.Find("Pants").transform.position, Color.yellow);
				
				int pantsMask = ~((1 << GameObject.Find("Player").gameObject.layer) | (1 << GameObject.Find("Pants").gameObject.layer) | (1 << GameObject.Find("NPC").gameObject.layer));
				int headMask = ~((1 << GameObject.Find("Player").gameObject.layer) | (1 << GameObject.Find("Head").gameObject.layer) | (1 << GameObject.Find("NPC").gameObject.layer));
				
				if (!Physics.Linecast(eyes.position, GameObject.Find("Pants").transform.position, pantsMask))
				{
					isPantsSeen = true;
				} else
				{
					isPantsSeen = false;
				}
				
				if (!Physics.Linecast(eyes.position, GameObject.Find("Head").transform.position, headMask))
				{
					isFaceSeen = true;
				} else
				{
					isFaceSeen = false;
				}
			} else
			{
				isPantsSeen = isFaceSeen = isPlayerSeen = false;
			}
			
			if (isPantsSeen || isFaceSeen)
			{
				isPlayerSeen = true;
			} else if (!isPantsSeen && !isFaceSeen)
			{
				isPlayerSeen = false;
			}
		}
	}
	
	private void CheckPlayer()
	{
		if (!player.GetComponent<ShameMeter>().onDialogue)
		{
			if (isPlayerSeen)
			{
				if (isPantsSeen)
				{
					PantsDetection();
				}
			}
		} else
		{
			isPlayerSeen = false;
		}
	}
	
	private void PantsDetection()
	{
		#region Ver Cueca
		if(camoCam)
		{
			Debug.DrawLine(eyes.position, GameObject.Find("Pants").transform.position, Color.red);
			
//			if (FOV.renderer.material.mainTexture != FOVexposureTex)
//				FOV.renderer.material.mainTexture = FOVexposureTex;
			if(fov.LightColor != exposureColor)
			{
				fov.LightColor = exposureColor;
			}
			
			ExecuteAIState = Exposure;
			return;
		}
		else
		{
			if(Vector3.Distance(transform.position, player.transform.position) > maxPlayerProximity)
			{
				Debug.DrawLine(eyes.position, GameObject.Find("Pants").transform.position, Color.red);
				
//				if (FOV.renderer.material.mainTexture != FOVexposureTex)
//					FOV.renderer.material.mainTexture = FOVexposureTex;
				if(fov.LightColor != exposureColor)
				{
					fov.LightColor = exposureColor;
				}
				
				ExecuteAIState = Exposure;
				return;
			}
			else
			{
				ExecuteAIState = Idle;
				return;
			}
		}
		
		
		#endregion
	}
	#endregion
	
	#region Funções de Exposição e Despistamento
	
	private void DamagePlayerExposure()
	{
		player.GetComponent<ShameMeter>().shame += Time.deltaTime * danoPorSegundo;
	}
	#endregion
	
	#region Estados de Movimentação e Comportamento
	private IEnumerator Idle()
	{
//		if (FOV.renderer.material.mainTexture != FOVocclusionTex)
//			FOV.renderer.material.mainTexture = FOVocclusionTex;
		if(fov.LightColor != idleColor)
		{
			fov.LightColor = idleColor;
		}
		
		if(followingPlayer)
		{followingPlayer = false;
			RotateCam(centerPos);
		}
		
		if (player)
		{
			if (!player.GetComponent<ShameMeter>().onDialogue)
			{
				CheckPlayer();
			}
		}
		yield return null;
	}
	
	private IEnumerator Exposure()
	{
		if (!isPantsSeen || player.GetComponent<ShameMeter>().onDialogue)
		{
			ExecuteAIState = Idle;
			yield return null;
		} else
		{
			
//			if (FOV.renderer.material.mainTexture != FOVexposureTex)
//				FOV.renderer.material.mainTexture = FOVexposureTex;
			if(fov.LightColor != exposureColor)
			{
				fov.LightColor = exposureColor;
			}
			
			if(camoCam)
			{
				if (!followingPlayer) followingPlayer = true;
				if(tweening)
				{
					tweening = false;
					iTween.Stop(camBase.gameObject);
					movingLeft = false;
				}
				//		        var targetDir = Quaternion.LookRotation(player.transform.position - transform.position);
				//		        camBase.rotation = Quaternion.Slerp(transform.rotation, targetDir, 0.5f);
				camBase.LookAt(player.transform.position);
			}
			
			DamagePlayerExposure();
			
		}  
	}
	
	#endregion
	
	#endregion
	
	void RotateCam(Transform pos)
	{
		tweening = true;
		iTween.RotateTo(camBase.gameObject, iTween.Hash ("rotation", pos, 
		                                                 "time", cycleTime, 
		                                                 "easetype", iTween.EaseType.easeInOutCubic, 
		                                                 "oncomplete", "WaitCycle",
		                                                 "oncompletetarget", gameObject));
	}
	
	IEnumerator WaitCycle()
	{
		yield return new WaitForSeconds(cycleTime/3);
		movingLeft = !movingLeft;
		tweening = false;
	}
}