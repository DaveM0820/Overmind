using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using System.Diagnostics;
public class JobManager : MonoBehaviour
{
    int numberOfPlayers;
    public List<JobHandle> jobList = new List<JobHandle>();

    //ctr
    public Stopwatch performanceTimer = new Stopwatch();
    GlobalGameInformation gameInformation;

    // variables for MoveTransform///////////////////////
    JobHandle moveTransformsJobHandle;
    MoveTransforms moveTransforms;

    public int totaltransforms = 0;
    float3[] position_MoveTransforms = new float3[400];
    float3[] destination_MoveTransforms = new float3[400];

    float3[] forward_MoveTransforms = new float3[400];
    float[] speed_MoveTransforms = new float[400];
    float[] timestep_MoveTransforms = new float[400];
    Transform[] transform_MoveTransforms = new Transform[400];
    int transformsToMove;
    public bool[] shouldMove_MoveTransforms = new bool[400];
    int checkDistanceCounter_MoveTransfors;

    NativeArray<bool> shouldMove_moveTransform_Native;
    NativeArray<bool> atDestination_moveTransform_Native;
    NativeArray<float> speed_moveTransform_Native;
    NativeArray<float3> direction_moveTransform_Native;
    NativeArray<float> timeStep_moveTransform_Native;
    NativeArray<float3> position_moveTransform_Native;
    NativeArray<float3> destination_moveTransform_Native;
    NativeArray<bool> shouldCheckDistance_moveTransform_Native;

    //variables for LookForTarget////////////////////////////
    JobHandle lookForTargetJobHandle;
    LookForTarget lookForTarget;

    public int numberOfUnitsToLookForTarget = 0;
    Unit[] unitsToLookForTarget = new Unit[400];
    Unit[] target = new Unit[400];
    int updateUnitPositionsCounter = 0;
    int totalNumberOfTargets = 0;

    bool[] military = new bool[400];
    float3[,] targetsToCheck = new float3[400, 400];
    Vector3[] targets = new Vector3[400];
    int[] numberOfTargetsPerFaction = new int[400];
    int[] unitsToLookForTargetCounter = new int[400];
    int[] stopCounting_notinjob = new int[400];
    int[] startCountingAgain_notinjob = new int[400];
    int[] unitRange = new int[400];

    NativeArray<int> faction_LookforTarget_Native;
    NativeArray<bool> isMilitary_LookforTarget_Native;
    NativeArray<float3> unitPosition_LookforTarget_Native;
    NativeArray<float3> targetPositions_LookforTarget_Native;
    NativeArray<int> factionTargetCount_LookforTarget_Native;
    NativeArray<int> closestMilitaryTarget_LookforTarget_Native;
    NativeArray<int> closestNonMilitaryTarget_LookforTarget_Native;
    NativeArray<int> range_LookforTarget_Native;
    NativeArray<float> currentDistance_LookforTarget_Native;
    NativeArray<float> minDistance_LookforTarget_Native;
    NativeArray<int> stopCounting_LookforTarget_Native;
    NativeArray<int> startCountingAgain_LookforTarget_Native;
    NativeArray<bool> shouldCheckDistance_LookforTarget_Native;


    //variables for moveTurret/////////////////////////////////////////
    JobHandle moveTurretJobHandle;
    MoveTurrets moveTurrets;
    int turretsToMove = 0;
    int totalTurrets = 0;
    GameObject[] unit_moveTurret = new GameObject[400];
    Transform[] turretsToMove_moveTurret = new Transform[400];
    Transform[] gunToMove_moveTurret = new Transform[400];
    Transform[] target_moveTurret = new Transform[400];
    float3[] turretposition_moveTurret = new float3[400];
    float3[] targetposition_moveTurret = new float3[400];
    float3[] gunForward_moveTurret = new float3[400];
    float3[] turretForward_moveTurret = new float3[400];
    float[] deltaTime_moveTurret = new float[400];
    float[] rotationspeed_moveTurret = new float[400];
    float[] gunrechargetotal_moveTurret = new float[400];
    float[] gunrechargecurrent_moveTurret = new float[400];
    bool[] shouldFire_moveTurret = new bool[400];
    bool[] shouldMove_moveTurret = new bool[400];

