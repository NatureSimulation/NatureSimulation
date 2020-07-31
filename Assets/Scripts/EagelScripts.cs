using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EagelScripts : MonoBehaviour
{
    private Animator animator;
    public float walkspeed = 5;
    float speedOut = 1;
    private float wonderTime;
	private float rotationDegreePerSecond = 1000;
	private bool isAttacking = false;
    private bool dead;

    public Vector3 target;
    public float minAttackDistance;
    private EagleState currentState;

    enum EagleState {
        Wandering,
        Targeting,
        Dead,
        Attacking
    };

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        wonderTime = Random.Range(1.0f, 2.0f);
        currentState = EagleState.Wandering;
    }

    void Update()
    {
        
    }

    void FixedUpdate()
	{
		if (currentState == EagleState.Wandering)
		{
            /* Rotate */
            if (wonderTime > 0) {
                wonderTime -= Time.deltaTime;
            } else {
                wonderTime = Random.Range(1.0f, 2.0f);
                transform.Rotate(0, Random.Range(-120, 120), 0, Space.World);
            }

            /* Forward */
            float planeY = 0f;
            try {
                planeY = GameManager.instance.getHeight(this.transform.position.x, this.transform.position.z);
            } catch {
                planeY = 0;
            }

            float velocityY = 0f;
            if (planeY + 10 > this.transform.position.y) {
                velocityY = 1f;
            } else {
                velocityY = -1f;
            }
            
			GetComponent<Rigidbody>().velocity = transform.forward * speedOut * walkspeed + new Vector3(0, velocityY, 0);
			animator.SetFloat("Speed", speedOut);

            /* Look near */
            Collider rabbitCollider = Physics.OverlapSphere(transform.position, 20.0f).SingleOrDefault(collider => collider.tag == "Grass");
            if (rabbitCollider != null) {
                currentState = EagleState.Targeting;
                transform.rotation = Quaternion.LookRotation(rabbitCollider.transform.position - transform.position, Vector3.up);
                target = rabbitCollider.transform.position;
            }

        } else if (currentState == EagleState.Targeting ) {
            Debug.DrawLine(transform.position, target, Color.white);
            transform.rotation = Quaternion.LookRotation(target - transform.position, Vector3.up);
            
            /* Forward */
            float velocityY = 0f;
            if (target.y > this.transform.position.y) {
                velocityY = 1f;
            } else {
                velocityY = -1f;
            }

            GetComponent<Rigidbody>().velocity = transform.forward * speedOut * walkspeed + new Vector3(0, velocityY, 0);
			animator.SetFloat("Speed", speedOut);
		}
	}
}
