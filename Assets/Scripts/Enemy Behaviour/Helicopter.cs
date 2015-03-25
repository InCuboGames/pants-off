using UnityEngine;
using System.Collections;

[System.Serializable]
public class Helicopter : MonoBehaviour
{
	#region Variáveis
	[System.NonSerialized]
	private GameObject player;

	[System.NonSerialized]
	public float
		alcance, danoPorSegundo = 0;

	public Transform eyes;
	
	delegate IEnumerator AIState();
	AIState ExecuteAIState;
	
	private bool isWaiting, isPlayerSeen, isPantsSeen, isFaceSeen = false;
	
//	public GameObject FOV;
//	public Texture FOVexposureTex;
//	public Texture FOVocclusionTex;

	public Light2D fov;
	public Color exposureColor;
	public Color idleColor;

	
	bool playerInsideFOV = false;

	public float rotationSpeed = 25;
	public Transform stageCenter;
	
	#endregion
	
	#region Behaviours do Unity
	// Use this for initialization
	void Awake()
	{
		#region Atribuição das Variáveis		
		
		#region Alcance (Visão)
		alcance = 100;

		danoPorSegundo = 20;
		#endregion

		#endregion

		player = GameObject.Find("Player");
		
//		FOV.transform.localScale = new Vector3(alcance * 3.845f, alcance * 3.845f, 1);
//		FOV.transform.localPosition = new Vector3(0, 0, eyes.transform.localPosition.z + (alcance * 3.845f) / 2);

		Light2D.RegisterEventListener(LightEventListenerType.OnStay, OnLightStay);
		Light2D.RegisterEventListener(LightEventListenerType.OnExit, OnLightExit);
		
		fov.LightRadius = alcance;
		
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

		transform.RotateAround(stageCenter.position, Vector3.up, rotationSpeed * Time.deltaTime);
	}

	void LateUpdate()
	{
		if(isPlayerSeen)
		{
			player.GetComponent<PlayerControl>().beingSeen = true;
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
			}
		}
	}
	#endregion
	
	#region Máquina de Estado
	
	#region Detecção Visual
	private void VisualDetection()
	{
		if (player)
		{
//			Vector3 forward = eyes.transform.TransformDirection(Vector3.forward);
//			Vector3 toOther = player.transform.position - eyes.transform.position;
//			float distance = Vector3.Distance(transform.position, player.transform.position);
			
			//if (Vector3.Dot(forward.normalized, toOther.normalized) > 0.8 && distance < alcance)
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
				PantsDetection();
			}
		} else
		{
			isPlayerSeen = false;
		}
	}
	
	private void PantsDetection()
	{
		#region Ver Cueca
				Debug.DrawLine(eyes.position, GameObject.Find("Pants").transform.position, Color.red);
				
					//			if (FOV.renderer.material.mainTexture != FOVexposureTex)
					//				FOV.renderer.material.mainTexture = FOVexposureTex;
					if(fov.LightColor != exposureColor)
					{
						fov.LightColor = exposureColor;
					}

				
				ExecuteAIState = Exposure;
				return;

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
		if (!isPlayerSeen || player.GetComponent<ShameMeter>().onDialogue)
		{
			ExecuteAIState = Idle;
			yield return null;
		} else
		{
			
			//				if (FOV.renderer.material.mainTexture != FOVexposureTex)
			//					FOV.renderer.material.mainTexture = FOVexposureTex;
			if(fov.LightColor != exposureColor)
			{
				fov.LightColor = exposureColor;
			}
            
            DamagePlayerExposure();
            
        }  
    }
    
    #endregion
    
    #endregion

	#endregion
}