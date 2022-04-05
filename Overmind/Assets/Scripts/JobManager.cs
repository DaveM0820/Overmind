using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
public class JobManager : MonoBehaviour
{
    int numberOfPlayers;
    // variables for MoveTransform
    GlobalGameInformation gameInformation;
    public static Transform[] transformArray;
    public int transformsToMove = 0;
    JobHandle moveTransformsJobHandle;
    MoveTransforms moveTransforms;
    float3[] positions = new float3[1000];
    float3[] directions = new float3[1000];
    float[] speeds = new float[1000];
    float[] timesteps = new float[1000];
    Transform[] transforms = new Transform[1000];
    //variables for LookForTarget
    JobHandle lookForTargetJobHandle;

    LookForTarget lookForTarget;
    public int numberOfUnitsToLookForTarget = 0;
    Unit[] unitsToLookForTarget = new Unit[10000];
    Unit[] target = new Unit[5000];

    bool[] military = new bool[1000];
    float3[,] targetsToCheck = new float3[1000, 1000];
    float3[] targets;
    int totalNumberOfTargets;
    int[] numberOfTargetsPerFaction;
    int unitsToLookForTargetCounter;
    int updateUnitPositionsCounter = 0;
    int[] stopCounting_notinjob;
    int[] startCountingAgain_notinjob;
    int[] unitRange;

    //float3[] targetsToCheck = new float3 [1000,1000];

