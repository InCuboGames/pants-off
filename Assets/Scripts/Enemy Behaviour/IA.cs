using UnityEngine;
using System.Collections;

[System.Serializable]
public class IA : MonoBehaviour
{
    #region Variáveis
	[SerializeField, HideInInspector]
	public int
		sexo = 0;
    [SerializeField, HideInInspector]
    public int
        personalidade = 0;
    [SerializeField, HideInInspector]
    public int
        humor = 0;
    [SerializeField, HideInInspector]
    public int
        visao = 2;
    [SerializeField, HideInInspector]
    public int
        cargo = 0;
    public bool memoria = false;

    public string arquetipo
    {
        get;
        set;
    }

    [System.NonSerialized]
    public float
        atencao, persistencia, paciencia, insistencia, alcance, danoPorSegundo, danoPorErro = 0;

    public GameObject questionWindow;

    public bool dialogueSuccess
    {
        get;
        set;
    }

    private GameObject player;

    private NavMeshAgent navAgent;
    public GameObject waypointPath;
    private GameObject bathroomPoint;
    private Transform[] aWaypoints;
    private int currentWaypoint;

    public bool wanderPath = false;
    public bool randomizePath = false;

    delegate IEnumerator AIState();
    AIState ExecuteAIState;

    private float wanderWaitTime = 0;
    private bool isWaiting, isPlayerSeen, isPantsSeen, isFaceSeen = false;

    private float reactionTime = 0;
    public GameObject reactionIcon;
    public Texture detectionTex;
    public Texture expositionTex;
    public Texture occlusionTex;
    public Texture scaredTex;

    public GameObject gizmoPrefab;
    private GameObject LastKnownPosition;
    private bool LKPset = false;

    private float lookAroundTime = 0;
    private int randomRotWaypoint;

	[SerializeField] Light2D fov;
	[SerializeField] Transform eyes;
	Material FOVmat;
	[SerializeField] Material invisibleFOVmat;

//    GameObject FOV;
//    public Texture FOVdetectionTex;
//    public Texture FOVexpositionTex;
//    public Texture FOVocclusionTex;
//    public Texture FOVscaredTex;
	public Color detectionColor;
	public Color expositionColor;
	public Color memoryColor;
	public Color scaredColor;

    
    //public Camera playerCam;
    private bool outwitted = false;
    public Transform diagCamAnchor;

    StageManager stageManager;

    [HideInInspector]public Animator anim;
	[SerializeField] AnimationClip histericalScream;

    float originalSpeed;

	[SerializeField]
	GameObject keyboardIcon;

	bool histTweening = false;
	
	const float maxPlayerProximity = 2f;

	bool exposedPlayer = false;

	bool playerInsideFOV = false;

	#region Visual Elements

	[SerializeField]
		GameObject[] maleModels;

	[SerializeField]
		Transform[] maleHairs;

	[SerializeField]
		GameObject[] femaleModels;

	[SerializeField]
		Transform[] femaleHairs;

	[SerializeField]
	Transform[] maleHeadPivots;

	[SerializeField]
	Transform[] femaleHeadPivots;

	[SerializeField]
	Color[] hairColors;

	GameObject currentModel;
	Transform currentHair;
	Transform currentHeadPivot;
	GameObject currentGravata;
	GameObject currentGlasses;
	GameObject currentSnorkel;
	GameObject currentEarmuff;
	GameObject currentNightvision;
	GameObject currentWitchHat;

	[SerializeField]
	Texture[]
	estagiarioCorpos, estagiariaCorpos,
	empregadoCorpos, empregadaCorpos,
	veteranoCorpos, veteranaCorpos,
	gerenteCorpos, gerentaCorpos,
	diretorCorpos, diretoraCorpos;

	[SerializeField]
	Texture[]
	estagiarioFaces, estagiariaFaces,
	empregadoFaces, empregadaFaces,
	veteranoFaces, veteranaFaces,
	gerenteFaces, gerentaFaces,
	diretorFaces, diretoraFaces;

	[SerializeField]
	Color[] personalityColors;

	[SerializeField]
	GameObject[]
	estagiarioGlasses, estagiariaGlasses,
	empregadoGlasses, empregadaGlasses,
	veteranoGlasses, veteranaGlasses,
	gerenteGlasses, gerentaGlasses,
	diretorGlasses, diretoraGlasses;

	[SerializeField]
	GameObject[] ties;

	[SerializeField]
	GameObject[] maleSnorkels;
	[SerializeField]
	GameObject[] femaleSnorkels;

	[SerializeField]
	GameObject[] maleNightvisions;
	[SerializeField]
	GameObject[] femaleNightvisions;

	[SerializeField]
	GameObject[] maleEarmuffs;
	[SerializeField]
	GameObject[] femaleEarmuffs;

//	[SerializeField]
//	GameObject[] malePartyHats;
//	[SerializeField]
//	GameObject[] femalePartyHats;
//
//	[SerializeField]
//	GameObject[] maleHalloweenAcessories;
//	[SerializeField]
//	GameObject[] femaleHalloweenAcessories;