    NativeArray<float3> turretPosition_moveTurret_Native;
    NativeArray<float3> targetPosition_moveTurret_Native;
    NativeArray<float3> gunForward_moveTurret_Native;
    NativeArray<float3> turretForward_moveTurret_Native;
    NativeArray<float> deltaTime_moveTurret_Native;
    NativeArray<float> rotationSpeed_moveTurret_Native;
    NativeArray<float> gunrechargeTotal_moveTurret_Native;
    NativeArray<float> gunrechargeCurrent_moveTurret_Native;
    NativeArray<bool> canFire_moveTurret_Native;
    NativeArray<bool> shouldCheckDistance_moveTurret_Native;


    //variables for moveVehilces/////////////////////////////////////////
    JobHandle moveVehiclesJobHandle;
    int vehiclesToMove = 0;

    float3[] unitposition_moveVehicle = new float3[400];
    float3[] targetposition_moveVehicle = new float3[400];
    float3[] unitforward_moveVehicle = new float3[400];
    float3[] unitVelocity_moveVehicle = new float3[400];
    float[] unitMaxSpeed_moveVehicle = new float[400];
    float[] unitTurnSpeed_moveVehicle = new float[400];
    float[] deltaTime_moveVehicle = new float[400];
    float[] acceleration_moveVehicle = new float[400];
    float[] dampening_moveVehicle = new float[400];
    bool[] atTarget_moveVehicle = new bool[400];



    int shouldCheckDistanceCounter = 0;
    bool shouldCheckDistance;
    float elapsed;
    float updateTimestep;
    float halfTimestep;

    void Awake() {
        gameInformation = gameObject.GetComponent<GlobalGameInformation>();
        numberOfPlayers = gameInformation.numberOfPlayers;
        updateTimestep = gameInformation.updateTimestep;
        halfTimestep = updateTimestep / 2;
    }
    public void LookForTarget(Unit unit, int range) {
        unitsToLookForTarget[numberOfUnitsToLookForTarget] = unit;
        unitRange[numberOfUnitsToLookForTarget] = range;
        numberOfUnitsToLookForTarget++;
    }
    public void AddTransformToMove(Transform transform, float speed) {
        position_MoveTransforms[totaltransforms] = transform.position;
        forward_MoveTransforms[totaltransforms] = transform.forward;
        destination_MoveTransforms[totaltransforms] = transform.position;
        speed_MoveTransforms[totaltransforms] = speed;
        transform_MoveTransforms[totaltransforms] = transform;
        shouldMove_MoveTransforms[totaltransforms] = false;


        transform.gameObject.GetComponent<UnitBehaviour>().transformNumber = totaltransforms;
        totaltransforms++;
    }
    public void MoveTransform(int index, Vector3 destination, Vector3 forward) {

        if (!shouldMove_MoveTransforms[index])
        {
            transformsToMove++;
            shouldMove_MoveTransforms[index] = true;
        }
        if (destination != (Vector3)destination_MoveTransforms[index])
        {

            forward_MoveTransforms[index] = forward;
            destination_MoveTransforms[index] = destination;
        }
    }
    public void StopMovingTransform(int index) {
        shouldMove_MoveTransforms[index] = false;
        transformsToMove--;
    }