    void Awake() {
        //jobHandleList = new NativeList<JobHandle>(Allocator.Persistent);
        gameInformation = gameObject.GetComponent<GlobalGameInformation>();
        numberOfPlayers = gameInformation.numberOfPlayers;
        numberOfTargetsPerFaction = new int[numberOfPlayers];
        stopCounting_notinjob = new int[numberOfPlayers];
        startCountingAgain_notinjob = new int[numberOfPlayers];
        unitsToLookForTarget = new Unit[10000];
        unitRange = new int[1000];

    }
    public void MoveTransform(Transform transform, float speed, float timestep) {
        positions[transformsToMove] = new float3(transform.position.x, transform.position.y, transform.position.z);
        directions[transformsToMove] = new float3(transform.forward.x, transform.forward.y, transform.forward.z);
        speeds[transformsToMove] = speed;
        timesteps[transformsToMove] = timestep;
        transforms[transformsToMove] = transform;
        transformsToMove++;
    }
    public void LookForTarget(Unit unit, int range) {
        unitsToLookForTarget[numberOfUnitsToLookForTarget] = unit;
        unitRange[numberOfUnitsToLookForTarget] = range;
        numberOfUnitsToLookForTarget++;

    }
    void Update() {
        //code to check for targets

        // basically it gets a list of potential targets for each faction, and puts it in a flattened 1d float3 array
        updateUnitPositionsCounter++;
        if (updateUnitPositionsCounter == 1) // only do it every 40 frames
        {
            targets = new float3[numberOfPlayers * 1000];
            military = new bool[1000];

            numberOfTargetsPerFaction = new int[numberOfPlayers];
            totalNumberOfTargets = 0;
            for (int currentfaction = 0; currentfaction < numberOfPlayers; currentfaction++)
            {
                foreach (Unit unitToCheck in gameInformation.unitList[currentfaction])
                {
                    targets[totalNumberOfTargets] = unitToCheck.gameObject.transform.position;
                    military[totalNumberOfTargets] = unitToCheck.military;
                    target[totalNumberOfTargets] = unitToCheck;
                    totalNumberOfTargets++;
                }
                numberOfTargetsPerFaction[currentfaction] = gameInformation.unitList[currentfaction].Count;
                stopCounting_notinjob[currentfaction] = 0;
                for (int i = 0; i < currentfaction; i++)
                {
                    stopCounting_notinjob[currentfaction] += numberOfTargetsPerFaction[i];
                }
                startCountingAgain_notinjob[currentfaction] = stopCounting_notinjob[currentfaction] + numberOfTargetsPerFaction[currentfaction];
            }
            updateUnitPositionsCounter = 0;
        }
        if (numberOfUnitsToLookForTarget > 0)
        {

            NativeArray<int> faction = new NativeArray<int>(numberOfUnitsToLookForTarget, Allocator.TempJob);
            NativeArray<bool> isMilitary = new NativeArray<bool>(totalNumberOfTargets, Allocator.TempJob);
            NativeArray<float3> unitposition = new NativeArray<float3>(numberOfUnitsToLookForTarget, Allocator.TempJob);
            NativeArray<float3> targetpositions = new NativeArray<float3>(totalNumberOfTargets, Allocator.TempJob);
            NativeArray<int> factionTargetCount = new NativeArray<int>(numberOfPlayers, Allocator.TempJob);
            NativeArray<int> closestMilitaryTarget = new NativeArray<int>(numberOfUnitsToLookForTarget, Allocator.TempJob);
            NativeArray<int> closestNonMilitaryTarget = new NativeArray<int>(numberOfUnitsToLookForTarget, Allocator.TempJob);
            NativeArray<int> range = new NativeArray<int>(numberOfUnitsToLookForTarget, Allocator.TempJob);
            NativeArray<float> currentDistance = new NativeArray<float>(numberOfUnitsToLookForTarget, Allocator.TempJob);
            NativeArray<float> minDistance = new NativeArray<float>(numberOfUnitsToLookForTarget, Allocator.TempJob);
            NativeArray<int> stopCounting = new NativeArray<int>(numberOfPlayers, Allocator.TempJob);
            NativeArray<int> startCountingAgain = new NativeArray<int>(numberOfPlayers, Allocator.TempJob);


            for (int i = 0; i < numberOfUnitsToLookForTarget; i++)
            {

                faction[i] = unitsToLookForTarget[i].faction;
                currentDistance[i] = 10000;
                minDistance[i] = 10000;
                range[i] = unitRange[i];
                unitposition[i] = unitsToLookForTarget[i].gameObject.transform.position;
                //  Debug.Log("faction " + faction[i] + " postition " + unitposition[i]);
            }
            // Debug.Log("totalNumberOfTargets" + totalNumberOfTargets);

            for (int i = 0; i < totalNumberOfTargets; i++)
            {
                targetpositions[i] = targets[i];
                isMilitary[i] = military[i];
            }

            for (int i = 0; i < numberOfPlayers; i++)
            {
                factionTargetCount[i] = numberOfTargetsPerFaction[i];
                stopCounting[i] = stopCounting_notinjob[i];
                startCountingAgain[i] = startCountingAgain_notinjob[i];

            }

            lookForTarget = new LookForTarget {
                faction = faction,
                unitposition = unitposition,
                targetpositions = targetpositions,
                factionTargetCount = factionTargetCount,
                closestMilitaryTarget = closestMilitaryTarget,
                closestNonMilitaryTarget = closestNonMilitaryTarget,
                isMilitary = isMilitary,
                currentDistance = currentDistance,
                minDistance = minDistance,
                stopCounting = stopCounting,
                startCountingAgain = startCountingAgain,
                range = range
            };
  

            lookForTargetJobHandle = lookForTarget.Schedule(numberOfUnitsToLookForTarget, 1);
            lookForTargetJobHandle.Complete();
            Debug.Log("numberOfUnitsToLookForTarget" + numberOfUnitsToLookForTarget);

            for (int i = 0; i < numberOfUnitsToLookForTarget-1; i++)
            {
                if (closestMilitaryTarget[i] != 1000)
                {


                    //  unitsToLookForTarget[i].gameObject.GetComponent<UnitBehaviour>().Attack(currentTarget);
                    if (unitsToLookForTarget[i].faction != target[lookForTarget.closestMilitaryTarget[i]].faction)
                    {
                      unitsToLookForTarget[i].gameObject.GetComponent<UnitBehaviour>().Attack(target[lookForTarget.closestMilitaryTarget[i]].gameObject);
                    }
                    // Debug.Log("FINAL ASSIGNMENT unit " + unitsToLookForTarget[i].gameObject.name + " is targeting " + target[lookForTarget.closestMilitaryTarget[i]].gameObject);
                    // Debug.Log("FINAL ASSIGNMENT unit " + unitsToLookForTarget[i].gameObject.name + " is targeting " + target[lookForTarget.closestMilitaryTarget[i]].gameObject);


                }
                else
                {
                 //  Debug.Log("FINAL ASSIGNMENT unit " + unitsToLookForTarget[i].gameObject.name + " found no targets in range, mind distance " + lookForTarget.minDistance[i]);

                }
               //   Debug.Log(unitsToLookForTarget[i].gameObject.name + " found target with unit number " + lookForTarget.closestTarget[i]);
            }

            faction.Dispose();
            unitposition.Dispose();
            targetpositions.Dispose();
            factionTargetCount.Dispose();
            closestMilitaryTarget.Dispose();
            closestNonMilitaryTarget.Dispose();
            isMilitary.Dispose();
            currentDistance.Dispose();
            minDistance.Dispose();
            stopCounting.Dispose();
            startCountingAgain.Dispose();
            range.Dispose();
        }
        numberOfUnitsToLookForTarget = 0;
        //   unitsToLookForTarget = new Unit[1000];
        ///////////////////////////////////////////////move transforms///////////////////////////////////////////////
        NativeArray<float> speedArray = new NativeArray<float>(1000, Allocator.TempJob);
        NativeArray<float3> directionArray = new NativeArray<float3>(1000, Allocator.TempJob);
        NativeArray<float> timeStepArray = new NativeArray<float>(1000, Allocator.TempJob);
        NativeArray<float3> positionArray = new NativeArray<float3>(1000, Allocator.TempJob);
        for (int i = 0; i < transformsToMove; i++)
        {

            positionArray[i] = positions[i];
            directionArray[i] = directions[i];
            speedArray[i] = speeds[i];
            timeStepArray[i] = timesteps[i];

        }
        moveTransforms = new MoveTransforms {

            position = positionArray,
            direction = directionArray,
            speed = speedArray,
            timeStep = timeStepArray,

        };

        moveTransformsJobHandle = moveTransforms.Schedule(transformsToMove, 8);
        moveTransformsJobHandle.Complete();

        for (int i = 0; i < transformsToMove; i++)
        {
            transforms[i].position = moveTransforms.position[i];

        }
        transformsToMove = 0;

        positionArray.Dispose();
        directionArray.Dispose();
        timeStepArray.Dispose();
        speedArray.Dispose();


    }
    private void LateUpdate() {

    }