	[SerializeField]
	AudioClip maleSurprise, femaleSurprise;

	[SerializeField]
	AudioClip maleScream, femaleScream;
	#endregion

    #endregion

    #region Behaviours do Unity
    // Use this for initialization
    void Awake()
    {
        #region Atribuição das Variáveis do Arquétipo			

//        #region Atribuição de Nomes de Arquétipo
//
//        //Defaults to the standard name
//        arquetipo = "Um Funcionário Qualquer";
//
//        if (personalidade == 0 && humor == 0 && visao == 0 && cargo == 0)
//        {
//            //Definir Arquétipos
//        }
//        #endregion

        #region Atenção e Persistência (Personalidade)
        switch (personalidade)
        {
            case 0:
                atencao = 1;
                persistencia = 6;
				
                break;
            case 1:
                atencao = 2;
                persistencia = 3;
                break;
            case 2:
                atencao = 0.5f;
                persistencia = 10;
                break;
            case 3:
                atencao = 1;
                persistencia = 3;
                break;
            case 4:
                atencao = 1;
                persistencia = 8;
                break;
        }
        #endregion

        #region Paciência e Insistência (Humor)
        switch (humor)
        {
            case 0:
                paciencia = 7;
                insistencia = 3;
                break;
            case 1:
                paciencia = 10;
                insistencia = 4;
                break;
            case 2:
                paciencia = 8;
                insistencia = 2;
                break;
            case 3:
                paciencia = 4;
                insistencia = 5;
                break;
            case 4:
                paciencia = 6;
                insistencia = 1;
                break;
        }
        #endregion

        #region Alcance (Visão)
        switch (visao)
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

        #region Dano por Segundo e Dano por Erro (Cargo)
        switch (cargo)
        {
            case 0:
                danoPorSegundo = 5;
                danoPorErro = 10;
                break;
            case 1:
                danoPorSegundo = 10;
                danoPorErro = 20;
                break;
            case 2:
                danoPorSegundo = 15;
                danoPorErro = 30;
                break;
            case 3:
                danoPorSegundo = 20;
                danoPorErro = 40;
                break;
            case 4:
                danoPorSegundo = 25;
                danoPorErro = 50;
                break;
        }
        #endregion


		#region Construir Visual
		if(sexo == 0)
		{
			sexo = (int)Random.Range(1,3);
		}

		currentModel = sexo == 1? maleModels[cargo] : femaleModels[cargo];
		currentHair = sexo == 1? maleHairs[cargo] : femaleHairs[cargo];
		currentHeadPivot = sexo == 1? maleHeadPivots[cargo] : femaleHeadPivots[cargo];
		currentGravata = sexo == 1? ties[cargo] : null;
		currentSnorkel = sexo == 1? maleSnorkels[cargo] : femaleSnorkels[cargo];

		foreach (GameObject model in maleModels)
		{
			model.SetActive(model == currentModel? true : false);
		}
		foreach (GameObject model in femaleModels)
		{
			model.SetActive(model == currentModel? true : false);
		}
		
		anim = currentModel.GetComponent<Animator>();

		#region Atribuição da cor da roupa (Personalidade)
		switch (cargo)
		{
			case 0:
			if(sexo == 1)
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = estagiarioCorpos[personalidade];
			}
			else
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = estagiariaCorpos[personalidade];
			}
			break;
		case 1:
			if(sexo == 1)
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = empregadoCorpos[personalidade];
			}
			else
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = empregadaCorpos[personalidade];
			}
			break;
		case 2:
			if(sexo == 1)
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = veteranoCorpos[personalidade];
			}
			else
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = veteranaCorpos[personalidade];
			}
			break;
		case 3:
			if(sexo == 1)
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = gerenteCorpos[personalidade];
			}
			else
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = gerentaCorpos[personalidade];
			}
			break;
		case 4:
			if(sexo == 1)
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = diretorCorpos[personalidade];
			}
			else
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = diretoraCorpos[personalidade];
			}
			break;
		}

		if(currentGravata) currentGravata.GetComponent<SkinnedMeshRenderer>().material.color = personalityColors[personalidade];
		#endregion

		//Atribuição da cor do cabelo (Personalidade)
		currentHair.GetComponentInChildren<SkinnedMeshRenderer>().material.color = hairColors[personalidade];
		//Fix de multiplos materials de cabelo
