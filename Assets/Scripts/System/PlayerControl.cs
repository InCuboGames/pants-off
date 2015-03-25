using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{

    public bool crouching;
    CharacterMotor charMotor;
    PlatformInputController inputController;

    public bool canControl
    {
        get;
        set;
    }

    public bool exposed 
    {
        get;
        set;
    }

	public bool beingSeen 
	{
		get;
		set;
	}

    bool scared = false;

    public Camera cam;

    public IA exposedBy
    {
        get;
        set;
    }

    [SerializeField]
    AudioSource
        audioSource;

    [SerializeField]
    AudioClip
        scareSFX;

    [SerializeField]
    public Animator
        anim;

    float originalSpeed;

	StageManager stageManager;

	public GameObject cellphone, 
	earmuffs, breath,
	snokel, snorkelBubbles;

    // Use this for initialization
    void OnEnable()
    {
        charMotor = GetComponent<CharacterMotor>();
        inputController = GetComponent<PlatformInputController>();

		stageManager = GameObject.Find("Stage Manager").GetComponent<StageManager>();

        originalSpeed = charMotor.movement.maxForwardSpeed;

        canControl = true;
        exposed = false;
			
		if(stageManager.VA == 1)
		{
			cellphone.SetActive(true);
		}
		else if(stageManager.VA == 2)
		{
			charMotor.movement.maxGroundAcceleration = 3.5f;
			earmuffs.SetActive(true);
			breath.SetActive(true);
		}
		else if(stageManager.VA == 3)
		{
			originalSpeed = originalSpeed/2;
			charMotor.movement.maxForwardSpeed = originalSpeed;
			anim.speed = 0.5f;
			snokel.SetActive(true);
			snorkelBubbles.SetActive(true);
		}
    }
	
    // Update is called once per frame
    void Update()
    {

        #region Animator Variables

        if(canControl)
        {
            anim.SetBool("crouching", crouching);

            bool horizontalInput = (Input.GetAxis("Horizontal") > 0.1f || Input.GetAxis("Horizontal") < -0.1f) ;
            bool verticalInput = (Input.GetAxis("Vertical") > 0.1f || Input.GetAxis("Vertical") < -0.1f) ;

            anim.SetBool("input", horizontalInput||verticalInput);
            anim.SetBool("exposed", exposed && !scared);
        }
        #endregion

       // print(exposed + "|" + scared);

        if (charMotor.canControl != canControl)
        {
            inputController.autoRotate = canControl;
            charMotor.canControl = canControl;
        }

        if (!exposed)
        {
            if (scared)
            {
                //StandUp();
                charMotor.movement.maxForwardSpeed = charMotor.movement.maxSidewaysSpeed = charMotor.movement.maxBackwardsSpeed = originalSpeed;
                StopCoroutine(Scare());
                scared = false;
            }

            if(canControl)
            {
                if (Input.GetButtonDown("Left Shift") || Input.GetButtonDown("B Button"))
                {
                    if (cam.GetComponent<MouseOrbit>().distance == 9 || cam.GetComponent<MouseOrbit>().distance == 3)
                    {
                        if (!crouching)
                        {
                            Crouch();
                        } else
                        {
                            StandUp();
                        }
                    }
                }
            }
        } else
        {
            if (!scared)
            {
                StartCoroutine(Scare());
            }
        }

		beingSeen = false;
    }

    void Crouch()
    {
        //transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
        //transform.localScale = new Vector3(1, 0.7f, 1);
        charMotor.movement.maxForwardSpeed = charMotor.movement.maxSidewaysSpeed = charMotor.movement.maxBackwardsSpeed = originalSpeed / 3;

        TweenDistCloser();
        TweenLimitCloser();
        //cam.cullingMask &= ~(1 << LayerMask.NameToLayer("FOVs"));

        crouching = true;
    }

    public void StandUp()
    {
        //transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        //transform.localScale = new Vector3(1, 1, 1);
        charMotor.movement.maxForwardSpeed = charMotor.movement.maxSidewaysSpeed = charMotor.movement.maxBackwardsSpeed = originalSpeed;

        TweenDistFarther();
        TweenLimitFarther();
        cam.cullingMask |= 1 << LayerMask.NameToLayer("FOVs");

        crouching = false;
    }

    public IEnumerator Scare()
    {
        if (crouching)
            StandUp();

        if(canControl)
        {
            audioSource.PlayOneShot(scareSFX);
            canControl = false;
        }

        Quaternion originalRot = transform.rotation;

        if(exposedBy)
        {
            Transform iaPos = exposedBy.transform;
            Quaternion targetDir = Quaternion.LookRotation(iaPos.position - transform.position);
            targetDir = new Quaternion(targetDir.x, targetDir.y, 0, targetDir.w);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetDir, 0.2f);

            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        
        yield return new WaitForSeconds(0.7f);



        if(!GetComponent<ShameMeter>().onDialogue && !GetComponent<ShameMeter>().gameOver)
        {
            if(exposedBy)
            {
                if(exposedBy.personalidade != 4)
                {
                  canControl = true;
                }
            }
            else
            {
                canControl = true;
            }
        }

        //transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
        //transform.localScale = new Vector3(1, 0.7f, 1);
        charMotor.movement.maxForwardSpeed = charMotor.movement.maxSidewaysSpeed = charMotor.movement.maxBackwardsSpeed = originalSpeed * 2f;

        scared = true;

        StopCoroutine(Scare());
        yield return null;
    }


    void TweenDistCloser()
    {
        iTween.ValueTo(gameObject, iTween.Hash(
        "from", 9,
        "to", 3,
        "time", 0.3f,
        "onupdatetarget", gameObject,
        "onupdate", "SetTweenDistCloser",
        "easetype", iTween.EaseType.easeInCirc));
    }

    void SetTweenDistCloser(float tweenedValue)
    {
        cam.GetComponent<MouseOrbit>().distance = tweenedValue;
    }

    void TweenLimitCloser()
    {
        iTween.ValueTo(gameObject, iTween.Hash(
        "from", 65,
        "to", 0,
        "time", 0.3f,
        "onupdatetarget", gameObject,
        "onupdate", "SetTweenLimitCloser",
        "easetype", iTween.EaseType.easeInCirc));
    }

    void SetTweenLimitCloser(float tweenedValue)
    {
        cam.GetComponent<MouseOrbit>().yMaxLimit = cam.GetComponent<MouseOrbit>().yMinLimit = (int)tweenedValue;
    }

    void TweenDistFarther()
    {
        iTween.ValueTo(gameObject, iTween.Hash(
        "from", 3,
        "to", 9,
        "time", 0.5f,
        "onupdatetarget", gameObject,
        "onupdate", "SetTweenDistFarther",
        "easetype", iTween.EaseType.easeOutCirc));
    }

    void SetTweenDistFarther(float tweenedValue)
    {
        cam.GetComponent<MouseOrbit>().distance = tweenedValue;
    }

    void TweenLimitFarther()
    {
        iTween.ValueTo(gameObject, iTween.Hash(
        "from", 0,
        "to", 65,
        "time", 0.5f,
        "onupdatetarget", gameObject,
        "onupdate", "SetTweenLimitFarther",
        "easetype", iTween.EaseType.easeOutExpo));
    }

    void SetTweenLimitFarther(float tweenedValue)
    {
        cam.GetComponent<MouseOrbit>().yMaxLimit = cam.GetComponent<MouseOrbit>().yMinLimit = (int)tweenedValue;
    }
}