    // Update is called once per frame
    void onDestroy() {

    }
}
[BurstCompile]
public struct MoveTransforms : IJobParallelFor
{
    public NativeArray<float3> position;
    public NativeArray<float> speed;
    public NativeArray<float3> direction;
    public NativeArray<float> timeStep;

    public void Execute(int index) {
        position[index] = position[index] + (speed[index] * direction[index] * timeStep[index]);
    }
}
[BurstCompile]

public struct LookForTarget : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<int> faction;
    [ReadOnly]
    public NativeArray<float3> unitposition;
    [ReadOnly]
    public NativeArray<bool> isMilitary;
    [ReadOnly]
    public NativeArray<float3> targetpositions;
    [ReadOnly]
    public NativeArray<int> factionTargetCount;
    [ReadOnly]
    public NativeArray<int> stopCounting;
    [ReadOnly]
    public NativeArray<int> range;
    [ReadOnly]
    public NativeArray<int> startCountingAgain;

    public NativeArray<int> closestNonMilitaryTarget;
    public NativeArray<int> closestMilitaryTarget;
    public NativeArray<float> currentDistance;
    public NativeArray<float> minDistance;
    public void Execute(int index) {
        for (int i = 0; i < targetpositions.Length; i++)
        {
            if (i < stopCounting[faction[index]] || i > startCountingAgain[faction[index]])
            {
         

            currentDistance[index] = math.distance(unitposition[index], targetpositions[i]);
            //  Debug.Log("unit " + index + " compared to " + i + " currentDistance " + currentDistance[index] + "stopcounting" + stopCounting[faction[index]] + "start counting"+ startCountingAgain[faction[index]] );
            {
                if (currentDistance[index] < minDistance[index])
                {
                    //   Debug.Log("unit " + index + " compared to " + i + " currentDistance " + currentDistance[index] + " minDistance " + minDistance[index]);

                    if (isMilitary[i])
                    {
                        //   Debug.Log("unit " + index + " compared to " + i + " currentDistance " + currentDistance[index] + " minDistance " + minDistance[index] + " is military");

                        minDistance[index] = currentDistance[index];
                        closestMilitaryTarget[index] = i;
                        // Debug.Log("in unit " + index + " min distance " + minDistance[index] + " closestMilitaryTarget[index] " + closestMilitaryTarget[index]+ " i is" + i) ;

                        //   if (currentDistance[index] < range[index])
                        //  {
                        //   Debug.Log("unit " + index + " compared to " + i + " currentDistance " + currentDistance[index] + " minDistance " + minDistance[index] + " is military and within range " + range[index]);
                        // break;
                        //   }     
                    }
                    else
                    {
                        closestNonMilitaryTarget[index] = i;
                    }
                }
                }
            }
        }

        if (minDistance[index] > range[index])
        {
            if (math.distance(unitposition[index], targetpositions[closestNonMilitaryTarget[index]]) < range[index])
            {
                closestMilitaryTarget[index] = closestNonMilitaryTarget[index];
            }
            else
            {
                closestMilitaryTarget[index] = 1000;
            }

            //   Debug.Log("unit " + index + " min distance " + minDistance[index] + " closestMilitaryTarget[index] " + closestMilitaryTarget[index]);

        }

    }
}