    public void AddTurretToMove(GameObject unit, Transform turret, Transform gun, float rotateSpeed) {
        unit_moveTurret[totalTurrets] = unit;
        turretsToMove_moveTurret[totalTurrets] = turret;
        gunToMove_moveTurret[totalTurrets] = gun;
        gunForward_moveTurret[totalTurrets] = gun.forward;
        turretForward_moveTurret[totalTurrets] = gun.forward;
        rotationspeed_moveTurret[totalTurrets] = rotateSpeed;
        shouldFire_moveTurret[totalTurrets] = false;
        shouldMove_moveTurret[totalTurrets] = false;
        unit.GetComponent<UnitBehaviour>().turretNumber = totalTurrets;

        totalTurrets++;
    }
    public void MoveTurret(int index, Transform target) {
        if (!shouldMove_moveTurret[index])
        {
            turretsToMove++;

            shouldMove_moveTurret[index] = true;

        }
        
        if (target != target_moveTurret[index])
        {
            target_moveTurret[index] = target;
        }
      //  UnityEngine.Debug.Log("should move turret of index" + index + " total turrets to move " + turretsToMove);

    }
    public void StopMovingTurret(int index)  {
        if (shouldMove_moveTurret[index])
        {
            shouldMove_moveTurret[index] = false;
            turretsToMove--;
        }
    }
    public void AddVehicleToMove( ) {
       // turretUpdateArray[turretsToMove] = turret;
        //turretsToMove++;
    }   
    public void moveVehicle(GameObject unit, Vector3 targetLocation, Vector3 unitVelocity, float maxSpeed, float turnSpeed, float timeStep, float acceleration, float dampening) {

        unitposition_moveVehicle[vehiclesToMove] = unit.transform.position;
        targetposition_moveVehicle[vehiclesToMove] = targetLocation;
        unitforward_moveVehicle[vehiclesToMove] = unit.transform.forward;
        unitVelocity_moveVehicle[vehiclesToMove] = unitVelocity;
        unitMaxSpeed_moveVehicle[vehiclesToMove] = maxSpeed;
        unitTurnSpeed_moveVehicle[vehiclesToMove] = turnSpeed;
        deltaTime_moveVehicle[vehiclesToMove] = timeStep;
        acceleration_moveVehicle[vehiclesToMove] = acceleration;
        dampening_moveVehicle[vehiclesToMove] = dampening;
    }

    void Update() {

        shouldCheckDistanceCounter++;
        if (shouldCheckDistanceCounter > 10)
        {
            shouldCheckDistanceCounter = 0;
            shouldCheckDistance = true;
        }
        else
        {
            shouldCheckDistance = false;
        }
        if (elapsed == 0)
        {
         
        }
        if (elapsed >= updateTimestep) //run the order every updateTimestep using the UpdateUnit method
        {
            performanceTimer.Restart();
  
            if (turretsToMove > 0)
            {
                MoveTurrets();
            }
            if (numberOfUnitsToLookForTarget > 0)
            {
                LookForTargets();
            }
         //   UnityEngine.Debug.Log("PerformanceTimer, LookForTargets:  " + performanceTimer.Elapsed.TotalMilliseconds);

            if (transformsToMove > 0)
            {
                MoveTransforms();
            }
            UnityEngine.Debug.Log("PerformanceTimer, all jobs:  " + performanceTimer.Elapsed.TotalMilliseconds);

            elapsed = 0;
        }
        elapsed += Time.deltaTime;


        // JobHandle.CompleteAll(jobList);
        //
        //

        //native arrays for finding target


        //native arrays for moving transform


        //native arrays for moving turrets

        //code to check for targets
        // get a list of potential targets for each faction, and puts it in a flattened float3 array, to be used in the job

        updateUnitPositionsCounter--;
        if (updateUnitPositionsCounter <= 0) // only do it every 40 frames
        {

            targets = new Vector3[1000];
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
            updateUnitPositionsCounter = 120;

        }




        ///////////////////////////////////////////////move turrets///////////////////////////////////////////////

        ////////
        //////


        ///look for target

        //moveturrets


    }

