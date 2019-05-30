using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eActStatus { Prepare = 0, Perform, Finish, End }
public enum eSkillStepType { Idle, SpawnProjectile }

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
        public GameObject spawnPrefab;
    }

    public eActStatus status { get; protected set; }

    protected Champion champion;
    public Conditions conditions;
    public LayerMask targets;

    [HideInInspector]
    public List<Step> prepareSteps;
    [HideInInspector]
    public List<Step> performSteps;
    [HideInInspector]
    public List<Step> finishSteps;

    protected int stepIndex = 0;
    

    private class Field
    {
        public static float stepDuration = 0;
    }

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

    }

    protected virtual void Update()
    {
        if (status == eActStatus.Prepare) DoSteps(prepareSteps);
        else if (status == eActStatus.Perform) DoSteps(performSteps);
        else if (status == eActStatus.Finish) DoSteps(finishSteps);
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
                    if (stepDuration != 0) stepDuration -= Time.deltaTime;
                    else
                    {
                        stepFinished = true;
                    }
                }
                break;
            case eSkillStepType.SpawnProjectile:
                {
                    GameObject inst = Instantiate(step.spawnPrefab, transform.position, transform.rotation);
                    stepFinished = true;
                }
                break;
        }
    }

    protected virtual void DoSteps(List<Step> steps)
    {
        if (stepFinished) {
            stepIndex++;
        }

        if (stepIndex >= steps.Count)
        {
            if ((int)status < (int)eActStatus.Finish)
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
        champion.StartSkill();
        status = eActStatus.Prepare;
        PrepareNewSteps();
        ResetStepDatas();
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
