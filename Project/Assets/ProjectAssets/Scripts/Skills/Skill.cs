using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eActStatus { Perform, End }
public enum eSkillStepType { Idle, SpawnProjectile, TrackTarget }

public class Skill : MonoBehaviour
{

    [System.Serializable]
    public class Conditions
    {
        public float healthCost;
        public float staminaCost;
        public float manaCost;
        public bool noCost = false;

        public Conditions(float healthCost, float staminaCost, float manaCost)
        {
            this.healthCost = healthCost;
            this.staminaCost = staminaCost;
            this.manaCost = manaCost;
        }
    }

    [System.Serializable]
    public class Step
    {
        public eSkillStepType type = eSkillStepType.Idle;
        public float duration = 1;
        public float lifeTime = 1;
        public SpawnProjectile projectile;
        public TrackTarget trackTarget;

        [System.Serializable]
        public class SpawnProjectile
        {
            public string code;
        }
        [System.Serializable]
        public class TrackTarget
        {
            public float speed = 3;
            public float stopDistance = 5;
            public Vector3 offset;
            public bool canPenetrateEnviroment = false;
        }
    }

    [System.Serializable]
    public class ProjectilePrefab
    {
        public string name;
        public GameObject prefab;
        public Vector3 offset = Vector3.zero;
        public float lifeTime = 0.5f;
        public float speed = 5;
    }

    private class Field
    {
        public static float stepDuration = 0;
    }

    public eActStatus status { get; protected set; }

    protected Champion champion;
    public Conditions conditions;
    public LayerMask targets;
    public float range;
    protected Character target;

    [HideInInspector]
    public List<Step> performSteps;

    public List<ProjectilePrefab> projectilePrefabs = new List<ProjectilePrefab>();
    public Dictionary<string, ProjectilePrefab> ProjectilePrefabs = new Dictionary<string, ProjectilePrefab>();


    protected float stepDuration {
        get
        {
            return Field.stepDuration;
        }
        set
        {
            Field.stepDuration = value;
            if (Field.stepDuration < 0) Field.stepDuration = 0;
        }
    }

    protected int stepIndex = 0;


    protected bool stepStarted = false;
    protected bool stepFinished = false;

    protected virtual void Awake()
    {
        champion = GetComponent<Champion>();

        stepDuration = 0;
        status = eActStatus.End;
    }

    protected virtual void Start()
    {
        foreach (ProjectilePrefab prefab in projectilePrefabs)
        {
            ProjectilePrefabs.Add(prefab.name, prefab);
        }
    }

    protected virtual void Update()
    {
        if (status == eActStatus.Perform) DoSteps(performSteps);
    }

    protected virtual void DoStep(Step step)
    {
        switch (step.type)
        {
            case eSkillStepType.Idle:
                {
                    if (stepStarted)
                    {
                        stepDuration = step.duration;
                        stepStarted = false;
                    }
                    else if (stepDuration != 0) stepDuration -= Time.deltaTime;
                    else
                    {
                        stepFinished = true;
                    }
                }
                break;
            case eSkillStepType.SpawnProjectile:
                {
                    SpawnProjectile(step);
                }
                break;
            case eSkillStepType.TrackTarget:
                {
                    TrackTarget(step);
                }
                break;
        }
    }

    protected virtual void SpawnProjectile(Step step)
    {
        ProjectilePrefab projectile = ProjectilePrefabs[step.projectile.code];
        GameObject inst = Instantiate(projectile.prefab, transform.position + transform.TransformDirection(projectile.offset), transform.rotation);
        // 임시
        BulletBehaviour bullet = inst.GetComponent<BulletBehaviour>();
        bullet.Fire(projectile.speed, projectile.lifeTime);
        stepStarted = false;
        stepFinished = true;
    }

    protected virtual void TrackTarget(Step step)
    {
        if (stepStarted)
        {
            stepStarted = false;
            champion.motor.DisablePhysicColliders();
            champion.motor.EnableRigidbody();
            champion.motor.rg.useGravity = false;

            champion.motor.UpdateTargetPosition(target.transform.position);
            champion.motor.LookTargetPositionInstant();
        }
        else if (target == null || Vector3.Distance(transform.position + transform.TransformDirection(step.trackTarget.offset), target.transform.position) < step.trackTarget.stopDistance + 0.5f)
        {
            champion.motor.DisableRigidbody();
            champion.motor.EnablePhysicColliders();
            stepFinished = true;
        }
        else
        {
            champion.motor.UpdateTargetPosition(transform.position + transform.forward);
            transform.position += transform.forward * Time.deltaTime * step.trackTarget.speed;
        }
    }

    protected virtual void DoSteps(List<Step> steps)
    {
        if (stepFinished && !stepStarted) {
            stepIndex++;
            ResetStepDatas();
        }

        if (stepIndex >= steps.Count)
        {
            if ((int)status < (int)eActStatus.Perform)
            {
                PrepareNewSteps();
                status++;
            }
            else EndSkill();
            return;
        }

        DoStep(steps[stepIndex]);
    }

    protected virtual void PrepareNewSteps()
    {
        stepIndex = 0;
    }

    protected virtual void ResetStepDatas()
    {
        stepDuration = 0;
        stepStarted = true;
        stepFinished = false;
    }

    public virtual void StartSkill()
    {
        if (!GetTarget()) return;

        champion.StartSkill();
        status = eActStatus.Perform;
        PrepareNewSteps();
        ResetStepDatas();
    }

    public virtual bool GetTarget()
    {
        if (range == 0) return true;

        target = null;
        Vector3 targetPosition = CameraFollow.inst.GetTargetPosition();
        bool isTargetExist = false;
        for (float i = 0; i <= 1f; i += 0.5f)
        {
            Collider[] colliders = Physics.OverlapSphere(targetPosition, i, targets);
            if (colliders.Length > 0)
                foreach (Collider collider in colliders)
                {
                    if (Vector3.Distance(transform.position, collider.transform.position) <= range)
                    {
                        target = collider.GetComponent<Character>();
                        isTargetExist = true;
                        break;
                    }
                }

            if (target != null) break;
        }
        return isTargetExist;
    }

    public virtual void CancelSkill()
    {
        status = eActStatus.End;
    }
    
    public virtual void EndSkill()
    {
        status = eActStatus.End;
        champion.EndSkill(); // 중요
    }
}