    void MoveTurrets() {


        // UnityEngine.Debug.Log("moveturrets got here1 turrets to move" + turretsToMove);
        NativeArray<float3> turretPosition_moveTurret_Native = new NativeArray<float3>(turretsToMove, Allocator.TempJob);
        NativeArray<float3> targetPosition_moveTurret_Native = new NativeArray<float3>(turretsToMove, Allocator.TempJob);
        NativeArray<float3> gunForward_moveTurret_Native = new NativeArray<float3>(turretsToMove, Allocator.TempJob);
        NativeArray<float3> turretForward_moveTurret_Native = new NativeArray<float3>(turretsToMove, Allocator.TempJob);
        NativeArray<float> deltaTime_moveTurret_Native = new NativeArray<float>(turretsToMove, Allocator.TempJob);
        NativeArray<float> rotationSpeed_moveTurret_Native = new NativeArray<float>(turretsToMove, Allocator.TempJob);
        NativeArray<float> gunrechargeTotal_moveTurret_Native = new NativeArray<float>(turretsToMove, Allocator.TempJob);
        NativeArray<float> gunrechargeCurrent_moveTurret_Native = new NativeArray<float>(turretsToMove, Allocator.TempJob);
        NativeArray<bool> canFire_moveTurret_Native = new NativeArray<bool>(turretsToMove, Allocator.TempJob);
        NativeArray<bool> shouldCheckDistance_moveTurret_Native = new NativeArray<bool>(1, Allocator.TempJob);

        shouldCheckDistance_moveTurret_Native[0] = shouldCheckDistance;
        // UnityEngine.Debug.Log("moveturrets got here2");

        int turretsToMoveCounter = 0;
        for (int i = 0; i < totalTurrets; i++)
        {
            if (shouldMove_moveTurret[i])
            {
                if (unit_moveTurret[i].activeSelf)
                {

                    if (target_moveTurret[i].gameObject.activeSelf)
                    {
                        turretPosition_moveTurret_Native[turretsToMoveCounter] = turretsToMove_moveTurret[i].position;
                        targetPosition_moveTurret_Native[turretsToMoveCounter] = target_moveTurret[i].position;
                        gunForward_moveTurret_Native[turretsToMoveCounter] = gunToMove_moveTurret[i].forward;
                        turretForward_moveTurret_Native[turretsToMoveCounter] = turretsToMove_moveTurret[i].forward;
                        deltaTime_moveTurret_Native[turretsToMoveCounter] = updateTimestep;
                        rotationSpeed_moveTurret_Native[turretsToMoveCounter] = rotationspeed_moveTurret[i];
                        canFire_moveTurret_Native[turretsToMoveCounter] = false;
                        turretsToMoveCounter++;
                    }
                    else
                    {
                        unit_moveTurret[i].GetComponent<UnitBehaviour>().OrderComplete();
                    }
                }
                else
                {
                    shouldMove_moveTurret[i] = false;
                }

            }

        }
        //UnityEngine.Debug.Log("moveturrets got here3");

        moveTurrets = new MoveTurrets {

            unitposition = turretPosition_moveTurret_Native,
            targetposition = targetPosition_moveTurret_Native,
            gunForward = gunForward_moveTurret_Native,
            turretForward = turretForward_moveTurret_Native,
            deltaTime = deltaTime_moveTurret_Native,
            rotationspeed = rotationSpeed_moveTurret_Native,
            canfire = canFire_moveTurret_Native,
            checkDistance = shouldCheckDistance_moveTurret_Native,
        };



        moveTurretJobHandle = moveTurrets.Schedule(turretsToMove, 4);
        moveTurretJobHandle.Complete();
        //  Debug.Log("moveTurretJobHandle is done");

        turretsToMoveCounter = 0;
        for (int i = 0; i < totalTurrets; i++)
        {
            if (shouldMove_moveTurret[i])
            {
                turretsToMove_moveTurret[i].forward = (Vector3)moveTurrets.turretForward[turretsToMoveCounter];
                gunToMove_moveTurret[i].forward = (Vector3)moveTurrets.gunForward[turretsToMoveCounter];
                if (moveTurrets.canfire[turretsToMoveCounter])
                {
                    unit_moveTurret[i].GetComponent<UnitBehaviour>().canFire = true;
                    UnityEngine.Debug.Log("can fire!");
                }
                turretsToMoveCounter++;
                if (turretsToMoveCounter > turretsToMove)
                {
                    break;
                }
            }
        }
        //  Debug.Log("gave movement orders to turrets");
        // UnityEngine.Debug.Log("moveturrets got here5");

        turretPosition_moveTurret_Native.Dispose();
        targetPosition_moveTurret_Native.Dispose();
        gunForward_moveTurret_Native.Dispose();
        turretForward_moveTurret_Native.Dispose();
        deltaTime_moveTurret_Native.Dispose();
        rotationSpeed_moveTurret_Native.Dispose();
        gunrechargeTotal_moveTurret_Native.Dispose();
        gunrechargeCurrent_moveTurret_Native.Dispose();
        canFire_moveTurret_Native.Dispose();


    }
    void MoveTransforms() {
       
        NativeArray<bool> shouldMove_moveTransform_Native = new NativeArray<bool>(transformsToMove, Allocator.TempJob);
        NativeArray<bool> atDestination_moveTransform_Native = new NativeArray<bool>(transformsToMove, Allocator.TempJob);
        NativeArray<float> speed_moveTransform_Native = new NativeArray<float>(transformsToMove, Allocator.TempJob);
        NativeArray<float3> direction_moveTransform_Native = new NativeArray<float3>(transformsToMove, Allocator.TempJob);
        NativeArray<float> timeStep_moveTransform_Native = new NativeArray<float>(1, Allocator.TempJob);
        NativeArray<float3> position_moveTransform_Native = new NativeArray<float3>(transformsToMove, Allocator.TempJob);
        NativeArray<float3> destination_moveTransform_Native = new NativeArray<float3>(transformsToMove, Allocator.TempJob);
        NativeArray<bool> shouldCheckDistance_moveTransform_Native = new NativeArray<bool>(1, Allocator.TempJob);

        shouldCheckDistance_moveTransform_Native[0] = shouldCheckDistance;
        timeStep_moveTransform_Native[0] = updateTimestep;
        int transformsToMoveCounter = 0;
        performanceTimer.Restart();

        for (int i = 0; i < totaltransforms; i++)
        {
            if (shouldMove_MoveTransforms[i])
            {
                position_moveTransform_Native[transformsToMoveCounter] = position_MoveTransforms[i];
                direction_moveTransform_Native[transformsToMoveCounter] = forward_MoveTransforms[i];
                speed_moveTransform_Native[transformsToMoveCounter] = speed_MoveTransforms[i];
                destination_moveTransform_Native[transformsToMoveCounter] = destination_MoveTransforms[i];
                shouldMove_moveTransform_Native[transformsToMoveCounter] = shouldMove_MoveTransforms[i];
                atDestination_moveTransform_Native[transformsToMoveCounter] = false;
                transformsToMoveCounter++;

            }
        }

        performanceTimer.Stop();
      //  UnityEngine.Debug.Log("PerformanceTimer, transfer from normal to native array:  " + performanceTimer.Elapsed.TotalMilliseconds);
        performanceTimer.Restart();

        moveTransforms = new MoveTransforms {

            position = position_moveTransform_Native,
            direction = direction_moveTransform_Native,
            destination = destination_moveTransform_Native,
            speed = speed_moveTransform_Native,
            timeStep = timeStep_moveTransform_Native,
            shouldMove = shouldMove_moveTransform_Native,
            atDestination = atDestination_moveTransform_Native,
            shouldCheckDistance = shouldCheckDistance_moveTransform_Native,
        };


        moveTransformsJobHandle = moveTransforms.Schedule(transformsToMove, 8);
        moveTransformsJobHandle.Complete();


        transformsToMoveCounter = 0;
        for (int i = 0; i < totaltransforms; i++)
        {
            if (shouldMove_MoveTransforms[i])
            {

                transform_MoveTransforms[i].position = moveTransforms.position[transformsToMoveCounter];
                position_MoveTransforms[i] = moveTransforms.position[transformsToMoveCounter];

                if (shouldCheckDistance_moveTransform_Native[0])
                {

                    if (moveTransforms.atDestination[transformsToMoveCounter])
                    {
                        StopMovingTransform(i);

                        transform_MoveTransforms[i].gameObject.GetComponent<UnitBehaviour>().OrderComplete();

                    }
                }
                transformsToMoveCounter++;
                if (transformsToMoveCounter > transformsToMove)
                {
                    break;
                }
            }

        }
        shouldMove_moveTransform_Native.Dispose();
        atDestination_moveTransform_Native.Dispose();
        speed_moveTransform_Native.Dispose();
        direction_moveTransform_Native.Dispose();
        timeStep_moveTransform_Native.Dispose();
        position_moveTransform_Native.Dispose();
        destination_moveTransform_Native.Dispose();
    }

