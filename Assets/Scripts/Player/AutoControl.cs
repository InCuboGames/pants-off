using UnityEngine;
using System.Collections;

public class AutoControl : MonoBehaviour {

    private NavMeshAgent navAgent;
    Transform target;
    [SerializeField] Animator anim;

	// Use this for initialization
	void OnEnable () {
		target = GameObject.Find("Player Start").transform;

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.SetDestination(target.position);
	}

    void Update()
    {
        #region Animator Variables

        anim.SetBool("input", navAgent.velocity.magnitude > 0.1f);

        #endregion
    }
}
