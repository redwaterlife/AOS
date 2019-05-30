using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SkillTypes { Q = 0, W, E, R }

public class PlayerController : MonoBehaviour
{
    public class Skill
    {
        public float baseCool { get; private set; }
        public float currentCool { get; private set; }
        public Skill(float baseCool)
        {
            this.baseCool = baseCool;
            currentCool = 0;
        }

        public void UpdateCooldowns()
        {
            if (currentCool > 0) currentCool -= Time.deltaTime;
            Mathf.Clamp(currentCool, 0, baseCool);
        }

        public void UseSKill()
        {
            currentCool = baseCool;
        }
    }

    Rigidbody rg;

    public GameObject[] skillObjects = new GameObject[4];
    public GameObject Q2;
    public GameObject Q3;

    public Skill[] skills = new Skill[4];

    private CameraFollow cam;
    private NavMeshAgent agent;
    private Vector3 targetPosition;
    private float minDistance = 0.6f;

    private int targetSkill = -1;

    public int qstack = 0;

    public LayerMask enemys;
    public Vector3 rtarget;

    private void Start()
    {
        cam = CameraFollow.inst;
        agent = GetComponent<NavMeshAgent>();
        rg = GetComponent<Rigidbody>();

        skills[(int)SkillTypes.Q] = new Skill(1f);
        skills[(int)SkillTypes.W] = new Skill(2f);
        skills[(int)SkillTypes.E] = new Skill(0.5f);
        skills[(int)SkillTypes.R] = new Skill(3f);
    }

    private void Update()
    {
        targetPosition = cam.GetTargetPosition();
        Rotate();

        if (Input.GetMouseButtonDown(1)) Move();
        if (Input.GetKeyDown(KeyCode.S)) agent.enabled = false;

        if (Input.GetKeyDown(KeyCode.Q)) Q();
        if (Input.GetKeyDown(KeyCode.E)) E();
        if (Input.GetKeyDown(KeyCode.R)) R();

        foreach(Skill skill in skills)
        {
            skill.UpdateCooldowns();
        }

        if (targetSkill == (int)SkillTypes.E && skills[(int)SkillTypes.E].currentCool < 0.2f) { 
            rg.velocity = new Vector3(0, rg.velocity.y, 0);
            targetSkill = -1;
            agent.enabled = true;
            agent.SetDestination(transform.position + transform.forward * 1);
        }
        if (targetSkill == (int)SkillTypes.R&& rtarget != Vector3.zero) transform.position = Vector3.Lerp(transform.position, rtarget, Time.deltaTime * 8f);
    }

    void Move()
    {
        if (targetSkill != -1) return;

        if (Vector3.Distance(transform.position, targetPosition) >= minDistance)
        {
            agent.enabled = true;
            agent.SetDestination(targetPosition);
        }
    }

    void Rotate()
    {
        Quaternion lookRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
        Quaternion newRotation = lookRotation;
        transform.eulerAngles = Vector3.up * newRotation.eulerAngles.y;
    }

    private void Q()
    {
        if (skills[(int)SkillTypes.Q].currentCool > 0) return;

        if (targetSkill == (int)SkillTypes.E)
        {
            BulletBehaviour instance = Instantiate(Q3, transform.position, transform.rotation).GetComponent<BulletBehaviour>();
            instance.Track(transform, 0.2f);
            if (qstack == 2) instance.AddAirbone();
            skills[(int)SkillTypes.Q].UseSKill();

            addQ();
        }
        else
        {
            if (qstack != 2)
            {
                BulletBehaviour instance = Instantiate(skillObjects[(int)SkillTypes.Q], transform.position, transform.rotation).GetComponent<BulletBehaviour>();
                instance.Fire(10, 0.1f);
                skills[(int)SkillTypes.Q].UseSKill();
            }
            else
            {
                BulletBehaviour instance = Instantiate(Q2, transform.position, transform.rotation).GetComponent<BulletBehaviour>();
                instance.Fire(6f, 0.75f);
                instance.AddAirbone();
                skills[(int)SkillTypes.Q].UseSKill();
            }
            addQ();
        }

        
    }

    public void addQ()
    {
        qstack++;
        if (qstack > 2) qstack = 0;
    }

    private void E()
    {
        if (skills[(int)SkillTypes.E].currentCool > 0) return;

        rg.AddForce(transform.forward * 10, ForceMode.Impulse);

        targetSkill = (int)SkillTypes.E;
        skills[(int)SkillTypes.E].UseSKill();
        agent.enabled = false;
    }

    private void R()
    {
        if (skills[(int)SkillTypes.R].currentCool > 0) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 8, enemys);


        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].transform.position.y > 1.5f)
            {
                targetSkill = (int)SkillTypes.R;
                skills[(int)SkillTypes.R].UseSKill();
                agent.enabled = false;
                Vector3 sub = (transform.position - colliders[i].transform.position) * .5f;
                sub.y = 0;
                rtarget = colliders[i].transform.position + sub;
                StartCoroutine(a(colliders[i].GetComponent<Enemy>()));
                break;
            }
        }
    }

    IEnumerator a(Enemy enemy)
    {
        enemy.Ice();
        agent.enabled = false;
        rg.isKinematic = true;
        yield return new WaitForSeconds(0.8f);
        rg.isKinematic = false;
        enemy.StopIce();
        rtarget = Vector3.zero;
        yield return new WaitForSeconds(.5f);
        targetSkill = -1;
    }
}