    void LookForTargets() {
        if (numberOfUnitsToLookForTarget == 400)
        {
            numberOfUnitsToLookForTarget = 0;
        }
        if (numberOfUnitsToLookForTarget > 0)
        {

            NativeArray<int> faction_LookforTarget_Native = new NativeArray<int>(numberOfUnitsToLookForTarget, Allocator.TempJob);
            NativeArray<bool> isMilitary_LookforTarget_Native = new NativeArray<bool>(totalNumberOfTargets, Allocator.TempJob);
            NativeArray<float3> unitPosition_LookforTarget_Native = new NativeArray<float3>(numberOfUnitsToLookForTarget, Allocator.TempJob);
            NativeArray<float3> targetPositions_LookforTarget_Native = new NativeArray<float3>(totalNumberOfTargets, Allocator.TempJob);
            NativeArray<int> factionTargetCount_LookforTarget_Native = new NativeArray<int>(numberOfPlayers, Allocator.TempJob);
            NativeArray<int> closestMilitaryTarget_LookforTarget_Native = new NativeArray<int>(numberOfUnitsToLookForTarget, Allocator.TempJob);
            NativeArray<int> closestNonMilitaryTarget_LookforTarget_Native = new NativeArray<int>(numberOfUnitsToLookForTarget, Allocator.TempJob);
            NativeArray<int> range_LookforTarget_Native = new NativeArray<int>(numberOfUnitsToLookForTarget, Allocator.TempJob);
            NativeArray<float> currentDistance_LookforTarget_Native = new NativeArray<float>(numberOfUnitsToLookForTarget, Allocator.TempJob);
            NativeArray<float> minDistance_LookforTarget_Native = new NativeArray<float>(numberOfUnitsToLookForTarget, Allocator.TempJob);
            NativeArray<int> stopCounting_LookforTarget_Native = new NativeArray<int>(numberOfPlayers, Allocator.TempJob);
            NativeArray<int> startCountingAgain_LookforTarget_Native = new NativeArray<int>(numberOfPlayers, Allocator.TempJob);

            for (int i = 0; i < numberOfUnitsToLookForTarget; i++)
            {
   
                faction_LookforTarget_Native[i] = unitsToLookForTarget[i].faction;

                currentDistance_LookforTarget_Native[i] = 10000;

                minDistance_LookforTarget_Native[i] = 10000;

                range_LookforTarget_Native[i] = unitRange[i];

                unitPosition_LookforTarget_Native[i] = unitsToLookForTarget[i].gameObject.transform.position;

                //  Debug.Log("faction " + faction[i] + " postition " + unitposition[i]);
            }

   
            for (int i = 0; i < totalNumberOfTargets; i++)
            {

                targetPositions_LookforTarget_Native[i] = targets[i];

                isMilitary_LookforTarget_Native[i] = military[i];

            }


            for (int i = 0; i < numberOfPlayers; i++)
            {

                factionTargetCount_LookforTarget_Native[i] = numberOfTargetsPerFaction[i];

                stopCounting_LookforTarget_Native[i] = stopCounting_notinjob[i];

                startCountingAgain_LookforTarget_Native[i] = startCountingAgain_notinjob[i];


            }

  
            lookForTarget = new LookForTarget {
                faction = faction_LookforTarget_Native,
                unitposition = unitPosition_LookforTarget_Native,
                targetpositions = targetPositions_LookforTarget_Native,
                factionTargetCount = factionTargetCount_LookforTarget_Native,
                closestMilitaryTarget = closestMilitaryTarget_LookforTarget_Native,
                closestNonMilitaryTarget = closestNonMilitaryTarget_LookforTarget_Native,
                isMilitary = isMilitary_LookforTarget_Native,
                currentDistance = currentDistance_LookforTarget_Native,
                minDistance = minDistance_LookforTarget_Native,
                stopCounting = stopCounting_LookforTarget_Native,
                startCountingAgain = startCountingAgain_LookforTarget_Native,
                range = range_LookforTarget_Native
            };


 
            lookForTargetJobHandle = lookForTarget.Schedule(numberOfUnitsToLookForTarget, 1);
            lookForTargetJobHandle.Complete();


            for (int i = 0; i < numberOfUnitsToLookForTarget - 1; i++)
            {
     
                if (closestMilitaryTarget_LookforTarget_Native[i] != 1000)
                {


                    //  unitsToLookForTarget[i].gameObject.GetComponent<UnitBehaviour>().Attack(currentTarget);
                 
                        unitsToLookForTarget[i].gameObject.GetComponent<UnitBehaviour>().Attack(target[lookForTarget.closestMilitaryTarget[i]].gameObject);
                        // Debug.Log("FINAL ASSIGNMENT unit " + unitsToLookForTarget[i].gameObject.name + " is targeting " + target[lookForTarget.closestMilitaryTarget[i]].gameObject);

                 
                    // Debug.Log("FINAL ASSIGNMENT unit " + unitsToLookForTarget[i].gameObject.name + " is targeting " + target[lookForTarget.closestMilitaryTarget[i]].gameObject);


                }
                else
                {
                  //  unitsToLookForTarget[i].gameObject.GetComponent<UnitBehaviour>().TargetGone();
                }
                //   Debug.Log(unitsToLookForTarget[i].gameObject.name + " found target with unit number " + lookForTarget.closestTarget[i]);
            }

        }
        numberOfUnitsToLookForTarget = 0;
        faction_LookforTarget_Native.Dispose();
        unitPosition_LookforTarget_Native.Dispose();
        targetPositions_LookforTarget_Native.Dispose();
        factionTargetCount_LookforTarget_Native.Dispose();
        closestMilitaryTarget_LookforTarget_Native.Dispose();
        closestNonMilitaryTarget_LookforTarget_Native.Dispose();
        isMilitary_LookforTarget_Native.Dispose();
        currentDistance_LookforTarget_Native.Dispose();
        minDistance_LookforTarget_Native.Dispose();
        stopCounting_LookforTarget_Native.Dispose();
        startCountingAgain_LookforTarget_Native.Dispose();
        range_LookforTarget_Native.Dispose();

    }