//		if(cargo == 2 && sexo == 1)
//		{
//			currentHair.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = hairColors[personalidade];
//		}

		#region Atribuição da expressão facial (Humor)
		switch (cargo)
		{
		case 0:
			if(sexo == 1)
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].mainTexture = estagiarioFaces[humor];
			}
			else
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].mainTexture = estagiariaFaces[humor];
			}
			break;
		case 1:
			if(sexo == 1)
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].mainTexture = empregadoFaces[humor];
			}
			else
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].mainTexture = empregadaFaces[humor];
			}
			break;
		case 2:
			if(sexo == 1)
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].mainTexture = veteranoFaces[humor];
			}
			else
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].mainTexture = veteranaFaces[humor];
			}
			break;
		case 3:
			if(sexo == 1)
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].mainTexture = gerenteFaces[humor];
			}
			else
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].mainTexture = gerentaFaces[humor];
			}
			break;
		case 4:
			if(sexo == 1)
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].mainTexture = diretorFaces[humor];
			}
			else
			{
				currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].mainTexture = diretoraFaces[humor];
			}
			break;
		}
		#endregion

		#region Atribuição de models de Óculos (Visão)
		currentGlasses = null;

		switch (cargo)
		{
		case 0:
			if(sexo == 1)
			{
				if(visao <2)
				{
					currentGlasses = estagiarioGlasses[visao];
				}
				else if (visao > 2)
				{
					currentGlasses = estagiarioGlasses[visao - 1];
				}

				foreach (GameObject model in estagiarioGlasses)
				{
					model.SetActive(model == currentGlasses? true : false);
				}
				if(currentGlasses)
				{
					currentGlasses.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = personalityColors[personalidade];
				}
			}
			else
			{
				if(visao <2)
				{
					currentGlasses = estagiariaGlasses[visao];
				}
				else if (visao > 2)
				{
					currentGlasses = estagiariaGlasses[visao - 1];
				}
				
				foreach (GameObject model in estagiariaGlasses)
				{
					model.SetActive(model == currentGlasses? true : false);
				}
				if(currentGlasses)
				{
					currentGlasses.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = personalityColors[personalidade];
				}
			}
			break;
		case 1:
			if(sexo == 1)
			{
				if(visao <2)
				{
					currentGlasses = empregadoGlasses[visao];
				}
				else if (visao > 2)
				{
					currentGlasses = empregadoGlasses[visao - 1];
				}
				
				foreach (GameObject model in empregadoGlasses)
				{
					model.SetActive(model == currentGlasses? true : false);
				}
				if(currentGlasses)
				{
					currentGlasses.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = personalityColors[personalidade];
				}
			}
			else
			{
				if(visao <2)
				{
					currentGlasses = empregadaGlasses[visao];
				}
				else if (visao > 2)
				{
					currentGlasses = empregadaGlasses[visao - 1];
				}
				
				foreach (GameObject model in empregadaGlasses)
				{
					model.SetActive(model == currentGlasses? true : false);
				}
				if(currentGlasses)
				{
					currentGlasses.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = personalityColors[personalidade];
				}
			}
			break;
		case 2:
			if(sexo == 1)
			{
				if(visao <2)
				{
					currentGlasses = veteranoGlasses[visao];
				}
				else if (visao > 2)
				{
					currentGlasses = veteranoGlasses[visao - 1];
				}
				
				foreach (GameObject model in veteranoGlasses)
				{
					model.SetActive(model == currentGlasses? true : false);
				}
				if(currentGlasses)
				{
					currentGlasses.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = personalityColors[personalidade];
				}
			}
			else
			{
				if(visao <2)
				{
					currentGlasses = veteranaGlasses[visao];
				}
				else if (visao > 2)
				{
					currentGlasses = veteranaGlasses[visao - 1];
				}
				
				foreach (GameObject model in veteranaGlasses)
				{
					model.SetActive(model == currentGlasses? true : false);
				}
				if(currentGlasses)
				{
					currentGlasses.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = personalityColors[personalidade];
				}
			}
			break;
		case 3:
			if(sexo == 1)
			{
				if(visao <2)
				{
					currentGlasses = gerenteGlasses[visao];
				}
				else if (visao > 2)
				{
					currentGlasses = gerenteGlasses[visao - 1];
				}
				
				foreach (GameObject model in gerenteGlasses)
				{
					model.SetActive(model == currentGlasses? true : false);
				}
				if(currentGlasses)
				{
					currentGlasses.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = personalityColors[personalidade];
				}
			}
			else
			{
				if(visao <2)
				{
					currentGlasses = gerentaGlasses[visao];
				}
				else if (visao > 2)
				{
					currentGlasses = gerentaGlasses[visao - 1];
				}
				
				foreach (GameObject model in gerentaGlasses)
				{
					model.SetActive(model == currentGlasses? true : false);
				}
				if(currentGlasses)
				{
					currentGlasses.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = personalityColors[personalidade];
				}
			}
			break;
		case 4:
			if(sexo == 1)
			{
				if(visao <2)
				{
					currentGlasses = diretorGlasses[visao];
				}
				else if (visao > 2)
				{
					currentGlasses = diretorGlasses[visao - 1];
				}
				
				foreach (GameObject model in diretorGlasses)
				{
					model.SetActive(model == currentGlasses? true : false);
				}
				if(currentGlasses)
				{
					currentGlasses.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = personalityColors[personalidade];
				}
			}
			else
			{
				if(visao <2)
				{
					currentGlasses = diretoraGlasses[visao];
				}
				else if (visao > 2)
				{
					currentGlasses = diretoraGlasses[visao - 1];
				}
				
				foreach (GameObject model in diretoraGlasses)
				{
					model.SetActive(model == currentGlasses? true : false);
				}
				if(currentGlasses)
				{
					currentGlasses.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = personalityColors[personalidade];
				}
			}
			break;
		}
		#endregion

		//Cor do bigode
		if(cargo == 3 && sexo == 1)
		{
			currentModel.GetComponentInChildren<SkinnedMeshRenderer>().materials[2].color = hairColors[personalidade];
		}

		//Substituiçao de animação histérica
		if(personalidade == 4)
		{
			AnimatorOverrideController overrideAnim = new AnimatorOverrideController();
			overrideAnim.runtimeAnimatorController = anim.runtimeAnimatorController;
			
			overrideAnim["Scare"] = histericalScream;
		}
		#endregion


        #endregion

        player = GameObject.Find("Player");

        bathroomPoint = GameObject.Find("BathroomPoint");
		stageManager = GameObject.Find("Stage Manager").GetComponent<StageManager>();

        navAgent = GetComponent<NavMeshAgent>();
        originalSpeed = navAgent.speed;

        aWaypoints = waypointPath.GetComponentsInChildren<Transform>();

        if (randomizePath)
        {
            currentWaypoint = Random.Range(0, aWaypoints.Length);
        } else
        {
            currentWaypoint = 0;
        }

        reactionIcon.renderer.enabled = false;
        reactionTime = 0;
        wanderWaitTime = 0;

		fov.LightRadius = alcance;

		FOVmat = fov.GetComponent<Light2D>().LightMaterial;

		//Variacao Ambiental
		if(stageManager.VA == 1)
		{
			fov.GetComponent<Light2D>().LightMaterial = invisibleFOVmat;

			if(sexo == 1)
			{
				currentNightvision = maleNightvisions[cargo];
			}
			else
			{
				currentNightvision = femaleNightvisions[cargo];
			}
			currentNightvision.SetActive(true);
			currentNightvision.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = personalityColors[personalidade];
			currentNightvision.GetComponentInChildren<Light>().color = personalityColors[personalidade];

			if(currentGlasses)currentGlasses.SetActive(false);
		}
		else if(stageManager.VA == 2)
		{
			navAgent.acceleration = 2;

			if(sexo == 1)
			{
				currentEarmuff = maleEarmuffs[cargo];
			}
			else
			{
				currentEarmuff = femaleEarmuffs[cargo];
			}
			currentEarmuff.SetActive(true);
			currentEarmuff.GetComponentInChildren<SkinnedMeshRenderer>().material.color = personalityColors[personalidade];
		}
		else if(stageManager.VA == 3)
		{
			originalSpeed = originalSpeed/2;
			navAgent.speed = originalSpeed;
			anim.speed = 0.7f;

			if(sexo == 1)
			{
				currentSnorkel = maleSnorkels[cargo];
			}
			else
			{
				currentSnorkel = femaleSnorkels[cargo];
			}
			currentSnorkel.SetActive(true);
			if(sexo == 1)
			{
				if(cargo != 0 && cargo != 2 && cargo != 4)
				{
					currentSnorkel.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = personalityColors[personalidade];
				}
				else if (cargo == 0 || cargo == 2 || cargo == 4)
				{
					currentSnorkel.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = personalityColors[personalidade];
				}
			}
			else
			{
				currentSnorkel.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = personalityColors[personalidade];
			}

			if(currentGlasses)currentGlasses.SetActive(false);
		}

		Light2D.RegisterEventListener(LightEventListenerType.OnStay, OnLightStay);
		Light2D.RegisterEventListener(LightEventListenerType.OnExit, OnLightExit);

        dialogueSuccess = false;

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
        keyboardIcon.gameObject.SetActive
            (player && Vector3.Distance(transform.position, player.transform.position) <= navAgent.stoppingDistance - 1 && 
             ExecuteAIState == Exposure);

        bool chasing = (ExecuteAIState == Exposure || ExecuteAIState == GoToLastKnowPosition || ExecuteAIState == LookAroundLastKnowPosition);

        if (anim)
        {
            anim.SetBool("chasing", chasing);
            anim.SetFloat("movement", navAgent.velocity.magnitude);
            anim.SetBool("playerSeen", isPlayerSeen);
        }

        VisualDetection();
        StartCoroutine(ExecuteAIState());

		eyes.transform.rotation = currentHeadPivot.transform.rotation;
    }
    #endregion

	#region Eventos de FOV da Luz
	void OnLightStay(Light2D _light, GameObject _go)
	{
		/* Function called every LateUpdate if an object is in the light.
         * Use the _go object that is passed to determin if the current
         * gameObject is equal to the one this script is in (if needed) */
		if (_go.tag == "Player" && _light.GetInstanceID() == fov.GetInstanceID())
		{
			playerInsideFOV = true;
			if(stageManager.VA == 1)
			{
				fov.GetComponent<Light2D>().LightMaterial = FOVmat;
			}
		}
	}

	void OnLightExit(Light2D _light, GameObject _go)
	{
		/* Function called every LateUpdate if an object is in the light.
         * Use the _go object that is passed to determin if the current
         * gameObject is equal to the one this script is in (if needed) */
		if (_go.tag == "Player" && _light.GetInstanceID() == fov.GetInstanceID())
		{
			playerInsideFOV = false;
			if(stageManager.VA == 1)
			{
				fov.GetComponent<Light2D>().LightMaterial = invisibleFOVmat;
			}
		}
	}
	#endregion


	void LateUpdate()
	{
		if(isPantsSeen || isPlayerSeen && memoria)
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
//            Vector3 forward = eyes.transform.TransformDirection(Vector3.forward);
//            Vector3 toOther = player.transform.position - eyes.transform.position;
//            float distance = Vector3.Distance(transform.position, player.transform.position);

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
                if (isPantsSeen && !memoria)
                {
                    PantsDetection();
                } else if (isFaceSeen && memoria)
                {
                    FaceRecognition();
                }
            } else
            {
                if (memoria)
                {
//                    if (FOV.renderer.material.mainTexture != FOVdetectionTex)
//                        FOV.renderer.material.mainTexture = FOVdetectionTex;
					if(fov.LightColor != memoryColor)
					{
						fov.LightColor = memoryColor;
					}
                }
				else
				{
					if(reactionTime > 0)
					{
						if(fov.LightColor != detectionColor)
						{
							fov.LightColor = detectionColor;
						}
					}
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
        Debug.DrawLine(eyes.position, GameObject.Find("Pants").transform.position, Color.red);

        if (reactionIcon.renderer.material.mainTexture != detectionTex)
            reactionIcon.renderer.material.mainTexture = detectionTex;
        reactionIcon.renderer.enabled = true;

//        if (FOV.renderer.material.mainTexture != FOVdetectionTex)
//            FOV.renderer.material.mainTexture = FOVdetectionTex;
		if(fov.LightColor != detectionColor)
		{
			fov.LightColor = detectionColor;
		}

        if (reactionTime <= atencao)
        {
            reactionTime += Time.deltaTime;
            var percent = (reactionTime / atencao);
            reactionIcon.renderer.material.SetFloat("_Cutoff", Mathf.Clamp(1 - percent, 0.001f, 1));

        } else
        {
            memoria = true;

            ExecuteAIState = Exposure;
            return;
        }
        #endregion
    }

    private void FaceRecognition()
    {
        #region Ver Rosto
        Debug.DrawLine(eyes.position, GameObject.Find("Head").transform.position, Color.red);

        ExecuteAIState = Exposure;
        return;
        #endregion
    }
    #endregion

    #region Funções de Exposição e Despistamento
    private void SetLKP()
    {
        LastKnownPosition = Instantiate(gizmoPrefab) as GameObject;
        LastKnownPosition.transform.position = player.transform.position;
        LastKnownPosition.transform.rotation = player.transform.rotation;
//        if(personalidade != 4)
//        {
//            LastKnownPosition.transform.localPosition += LastKnownPosition.transform.forward * persistencia / 2;
//        }
        lookAroundTime = 0;
        LKPset = true;
    }

    private void Outwit()
    {
        outwitted = true;

        LastKnownPosition = Instantiate(gizmoPrefab) as GameObject;
        LastKnownPosition.transform.position = transform.position;
        LastKnownPosition.transform.rotation = transform.rotation;
        lookAroundTime = 0;
//      LKPset = true;
    }

    private void ResetLKP()
    {
        Destroy(LastKnownPosition);
        lookAroundTime = 0;
        LKPset = false;
    }

    private void DamagePlayerExposure()
    {
        player.GetComponent<ShameMeter>().shame += Time.deltaTime * danoPorSegundo;
    }

    public void DamagePlayerDialogue()
    {
        player.GetComponent<ShameMeter>().shame += danoPorErro;
    }
    #endregion

    #region Estados de Movimentação e Comportamento
    private IEnumerator Idle()
    {
        if (player)
        {
            if (!player.GetComponent<ShameMeter>().onDialogue)
            {
                CheckPlayer();
            }

			if(Vector3.Distance(transform.position, player.transform.position) <= maxPlayerProximity)
            {
                var targetDir = Quaternion.LookRotation(player.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetDir, 0.2f);
            }
        }

        if (navAgent.stoppingDistance != 1)
            navAgent.stoppingDistance = 1;
        if (navAgent.speed != originalSpeed)
            navAgent.speed = originalSpeed;

		if (player && !isWaiting && Vector3.Distance(transform.position, player.transform.position) > maxPlayerProximity)
        {
            navAgent.SetDestination(aWaypoints [currentWaypoint].position);

            float wpDist = Vector3.Distance(transform.position, aWaypoints [currentWaypoint].position);
            if (wpDist <= navAgent.stoppingDistance)
            {
                if (wanderPath)
                {
                    if (!isWaiting)
                    {
                        isWaiting = true;
                    }
                }

                if (randomizePath)
                {
                    randomRotWaypoint = currentWaypoint;
                    currentWaypoint = Random.Range(0, aWaypoints.Length);
                } else
                {
                    currentWaypoint++;
                    if (currentWaypoint >= aWaypoints.Length)
                    {
                        currentWaypoint = 0;
                    }
                }
            }
		} else if (isWaiting && Vector3.Distance(transform.position, player.transform.position) > maxPlayerProximity)
        {
            if (wanderWaitTime >= (persistencia / 2))
            {
                isWaiting = false;
                wanderWaitTime = 0;
            } else
            {
                wanderWaitTime += Time.deltaTime;

                if (!randomizePath)
                {
                    var rotWaypoint = 0;
                    if (currentWaypoint <= 0)
                    {
                        rotWaypoint = aWaypoints.Length - 1;
                    } else
                    {
                        rotWaypoint = currentWaypoint - 1;
                    }

                    transform.rotation = Quaternion.Slerp(transform.rotation, aWaypoints [rotWaypoint].rotation, 0.05f);
                } else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, aWaypoints [randomRotWaypoint].rotation, 0.05f);
                }
            }
        } 
        yield return null;
    }

    private IEnumerator Exposure()
    {
        ResetLKP();

        if (!isPlayerSeen || player.GetComponent<ShameMeter>().onDialogue)
        {
            ExecuteAIState = GoToLastKnowPosition;
            yield return null;
        } else
        {
            if(personalidade != 4)
            {
                if (reactionIcon.renderer.material.mainTexture != expositionTex)
                    reactionIcon.renderer.material.mainTexture = expositionTex;
                if (reactionIcon.renderer.enabled != true)
                    reactionIcon.renderer.enabled = true;
                reactionIcon.renderer.material.SetFloat("_Cutoff", 1);

//                if (FOV.renderer.material.mainTexture != FOVexpositionTex)
//                    FOV.renderer.material.mainTexture = FOVexpositionTex;
				if(fov.LightColor != expositionColor)
				{
					fov.LightColor = expositionColor;
				}

                if (navAgent.stoppingDistance != 3)
                    navAgent.stoppingDistance = 3;
                if (navAgent.speed != originalSpeed / 1.5f)
                    navAgent.speed = originalSpeed / 1.5f;

                var targetDir = Quaternion.LookRotation(player.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetDir, 0.2f);

//                Quaternion fixedAngle = transform.localRotation;
//                fixedAngle.y = 0;
//                eyes.transform.localRotation = fixedAngle;  

                if (!exposedPlayer)
                {
					exposedPlayer = true;
                    stageManager.timesSpotted++;
                    player.GetComponent<PlayerControl>().exposedBy = this;
                    player.GetComponent<PlayerControl>().exposed = true;
					GetComponent<AudioSource>().PlayOneShot(sexo == 1 ? maleSurprise : femaleSurprise);

                    if(anim)
                    {
                        anim.SetBool("exposure", true);
                    }
                }

                navAgent.Stop();

                yield return new WaitForSeconds(0.7f);

                if(anim)
                {
                    anim.SetBool("exposure",false);
                }

                navAgent.SetDestination(player.transform.position);

                float dist = Vector3.Distance(player.transform.position, transform.position);

                if (dist <= navAgent.stoppingDistance - 0.5f && !player.GetComponent<ShameMeter>().onDialogue && Input.GetKeyDown(KeyCode.E))
                {
                    ExecuteAIState = Dialogue;
                    yield return null;
                }
                DamagePlayerExposure();

            }
            else
            {
                if(player.GetComponent<PlayerControl>().crouching)
                {
                    Camera.main.GetComponent<MouseOrbit>().yMaxLimit = Camera.main.GetComponent<MouseOrbit>().yMinLimit = 65;
                    Camera.main.GetComponent<MouseOrbit>().distance = 9;
                }
                    HistericalTween();

                if (reactionIcon.renderer.material.mainTexture != expositionTex)
                    reactionIcon.renderer.material.mainTexture = expositionTex;
                if (reactionIcon.renderer.enabled != true)
                    reactionIcon.renderer.enabled = true;
                reactionIcon.renderer.material.SetFloat("_Cutoff", 1);

//                if (FOV.renderer.material.mainTexture != FOVexpositionTex)
//                    FOV.renderer.material.mainTexture = FOVexpositionTex;
				if(fov.LightColor != expositionColor)
				{
					fov.LightColor = expositionColor;
				}

                if(isPlayerSeen)
					if (!exposedPlayer)
                {
					exposedPlayer = true;
                    stageManager.timesSpotted++;
                    player.GetComponent<PlayerControl>().exposedBy = this;
                    player.GetComponent<PlayerControl>().exposed = true;

					if(anim)
					{
						anim.SetBool("exposure", true);
					}
                }

                foreach (IA npc in stageManager.arrayNPC)
                {
                    if (npc != this)
                    {
                        npc.GetComponent<NavMeshAgent>().Stop();
                    }
                }

                navAgent.Stop();

                yield return new WaitForSeconds(1.5f);

				if(anim)
				{
					anim.SetBool("exposure",false);
				}

				Camera.main.GetComponent<PauseManager>().cameraTweening = false;
                
                if(ExecuteAIState != HideInBathroom)
                {
                   	Camera.main.GetComponent<AudioSource>().PlayOneShot(sexo == 1 ? maleScream : femaleScream);
					stageManager.timesSpotted++;
                    ExecuteAIState = HideInBathroom;
                }

                if (!LKPset)
                {
                    SetLKP();
                    
                    foreach (IA npc in stageManager.arrayNPC)
                    {
                        if (npc != this)
                        {
                            if(npc.ExecuteAIState != npc.Exposure())
                            {
                                npc.SetLKP();
								npc.exposedPlayer = true;
                                npc.LastKnownPosition.transform.position = LastKnownPosition.transform.position;
                                npc.SearchPosition();
                            }
                        }
                    }
                    
                    DamagePlayerDialogue();
                }
                
                yield return new WaitForSeconds(1f);
                
                if (player.GetComponent<PlayerControl>().exposed)
                {
                    player.GetComponent<PlayerControl>().exposed = false;
                    player.GetComponent<PlayerControl>().exposedBy = null;
                }
                player.GetComponent<PlayerControl>().canControl = true;
                Camera.main.GetComponent<MouseOrbit>().enabled = true;
                histTweening = false;
                Destroy(GameObject.Find("lookPos"));
                StopAllCoroutines();
                yield return null;
            }
        }  
    }
    
    private IEnumerator Dialogue()
    {   
        if (!player.GetComponent<ShameMeter>().onDialogue && !player.GetComponent<ShameMeter>().gameOver)
        {
                Screen.lockCursor = false;

                player.GetComponent<ShameMeter>().onDialogue = true;
                player.GetComponent<PlayerControl>().canControl = false;

                Camera.main.GetComponent<MouseOrbit>().enabled = false;
                Camera.main.transform.position = diagCamAnchor.position;
                Camera.main.transform.rotation = diagCamAnchor.rotation;

                Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("FOVs"));

                StartCoroutine(CastQuestion());
        }

        if (player.GetComponent<ShameMeter>().onDialogue)
        {
            if (dialogueSuccess)
            {
                Screen.lockCursor = true;

                Camera.main.GetComponent<MouseOrbit>().enabled = true;
                player.GetComponent<PlayerControl>().exposed = false;
                player.GetComponent<PlayerControl>().canControl = true;
                player.GetComponent<ShameMeter>().onDialogue = false;

                questionWindow.SetActive(false);
                dialogueSuccess = false;

                player.GetComponent<PlayerControl>().StandUp();
                Outwit();
				anim.SetTrigger("correctAnswer");
                ExecuteAIState = LookAroundLastKnowPosition;
                yield return null;
            }
        }

        yield return null;
    }

    private IEnumerator GoToLastKnowPosition()
    {
        if (player.GetComponent<PlayerControl>().exposed)
        {
            player.GetComponent<PlayerControl>().exposed = false;
            player.GetComponent<PlayerControl>().exposedBy = null;
        }

        if(player)
        {
            CheckPlayer();

			if(Vector3.Distance(transform.position, player.transform.position) <= maxPlayerProximity)
            {
                var targetDir = Quaternion.LookRotation(player.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetDir, 0.2f);
            }
        }

        if (navAgent.stoppingDistance != 1f)
            navAgent.stoppingDistance = 1f;
        if (navAgent.speed != originalSpeed)
            navAgent.speed = originalSpeed;

        if (!LKPset)
        {
            SetLKP();
        }

        if (reactionIcon.renderer.material.mainTexture != detectionTex)
            reactionIcon.renderer.material.mainTexture = detectionTex;
//        if (FOV.renderer.material.mainTexture != FOVdetectionTex)
//            FOV.renderer.material.mainTexture = FOVdetectionTex;
		if(fov.LightColor != memoryColor)
		{
			fov.LightColor = memoryColor;
		}

        var percent = (reactionTime / atencao);
        reactionIcon.renderer.material.SetFloat("_Cutoff", Mathf.Clamp(1 - percent, 0.001f, 1));


        float lkpDist = Vector3.Distance(transform.position, LastKnownPosition.transform.position);
        if (lkpDist <= navAgent.stoppingDistance)
        {
            ExecuteAIState = LookAroundLastKnowPosition;
            yield return null;
        } else
        {
            navAgent.SetDestination(LastKnownPosition.transform.position);
        }

        yield return null;
    }

    private IEnumerator HideInBathroom()
    {
        if (navAgent.stoppingDistance != 1f)
            navAgent.stoppingDistance = 1f;
        if (navAgent.speed != originalSpeed)
            navAgent.speed = originalSpeed;
        
        if (reactionIcon.renderer.material.mainTexture != scaredTex)
            reactionIcon.renderer.material.mainTexture = scaredTex;
        reactionIcon.renderer.material.SetFloat("_Cutoff", 0.001f);
//        if (FOV.renderer.material.mainTexture != FOVscaredTex)
//            FOV.renderer.material.mainTexture = FOVscaredTex;
		if(fov.LightColor != scaredColor)
		{
			fov.LightColor = scaredColor;
		}
        
        float lkpDist = Vector3.Distance(transform.position, bathroomPoint.transform.position);
        if (lkpDist <= navAgent.stoppingDistance)
        {
            ExecuteAIState = LookAroundLastKnowPosition;
            yield return null;;
        } else
        {
            navAgent.SetDestination(bathroomPoint.transform.position);
        }

        yield return null;
    }

    private IEnumerator LookAroundLastKnowPosition()
    {
        if (!outwitted && personalidade != 4)
        {
            CheckPlayer();

            if(Vector3.Distance(transform.position, player.transform.position) <= 1.5f)
            {
                var targetDir = Quaternion.LookRotation(player.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetDir, 0.2f);
            }
        }

        if (lookAroundTime >= persistencia)
        {
            ResetToIdle();
			ExecuteAIState = GiveUpSearch;
            yield return null;
        } else
        {
            lookAroundTime += Time.deltaTime;
            var percent = (lookAroundTime / persistencia);
            if (personalidade != 4)
            {
                if (reactionIcon.renderer.material.mainTexture != occlusionTex)
                    reactionIcon.renderer.material.mainTexture = occlusionTex;

//                if (FOV.renderer.material.mainTexture != FOVocclusionTex)
//                    FOV.renderer.material.mainTexture = FOVocclusionTex;
				if(fov.LightColor != memoryColor)
				{
					fov.LightColor = memoryColor;
				}
            }
            reactionIcon.renderer.material.SetFloat("_Cutoff", Mathf.Clamp(percent, 0.001f, 1));


            transform.rotation = Quaternion.Slerp(transform.rotation, LastKnownPosition.transform.rotation, 0.05f);
        }

        yield return null;
    }

	private IEnumerator GiveUpSearch()
	{
		if(!outwitted)
		{
			yield return new WaitForSeconds(2);
		}
		
		outwitted = false;


		ExecuteAIState = Idle;
		yield return null;
    }
    
    private void ResetToIdle()
    {
        ResetLKP();
        reactionIcon.renderer.enabled = false;

        var percent = (lookAroundTime / persistencia);
        if (reactionIcon.renderer.material.mainTexture != occlusionTex)
            reactionIcon.renderer.material.mainTexture = occlusionTex;
        reactionIcon.renderer.material.SetFloat("_Cutoff", Mathf.Clamp(percent, 0.001f, 1));

//        if (FOV.renderer.material.mainTexture != FOVocclusionTex)
//            FOV.renderer.material.mainTexture = FOVocclusionTex;
		if(fov.LightColor != memoryColor)
		{
			fov.LightColor = memoryColor;
		}

		isWaiting = false;
		wanderWaitTime = 0;
		exposedPlayer = false;
    }
    #endregion

    #endregion

    #region Funções de Diálogo
    IEnumerator CastQuestion()
    {
        yield return new WaitForFixedUpdate();
        if (!questionWindow.activeSelf)
        {
            questionWindow.SetActive(true);
        }
        questionWindow.GetComponent<QuestionManager>().Initialize(humor, this);
    }

    public void ResetAnswer()
    {
        if (!dialogueSuccess && !player.GetComponent<ShameMeter>().gameOver)
        {
            StartCoroutine(CastQuestion());
        }
    }
    #endregion

    public void SearchPosition()
    {
        if (reactionIcon.renderer.material.mainTexture != detectionTex)
            reactionIcon.renderer.material.mainTexture = detectionTex;
        if (reactionIcon.renderer.enabled != true)
            reactionIcon.renderer.enabled = true;

//        if (FOV.renderer.material.mainTexture != FOVdetectionTex)
//            FOV.renderer.material.mainTexture = FOVdetectionTex;
		if(!memoria)
		{
			if(fov.LightColor != detectionColor)
			{
				fov.LightColor = detectionColor;
			}
		}
		else
		{
			if(fov.LightColor != memoryColor)
			{
				fov.LightColor = memoryColor;
			}
		}
		navAgent.SetDestination(LastKnownPosition.transform.position);
        ExecuteAIState = GoToLastKnowPosition;
        return;
    }

    void HistericalTween()
    {
        if(!histTweening)
        {
            Vector3 camOriginalPos = Camera.main.transform.position;

//            GameObject lookPos = new GameObject();
//            lookPos.name = "lookPos";
//            lookPos.transform.position = new Vector3(transform.position.x, transform.position.y + 9, transform.position.z);
//            lookPos.transform.LookAt(transform.position);
//            lookPos.transform.position -= lookPos.transform.up * 4;

            Camera.main.GetComponent<MouseOrbit>().enabled = false;

			Camera.main.GetComponent<PauseManager>().cameraTweening = true;

            iTween.MoveTo(Camera.main.gameObject, diagCamAnchor.transform.position, 2);
			iTween.RotateTo(Camera.main.gameObject,  diagCamAnchor.transform.rotation.eulerAngles,2);


            histTweening = true;
        }
    }
}