    // Update is called once per frame
  
}
[BurstCompile]

public struct MoveTransforms : IJobParallelFor
{
    public NativeArray<float3> position;
    public NativeArray<float3> destination;
    public NativeArray<float> speed;
    public NativeArray<float3> direction;
    public NativeArray<float> timeStep;
    public NativeArray<bool> shouldMove;
    public NativeArray<bool> shouldCheckDistance;
    public NativeArray<bool> atDestination;
    public void Execute(int index) {
        //    Debug.Log("movetransforms injob " + index);
        position[index] += speed[index] * direction[index] * timeStep[0];

        //Debug.Log("movetransforms injob should check distance " + shouldCheckDistance[0]);

        if (shouldCheckDistance[0])
        {

            if (math.distance(position[index], destination[index]) < 2)
            {
                atDestination[index] = true;

            }

        }

    }
}
[BurstCompile]

public struct MoveTurrets : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<float3> unitposition;
    [ReadOnly]
    public NativeArray<float3> targetposition;
    public NativeArray<float3> gunForward;
    public NativeArray<float3> turretForward;
    [ReadOnly]
    public NativeArray<float> deltaTime;
    [ReadOnly]
    public NativeArray<float> rotationspeed;
    public NativeArray<bool> canfire;
    [ReadOnly]
    public NativeArray<bool> checkDistance;
    public NativeArray<bool> inRange;
    public void Execute(int index) {

        rotationspeed[index] *= deltaTime[index];
        float3 direction = math.normalize(targetposition[index] - unitposition[index]);
        turretForward[index] = math.lerp(turretForward[index], new float3(direction.x, 0, direction.z), rotationspeed[index]);
        gunForward[index] = math.lerp(gunForward[index], new float3(0, direction.y, 0), rotationspeed[index]);
        gunForward[index] = new float3(turretForward[index].x, gunForward[index].y, turretForward[index].z);
        if (checkDistance[0])
        {
            if (math.distance(turretForward[index], direction) < 0.1f)
            {
                canfire[index] = true;

            }
            else
            {
                canfire[index] = false;

            }
        }

    }
}
public struct moveVehicles : IJobParallelFor
{
    public NativeArray<float3> unitposition;
    public NativeArray<float3> targetposition;
    public NativeArray<float3> unitforward;
    public NativeArray<float3> unitVelocity;
    public NativeArray<float> unitMaxSpeed;
    public NativeArray<float> unitTurnSpeed;
    public NativeArray<float> deltaTime;
    public NativeArray<float> acceleration;
    public NativeArray<float> dampening;
    public NativeArray<bool> atTarget;


    public void Execute(int index) {
        unitTurnSpeed[index] *= deltaTime[index];
        unitMaxSpeed[index] *= deltaTime[index];

        //turn the unit 
        float3 direction = math.normalize(targetposition[index] - unitposition[index]);
        unitforward[index] = math.lerp(unitforward[index], new float3(direction.x, 0, direction.z), unitTurnSpeed[index]);
        if (math.distance(unitforward[index], direction) < 0.5f)// if the vehicle is facing the target
        {
            unitVelocity[index] += unitforward[index] * acceleration[index] * deltaTime[index];
            unitVelocity[index] *= dampening[index];
        }
        float distanceToTarget = math.distance(unitposition[index], targetposition[index]);
        if (distanceToTarget < 5)// if the vehicle is facing the target
        {
            unitVelocity[index] *= 0.5f;
        }
        if (distanceToTarget < 1)// if the vehicle is facing the target
        {
            unitVelocity[index] *= 0.1f;
            atTarget[index] = true;
        }
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
        // Debug.Log("look for target11");

        for (int i = 0; i < targetpositions.Length; i++)
        {
            if (i < stopCounting[faction[index]] || i > startCountingAgain[faction[index]])
            {
                currentDistance[index] = math.distance(unitposition[index], targetpositions[i]);
                {
                    if (currentDistance[index] < minDistance[index])
                    {
                        if (isMilitary[i])
                        {
                            minDistance[index] = currentDistance[index];
                            closestMilitaryTarget[index] = i;
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
        }
        // Debug.Log("look for target12");

    }
}
