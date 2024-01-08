using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using System.Diagnostics;
using UnityEngine.ParticleSystemJobs;
//using Unity.Entities;
//using Unity.Rendering;
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
    float3[] position_MoveTransforms = new float3[200];
    float3[] destination_MoveTransforms = new float3[200];

    float3[] forward_MoveTransforms = new float3[200];
    float[] speed_MoveTransforms = new float[200];
    float[] timestep_MoveTransforms = new float[200];
    Transform[] transform_MoveTransforms = new Transform[200];
    int transformsToMove;
    public bool[] shouldMove_MoveTransforms = new bool[200];
    int checkDistanceCounter_MoveTransfors;


    //variables for LookForTarget////////////////////////////
    JobHandle lookForTargetJobHandle;
    LookForTarget lookForTarget;

    public int numberOfUnitsToLookForTarget = 0;
    Unit[] unitsToLookForTarget = new Unit[400];
    Unit[] target = new Unit[400];
    int updateUnitPositionsCounter = 40;
    int totalNumberOfTargets = 400;

    float3[,] targetsToCheck = new float3[100, 100];
    Vector2[] targets = new Vector2[100];
    int[] numberOfTargetsPerFaction = new int[100];
    int[] unitsToLookForTargetCounter = new int[100];
    int[] stopCounting_notinjob = new int[100];
    int[] startCountingAgain_notinjob = new int[100];
    int[] unitRange = new int[100];
    bool[] military;
    LookForTargetRequest[] lookForTargetRequest = new LookForTargetRequest[100];
    LookForTargetUnitList[] lookForTargetUnitList = new LookForTargetUnitList[400];

    //variables for moveTurret/////////////////////////////////////////
    JobHandle moveTurretJobHandle;
    MoveTurrets moveTurrets;
    int turretsToMove = 0;
    int totalTurrets = 0;
    GameObject[] unit_moveTurret = new GameObject[400];
    Transform[] turretsToMove_moveTurret = new Transform[400];
    Transform[] gunToMove_moveTurret = new Transform[400];
    Transform[] target_moveTurret = new Transform[400];
    TurretToMove[] turretsToMoveStruct = new TurretToMove[400];


    //variables for moveVehilces/////////////////////////////////////////
    JobHandle moveVehiclesJobHandle;
    MoveVehicles moveVehicles;

    int totalVehicles;
    int numberOfVehiclesToMove = 0;

    VehicleToMove[] vehiclesToMove = new VehicleToMove[400];
    Transform[] vehicleTransforms = new Transform[400];



    //   JobHandle createParticlesJobHAndle = new JobHandle();
    JobHandle createProjectilesJobHandle;

    Projectile[] projectiles_createProjectile = new Projectile[400];
    ParticleSystem[] shots_createProjectile = new ParticleSystem[400];
    ParticleSystem[] hits_createProjectile = new ParticleSystem[400];
    UnitBehaviour[] targets_createProjectile = new UnitBehaviour[400];
    int projectilesToCreate = 0;
    List<ProjectileHit> projectileHitList = new List<ProjectileHit>();
    Collider[] hitColliders = new Collider[10];
    int numberOfHits;

    public float updateTimestep = 0.0416f;

    public bool assumingDirectControl;
    public UnitBehaviour unitUnderDirectControl;

    int shouldCheckDistanceCounter = 0;
    int shouldCheckDistance = 0;
    float counter = 0;
    float elapsed = 0;
    float halfTimestep;

    void Awake() {
        gameInformation = gameObject.GetComponent<GlobalGameInformation>();
        numberOfPlayers = gameInformation.numberOfPlayers; 

    }
    private void Start() {
        UpdateUnitPositions();
    }
    void Update() {
        ProjectileHit();
        elapsed += Time.deltaTime;
        if (counter == 0)
        {
            if (elapsed > updateTimestep)
                elapsed = 0;
            foreach (List<Unit> faction in gameInformation.unitList)
            {
                foreach (Unit unit in faction)
                {
                    unit.unitBehaviour.UpdateUnit();
                }
                }
            {

            }
         //  UnityEngine.Debug.Log("numofunits " + numofunits);

            if (numberOfUnitsToLookForTarget > 0)
            {
                performanceTimer.Restart();
                LookForTargets();

                if (performanceTimer.Elapsed.TotalMilliseconds > 0.2f)
                {

                    //  UnityEngine.Debug.Log("PerformanceTimer, LookForTargets  " + performanceTimer.Elapsed.TotalMilliseconds + " how many? " + lookfortargetnum);
                }
            }
            if (Time.deltaTime > 0.015)
            {
                if (QualitySettings.lodBias > 0.7f)
                {
                    QualitySettings.lodBias -= 0.001f;
                }

            }
            else if (Time.deltaTime < 0.013)
            {
                if (QualitySettings.lodBias < 1.4f)
                {
                    QualitySettings.lodBias += 0.001f;
                }
            }

        }

        if (counter == 1) //run the order every updateTimestep using the UpdateUnit method
        {

            if (numberOfVehiclesToMove > 0)
            {
                performanceTimer.Restart();

                MoveVehicles();
                if (performanceTimer.Elapsed.TotalMilliseconds > 0.2f)
                {

                     //   UnityEngine.Debug.Log("PerformanceTimer, MoveVehicles  " + performanceTimer.Elapsed.TotalMilliseconds + " how many? " + vehiclesToMove);
                }

            }
            if (projectilesToCreate > 0)
            {
                CreateProjectiles();

            }

            //   UnityEngine.Debug.Log("PerformanceTimer, LookForTargets:  " + performanceTimer.Elapsed.TotalMilliseconds);

            if (transformsToMove > 0)
            {
                performanceTimer.Restart();

                MoveTransforms();
                if (performanceTimer.Elapsed.TotalMilliseconds > 0.2f)
                {

                    //   UnityEngine.Debug.Log("PerformanceTimer, MoveTransforms  " + performanceTimer.Elapsed.TotalMilliseconds + " how many? " + transformsToMove);
                }
            }



        }
        if (counter == 2)
        {
            counter = 0;
            if (turretsToMove > 0)
            {

                performanceTimer.Restart();
                MoveTurrets();
                if (performanceTimer.Elapsed.TotalMilliseconds > 0.2f)
                {

                    //  UnityEngine.Debug.Log("PerformanceTimer, MoveTurrets  " + performanceTimer.Elapsed.TotalMilliseconds + " how many? " + turretsToMove);
                }
            }

        }
        else
        {
            counter++;
        }

        shouldCheckDistanceCounter++;
        if (shouldCheckDistanceCounter >= 5)
        {
            shouldCheckDistanceCounter = 0;
            shouldCheckDistance = 1;
        }
        else
        {
            shouldCheckDistance = 0;
        }
        updateUnitPositionsCounter++;

        if (updateUnitPositionsCounter >= 200)
        {
            UpdateUnitPositions();

        }
        if (assumingDirectControl)
        {
            unitUnderDirectControl.DirectControl();
        }
    }

    public void newProjectile(ParticleSystem shot, ParticleSystem hit, Vector3 startLocation, Vector3 endLocation, UnitBehaviour target = null, int aoe = 0, int damage = 0, float speed = 150, bool directionalHit = false) {

        projectiles_createProjectile[projectilesToCreate] = new Projectile {
            startLocation = startLocation,
            endLocation = endLocation,
            damage = damage,
            aoe = aoe,
            speed = speed,
            directionalHit = directionalHit
        };
        shots_createProjectile[projectilesToCreate] = shot;
        hits_createProjectile[projectilesToCreate] = hit;
        targets_createProjectile[projectilesToCreate] = target;
        projectilesToCreate++;
    }
    public void LookForTarget(Unit unit, int range) {
        if (numberOfUnitsToLookForTarget > 30)
        {
            numberOfUnitsToLookForTarget = 30;
        }
        unitsToLookForTarget[numberOfUnitsToLookForTarget] = unit;
        lookForTargetRequest[numberOfUnitsToLookForTarget].range = range;
        lookForTargetRequest[numberOfUnitsToLookForTarget].faction = unit.faction;
        lookForTargetRequest[numberOfUnitsToLookForTarget].distance = 10000;
        lookForTargetRequest[numberOfUnitsToLookForTarget].unitPosition = new float2(unit.gameObject.transform.position.x, unit.gameObject.transform.position.z);
        if (unit.military)
        {
            lookForTargetRequest[numberOfUnitsToLookForTarget].military = 1;
        }
        else
        {
            lookForTargetRequest[numberOfUnitsToLookForTarget].military = 0;
        }
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

        turretsToMoveStruct[totalTurrets].rotationSpeed = rotateSpeed;
        turretsToMoveStruct[totalTurrets].canFire = 0;
        turretsToMoveStruct[totalTurrets].shouldMove = 0;
        unit.GetComponent<UnitBehaviour>().turretNumber = totalTurrets;
        totalTurrets++;
    }
    public void MoveTurret(int index, Transform target) {
        if (turretsToMoveStruct[index].shouldMove == 0)
        {
            turretsToMove++;

            turretsToMoveStruct[index].shouldMove = 1;

        }
        // UnityEngine.Debug.Log("moveturret " + totalTurrets);

        if (target != target_moveTurret[index])
        {
            target_moveTurret[index] = target;
        }
        //    UnityEngine.Debug.Log("should move turret of index" + index + " total turrets to move " + turretsToMove);

    }
    public void StopMovingTurret(int index) {
        if (turretsToMoveStruct[index].shouldMove == 1)
        {
            turretsToMoveStruct[index].shouldMove = 0;
            turretsToMove--;
        }
    }
    public void AddVehicleToMove(Transform vehicleTransform, float maxSpeed, float turnSpeed, float acceleration) {
      vehiclesToMove[totalVehicles] = new VehicleToMove { acceleration = acceleration, maxSpeed = maxSpeed, turnSpeed = turnSpeed, shouldMove = 0, velocity = 0 };
        //UnityEngine.Debug.Log("!! vehiclesToMove[totalVehicles].accel " + vehiclesToMove[totalVehicles].acceleration);

        vehicleTransforms[totalVehicles] = vehicleTransform;
        vehicleTransform.gameObject.GetComponent<UnitBehaviour>().vehicleNumber = totalVehicles;

        totalVehicles++;
    }
    public void MoveVehicle(int vehicleNumber, Vector3 destination, bool finalDestination) {
        if (vehiclesToMove[vehicleNumber].shouldMove == 0)
        {
            numberOfVehiclesToMove++;

            vehiclesToMove[vehicleNumber].shouldMove = 1;
            vehiclesToMove[vehicleNumber].destination = destination;
            if (finalDestination)
            {
                vehiclesToMove[vehicleNumber].finalDestination = 1;
            }

        }

        if (destination != (Vector3)vehiclesToMove[vehicleNumber].destination)//if it's a new destination
        {
            vehiclesToMove[vehicleNumber].destination = destination;
            if (finalDestination)
            {
                vehiclesToMove[vehicleNumber].finalDestination = 1;
            }
            else
            {
                vehiclesToMove[vehicleNumber].finalDestination = 0;

            }
        }
        UnityEngine.Debug.Log("JobManager MoveVehicle, tank got move order to " + destination);

    }
    public void StopMovingVehicle(int vehicleNumber) {
        if (vehiclesToMove[vehicleNumber].shouldMove == 1)
        {
            vehiclesToMove[vehicleNumber].shouldMove = 0;
            numberOfVehiclesToMove--;
        }

    }



    void UpdateUnitPositions() {
        numberOfTargetsPerFaction = new int[numberOfPlayers];
        totalNumberOfTargets = 0;
        for (int currentfaction = 0; currentfaction < numberOfPlayers; currentfaction++)
        {
            foreach (Unit unitToCheck in gameInformation.unitList[currentfaction])
            {
                lookForTargetUnitList[totalNumberOfTargets].targetPosition = new Vector2(unitToCheck.gameObject.transform.position.x, unitToCheck.gameObject.transform.position.z);
                if (unitToCheck.military)
                {
                    lookForTargetUnitList[totalNumberOfTargets].military = 1;

                }
                else
                {
                    lookForTargetUnitList[totalNumberOfTargets].military = 0;

                }
                target[totalNumberOfTargets] = unitToCheck;
                totalNumberOfTargets++;
                
            }
            numberOfTargetsPerFaction[currentfaction] = gameInformation.unitList[currentfaction].Count;
            lookForTargetUnitList[currentfaction].stopCounting = 0;
            for (int i = 0; i < currentfaction; i++)
            {
                lookForTargetUnitList[currentfaction].stopCounting += numberOfTargetsPerFaction[i];
            }
            lookForTargetUnitList[currentfaction].startCounting = lookForTargetUnitList[currentfaction].stopCounting + numberOfTargetsPerFaction[currentfaction];
        }
        updateUnitPositionsCounter = 120;
        lookForTargetUnitList[0].totalNumberOfUnits = totalNumberOfTargets;
    }

    void ProjectileHit() {

        foreach (ProjectileHit projectileHit in projectileHitList.ToArray())
        {

            if (Time.time > projectileHit.time)
            {
                if (projectileHit.animation != null)
                {
                    projectileHit.animation.Play();

                }
                if (projectileHit.aoe == 0)
                {
                    if (projectileHit.target != null)
                    {
                        projectileHit.target.changeHP(-projectileHit.damage);
                    }
                }
                else
                {
                    numberOfHits = Physics.OverlapSphereNonAlloc(projectileHit.location, projectileHit.aoe, hitColliders);

                    for (int j = 0; j < numberOfHits; j++)
                    {
                        if (hitColliders[j].gameObject.TryGetComponent<UnitBehaviour>(out UnitBehaviour unitBehaviour))
                        {
                            unitBehaviour.changeHP(-projectileHit.damage);
                            //  UnityEngine.Debug.Log("hit " + unitBehaviour.gameObject.name + " for damage: " + projectileHit.damage);
                        }
                    }
                }
                projectileHitList.Remove(projectileHit);
            }

        }

    }
    void CreateProjectiles() {
        if (projectilesToCreate > 200)
        {
            UnityEngine.Debug.Log("projectilestoCreate 0" + projectilesToCreate);

        }

        // UnityEngine.Debug.Log("timer 1 "  + performanceTimer.Elapsed.TotalMilliseconds);
        performanceTimer.Restart();
        NativeArray<Projectile> projectiles_projectilesToCreate_Native = new NativeArray<Projectile>(projectilesToCreate, Allocator.TempJob);
        TransformAccessArray projectileTransforms_projectilesToCreate_Native = new TransformAccessArray(projectilesToCreate * 2);
        NativeArray<float> distances_projectilesToCreate_Native = new NativeArray<float>(projectilesToCreate, Allocator.TempJob);
        NativeArray<float> time_projectilesToCreate_Native = new NativeArray<float>(projectilesToCreate, Allocator.TempJob);
        int badProjectiles = 0;
        for (int i = 0; i < projectilesToCreate; i++)
        {
            projectiles_projectilesToCreate_Native[i] = projectiles_createProjectile[i];
            projectileTransforms_projectilesToCreate_Native.Add(shots_createProjectile[i].gameObject.transform);
            projectileTransforms_projectilesToCreate_Native.Add(hits_createProjectile[i].gameObject.transform);
        }
        projectilesToCreate -= badProjectiles;
        //  UnityEngine.Debug.Log("timer 2 " + performanceTimer.Elapsed.TotalMilliseconds);
        performanceTimer.Restart();

        CreateProjectile createProjectile = new CreateProjectile() {
            projectile = projectiles_projectilesToCreate_Native,
            distance = distances_projectilesToCreate_Native,
            time = time_projectilesToCreate_Native,
        };
        createProjectilesJobHandle = createProjectile.Schedule(projectileTransforms_projectilesToCreate_Native);
        createProjectilesJobHandle.Complete();
        //UnityEngine.Debug.Log("timer 3 " + performanceTimer.Elapsed.TotalMilliseconds);
        performanceTimer.Restart();

        for (int i = 0; i < projectilesToCreate; i++)
        {
            if (createProjectile.time[i] > 0)
            {
                var main = shots_createProjectile[i].main;
                main.startLifetime = createProjectile.time[i];
                shots_createProjectile[i].Play();
                projectileHitList.Add(new ProjectileHit { time = createProjectile.time[i] + Time.time, animation = hits_createProjectile[i], aoe = projectiles_createProjectile[i].aoe, damage = projectiles_createProjectile[i].damage, target = targets_createProjectile[i], location = projectiles_createProjectile[i].endLocation });
            }
        }

        projectilesToCreate = 0;
        // UnityEngine.Debug.Log("timer 4 " + performanceTimer.Elapsed.TotalMilliseconds);
        performanceTimer.Restart();

        time_projectilesToCreate_Native.Dispose();
        distances_projectilesToCreate_Native.Dispose();
        projectiles_projectilesToCreate_Native.Dispose();
        projectileTransforms_projectilesToCreate_Native.Dispose();
        //  UnityEngine.Debug.Log("timer 5 " + performanceTimer.Elapsed.TotalMilliseconds);

    }

    void MoveTurrets() {


        // UnityEngine.Debug.Log("moveturrets got here1 turrets to move" + turretsToMove);
        NativeArray <TurretToMove> turretsToMove_native = new NativeArray<TurretToMove>(turretsToMove, Allocator.TempJob);
        TransformAccessArray transformAccessArray = new TransformAccessArray(turretsToMove*2);

        // UnityEngine.Debug.Log("moveturrets got here2");

        int turretsToMoveCounter = 0;
        for (int i = 0; i < totalTurrets; i++)
        {
            if (turretsToMoveStruct[i].shouldMove == 1)
            {
                if (unit_moveTurret[i] != null)
                {

                    if (target_moveTurret[i] != null)
                    {
                        turretsToMoveStruct[i].targetPosition = target_moveTurret[i].position;
                        turretsToMoveStruct[i].gunPosition = gunToMove_moveTurret[i].position;
                        turretsToMoveStruct[i].deltaTime = updateTimestep;
                        turretsToMoveStruct[i].shouldCheckDistance = shouldCheckDistance;
                        turretsToMove_native[turretsToMoveCounter] = turretsToMoveStruct[i];
                        transformAccessArray.Add(turretsToMove_moveTurret[i]);
                        transformAccessArray.Add(gunToMove_moveTurret[i]);
                        turretsToMoveCounter++;
                    }
                    else
                    {
                        unit_moveTurret[i].GetComponent<UnitBehaviour>().TargetGone();
                        turretsToMoveStruct[i].shouldMove = 0;

                    }
                }
                else
                {
                    turretsToMoveStruct[i].shouldMove = 0;
                }

            }

        }
        moveTurrets = new MoveTurrets {
            turret = turretsToMove_native
        };

        moveTurretJobHandle = moveTurrets.Schedule(transformAccessArray);
        moveTurretJobHandle.Complete();
        turretsToMoveCounter = 0;
        for (int i = 0; i < turretsToMove_native.Length; i++)
        {
            if (turretsToMove_native[i].shouldMove == 1)
            {

                if (turretsToMove_native[i].canFire ==1)
                {
                    if (unit_moveTurret[i] != null)
                    {
                 
                        unit_moveTurret[i].GetComponent<UnitBehaviour>().canFire = true;
                    }

                }
                turretsToMoveCounter++;
                if (turretsToMoveCounter > turretsToMove)
                {
                    break;
                }
            }
        }
        turretsToMove_native.Dispose();
        transformAccessArray.Dispose();


    }
    void MoveVehicles() {


    //     UnityEngine.Debug.Log("Jobmanager, MoveVehicles got here1, total veh to move: " + numberOfVehiclesToMove);
        NativeArray<VehicleToMove> vehiclesToMove_Native = new NativeArray<VehicleToMove>(vehiclesToMove, Allocator.TempJob);
        TransformAccessArray transformAccessArray = new TransformAccessArray(numberOfVehiclesToMove);
        
        int vehiclesToMoveCounter = 0;
        for (int i = 0; i < totalVehicles; i++)
        {
            if (vehiclesToMove[i].shouldMove == 1)
            {
                if (vehicleTransforms[i] != null)//if the transform still exists
                {
                    vehiclesToMove[i].shouldCheckDistance = shouldCheckDistance;
 
                    vehiclesToMove[i].deltaTime = updateTimestep;
                  //  UnityEngine.Debug.Log("Jobmanager, MoveVehicles got here 2 pos: " + vehicleTransforms[i].position +" destination: " + vehiclesToMove[i].destination);

                    vehiclesToMove_Native[vehiclesToMoveCounter] = vehiclesToMove[i];
                 //   UnityEngine.Debug.Log("Jobmanager, MoveVehicles got here 2 native destination: " + vehiclesToMove_Native[vehiclesToMoveCounter].destination + " i = " + i + " vehiclesToMoveCounter " + vehiclesToMoveCounter);

                    transformAccessArray.Add(vehicleTransforms[i]);
                    vehiclesToMoveCounter++;
                }
                else
                {
                    vehiclesToMove[i].shouldMove = 0;
                }

            }

        }

        moveVehicles = new MoveVehicles {
            vehiclesToMove = vehiclesToMove_Native,
        };

     //   UnityEngine.Debug.Log("Jobmanager, MoveVehicles got here3 about to complete");

        moveVehiclesJobHandle = moveVehicles.Schedule(transformAccessArray);
        moveVehiclesJobHandle.Complete();
     //   UnityEngine.Debug.Log("Jobmanager, MoveVehicles got here4 complete");

        //  Debug.Log("moveTurretJobHandle is done");
      //  UnityEngine.Debug.Log("Jobmanager, MoveVehicles got here5");

        vehiclesToMoveCounter = 0;
        for (int i = 0; i < totalVehicles; i++)
        {
            if (vehiclesToMove[i].shouldMove == 1)
            {
                vehiclesToMove[i] = vehiclesToMove_Native[i];
                if (vehiclesToMove_Native[i].atDestination == 1)
                {
                    vehicleTransforms[i].gameObject.GetComponent<UnitBehaviour>().OrderComplete();
                    if (vehiclesToMove_Native[i].finalDestination == 1)
                    {
                        StopMovingVehicle(i);
                    
                    }
                }
                vehiclesToMoveCounter++;
                if (vehiclesToMoveCounter > numberOfVehiclesToMove)
                {
                    break;
                }
            }
        }
        //  Debug.Log("gave movement orders to turrets");
        // UnityEngine.Debug.Log("moveturrets got here5");
     //   UnityEngine.Debug.Log("Jobmanager, MoveVehicles got here6");


        vehiclesToMove_Native.Dispose();
        transformAccessArray.Dispose();
     //   UnityEngine.Debug.Log("Jobmanager, MoveVehicles got here7 done");


    }
    void MoveTransforms() {

        NativeArray<bool> shouldMove_moveTransform_Native = new NativeArray<bool>(transformsToMove, Allocator.TempJob);
        NativeArray<bool> atDestination_moveTransform_Native = new NativeArray<bool>(transformsToMove, Allocator.TempJob);
        NativeArray<float> speed_moveTransform_Native = new NativeArray<float>(transformsToMove, Allocator.TempJob);
        NativeArray<float3> direction_moveTransform_Native = new NativeArray<float3>(transformsToMove, Allocator.TempJob);
        NativeArray<float> timeStep_moveTransform_Native = new NativeArray<float>(1, Allocator.TempJob);
        NativeArray<float3> position_moveTransform_Native = new NativeArray<float3>(transformsToMove, Allocator.TempJob);
        NativeArray<float3> destination_moveTransform_Native = new NativeArray<float3>(transformsToMove, Allocator.TempJob);
        NativeArray<int> shouldCheckDistance_moveTransform_Native = new NativeArray<int>(1, Allocator.TempJob);

        shouldCheckDistance_moveTransform_Native[0] = shouldCheckDistance;
        timeStep_moveTransform_Native[0] = gameInformation.updateTimestep;
        int transformsToMoveCounter = 0;
        performanceTimer.Restart();

        for (int i = 0; i < totaltransforms; i++)
        {
            if (shouldMove_MoveTransforms[i])
            {
                if (transform_MoveTransforms[i].gameObject.activeInHierarchy)
                {

                    position_moveTransform_Native[transformsToMoveCounter] = position_MoveTransforms[i];
                    direction_moveTransform_Native[transformsToMoveCounter] = forward_MoveTransforms[i];
                    speed_moveTransform_Native[transformsToMoveCounter] = speed_MoveTransforms[i];
                    destination_moveTransform_Native[transformsToMoveCounter] = destination_MoveTransforms[i];
                    shouldMove_moveTransform_Native[transformsToMoveCounter] = shouldMove_MoveTransforms[i];
                    atDestination_moveTransform_Native[transformsToMoveCounter] = false;
                    transformsToMoveCounter++;
                }
                else
                {
                    transformsToMove--;
                    shouldMove_MoveTransforms[i] = false;
                }
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

                if (shouldCheckDistance_moveTransform_Native[0] == 1)
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

        NativeArray<LookForTargetRequest> lookForTargetRequest_Native = new NativeArray<LookForTargetRequest>(numberOfUnitsToLookForTarget, Allocator.TempJob);
        NativeArray<LookForTargetUnitList> lookForTargetUnitList_Native = new NativeArray<LookForTargetUnitList>(totalNumberOfTargets, Allocator.TempJob);

        for (int i = 0; i < numberOfUnitsToLookForTarget; i++)
        {
lookForTargetRequest_Native[i] = lookForTargetRequest[i];
        }

        for (int i = 0; i < totalNumberOfTargets; i++)
        {
            lookForTargetUnitList_Native[i] = lookForTargetUnitList[i];
        }


        lookForTarget = new LookForTarget {
             lookForTargetRequest = lookForTargetRequest_Native,
             lookForTargetUnitList = lookForTargetUnitList_Native,
        };



        lookForTargetJobHandle = lookForTarget.Schedule(numberOfUnitsToLookForTarget, 2);
        lookForTargetJobHandle.Complete();

        for (int i = 0; i < numberOfUnitsToLookForTarget; i++)
        {
            if (i < lookForTargetRequest_Native.Length && i < unitsToLookForTarget.Length)
            {
         
            if (lookForTargetRequest_Native[i].closestMilitaryTarget != 10000 && unitsToLookForTarget[i].gameObject != null)
            {

                    //  unitsToLookForTarget[i].gameObject.GetComponent<UnitBehaviour>().Attack(currentTarget);
                   // UnityEngine.Debug.Log("closestMilitaryTarget = " + lookForTargetRequest_Native[i].closestMilitaryTarget);
                   // UnityEngine.Debug.Log("target.Length " + target.Length);

                    unitsToLookForTarget[i].gameObject.GetComponent<UnitBehaviour>().Attack(target[lookForTargetRequest_Native[i].closestMilitaryTarget].gameObject);
                // Debug.Log("FINAL ASSIGNMENT unit " + unitsToLookForTarget[i].gameObject.name + " is targeting " + target[lookForTarget.closestMilitaryTarget[i]].gameObject);




            }
            else
            {
                unitsToLookForTarget[i].gameObject.GetComponent<UnitBehaviour>().TargetGone();
                }
            }
        }


        numberOfUnitsToLookForTarget = 0;
        lookForTargetRequest_Native.Dispose();
        lookForTargetUnitList_Native.Dispose();


    }


    // Update is called once per frame

}
[BurstCompile]

public struct MoveTransforms : IJobParallelFor
{


    public NativeArray<float3> position;
    [ReadOnly]
    public NativeArray<float3> destination;
    [ReadOnly]
    public NativeArray<float> speed;
    [ReadOnly]
    public NativeArray<float3> direction;
    [ReadOnly]
    public NativeArray<float> timeStep;
    [ReadOnly]
    public NativeArray<int> shouldCheckDistance;
    public NativeArray<bool> atDestination;
    public void Execute(int index) {
        //    Debug.Log("movetransforms injob " + index);
        position[index] += speed[index] * direction[index] * timeStep[0];

        //Debug.Log("movetransforms injob should check distance " + shouldCheckDistance[0]);

        if (shouldCheckDistance[0] == 1)
        {

            if (math.distance(position[index], destination[index]) < 2)
            {
                atDestination[index] = true;

            }

        }

    }
}
[BurstCompile]

public struct MoveTurrets : IJobParallelForTransform
{
    [NativeDisableParallelForRestriction]

    public NativeArray<TurretToMove> turret;

    public void Execute(int i, TransformAccess transformAccessArray) {
        int index;
        TurretToMove currentTurret;
        if (i % 2 == 0 || i == 0)
        {
            index = i / 2;
            currentTurret = turret[index];
            float3 direction = math.normalize(currentTurret.targetPosition - currentTurret.gunPosition);
            direction.y = 0;
            currentTurret.rotationSpeed *= currentTurret.deltaTime;
            transformAccessArray.rotation = math.slerp(transformAccessArray.rotation, quaternion.LookRotation(direction, new float3(0, 1, 0)) , currentTurret.rotationSpeed);
            if (currentTurret.shouldCheckDistance == 1)
            {
                float3 currentForward = transformAccessArray.rotation * new Vector3(0, 0, 1);
                if (math.distance(currentForward, direction) < 0.2f)
                {
                    currentTurret.canFire = 1;
                    turret[index] = currentTurret;
                }
                else
                {
                    currentTurret.canFire = 0;

                }
            }

        }
        else
        {
            index = (i - 1) / 2;
            currentTurret = turret[index];
            float3 direction = math.normalize(currentTurret.targetPosition - currentTurret.gunPosition);
            direction.x = 0;
            direction.z = 0;
            currentTurret.rotationSpeed *= currentTurret.deltaTime;
            transformAccessArray.rotation = math.slerp(transformAccessArray.rotation, quaternion.LookRotation(direction, new float3(0, 1, 0)), currentTurret.rotationSpeed);


        }



    }
}
[BurstCompile]

public struct MoveVehicles : IJobParallelForTransform
{
    public NativeArray<VehicleToMove> vehiclesToMove;
    public void Execute(int i, TransformAccess transformAccessArray) {


        VehicleToMove vehicle = new VehicleToMove();
     

        vehicle = vehiclesToMove[i];
       

        //vehicle.destination = new float3(vehicle.destination.x, 0, vehicle.destination.z);
       // UnityEngine.Debug.Log("Jobmanager MoveVeh struct got here ");

        float3 direction = math.normalize(vehicle.destination - (float3)transformAccessArray.position);
        float3 forward = math.forward(transformAccessArray.rotation);
        float distanceToTarget = math.distance(transformAccessArray.position, vehicle.destination);
        float directionDelta = math.distance(forward, direction);
      //  UnityEngine.Debug.Log("Jobmanager MoveVeh struct, pos current: " + transformAccessArray.position + " destination: " + vehicle.destination + " distance: " + distanceToTarget);
      //  UnityEngine.Debug.Log("Jobmanager MoveVeh struct, forward current " + forward + " target direction " + direction + "direction delta "  + directionDelta);
      //  UnityEngine.Debug.Log("Jobmanager MoveVeh struct, vehicle.velocity: " + vehicle.velocity);

        if (vehicle.velocity > vehicle.maxSpeed * vehicle.deltaTime)
        {
            vehicle.velocity = vehicle.maxSpeed * vehicle.deltaTime;


        }

        if (directionDelta < 0.6f)//if the vehicle is facting the correct direction add forward velocity, otherwise slow down
        {
            vehicle.velocity += 0.01f + vehicle.acceleration * vehicle.deltaTime;
        }
        else
        {
            vehicle.velocity *= 0.8f;

        }
        if (distanceToTarget < 2)// if vehicle is within 2m of target, it has arrived 
        {
            vehicle.atDestination = 1;
        }
        if (vehicle.finalDestination == 1)//if this is the final point, slow down in preperation to stop
        {
            if (distanceToTarget < 4)
            {
                vehicle.velocity *= 0.8f;
            }
     
   
        }
        transformAccessArray.rotation = math.slerp(transformAccessArray.rotation, quaternion.LookRotation(direction, new float3(0, 1, 0)), vehicle.turnSpeed * vehicle.deltaTime);

        transformAccessArray.position += (Vector3)forward * vehicle.velocity;


        //UnityEngine.Debug.Log("pos2 " + transformAccessArray.position);

        vehiclesToMove[i] = vehicle;
    }

}
[BurstCompile]

public struct CreateProjectile : IJobParallelForTransform
{
    [ReadOnly]
    public NativeArray<Projectile> projectile;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> distance;
    [NativeDisableParallelForRestriction]

    public NativeArray<float> time;


    public void Execute(int i, TransformAccess transformAccessArray) {

        if (i % 2 == 0)
        {
            int index = i / 2;


            float3 direction = projectile[index].endLocation - projectile[index].startLocation;
            if ((Vector3)direction == Vector3.zero)
            {
                time[index] = 0;
                return;
            }
            transformAccessArray.rotation = Quaternion.LookRotation(direction);
            float distance = math.distance(projectile[index].startLocation, projectile[index].endLocation);
            time[index] = (distance / projectile[index].speed);

        }
        else
        {
            int index = (i - 1) / 2;

            transformAccessArray.position = projectile[index].endLocation;
            //  UnityEngine.Debug.Log("transform hit " + i + " position 2 " + transformAccessArray.position);
            if (projectile[index].directionalHit)
            {
                float3 direction = projectile[index].endLocation - projectile[index].startLocation;
                transformAccessArray.rotation = quaternion.LookRotation(-direction, new Vector3(0, 1, 0));
            }

        }


    }
}
[BurstCompile]

public struct LookForTarget : IJobParallelFor
{
    [NativeDisableParallelForRestriction]

    public NativeArray<LookForTargetRequest> lookForTargetRequest;

    [NativeDisableParallelForRestriction]

    public NativeArray<LookForTargetUnitList> lookForTargetUnitList;


    public void Execute(int index) {
        // Debug.Log("look for target11");
        LookForTargetRequest thisRequest = lookForTargetRequest[index];
        float currentDistance;
        for (int i = 0; i < lookForTargetUnitList[0].totalNumberOfUnits; i++)
        {
            
            if (i < lookForTargetUnitList[lookForTargetRequest[index].faction].stopCounting || i > lookForTargetUnitList[lookForTargetRequest[index].faction].startCounting)
            {
                currentDistance = math.distance(lookForTargetRequest[index].unitPosition, lookForTargetUnitList[i].targetPosition);
                {
                    if (currentDistance  < thisRequest.distance)
                    {
                        if (lookForTargetUnitList[i].military ==1)
                        {
                            thisRequest.distance = (int)currentDistance;
                            thisRequest.closestMilitaryTarget = i;
                            if (thisRequest.distance < thisRequest.range)
                            {
                                lookForTargetRequest[index] = thisRequest;
                                return;
                            }
                        }
                        else
                        {
                            thisRequest.closestNonMilitaryTarget = i;
                        }
                    }
                }
            }
        }
    thisRequest.closestMilitaryTarget = 10000;
        lookForTargetRequest[index] = thisRequest;
        /* if (minDistance[index] > range[index])
         {
             if (math.distance(unitposition[index], targetPosition[closestNonMilitaryTarget[index]]) < range[index])
             {
                 closestMilitaryTarget[index] = closestNonMilitaryTarget[index];
             }
             else
             {
                 closestMilitaryTarget[index] = 1000;
             }
         }*/

        // Debug.Log("look for target12");
    }
}
public struct Projectile
{
    public Vector3 startLocation;
    public Vector3 endLocation;
    public int aoe;
    public int damage;
    public float time;
    public float speed;
    public bool directionalHit;
}
public struct ProjectileHit
{
    public float time;
    public float3 location;
    public ParticleSystem animation;
    public int aoe;
    public int damage;
    public UnitBehaviour target;
}
public struct VehicleToMove
{
    public float3 destination;
    public float velocity;
    public int finalDestination;
    public float maxSpeed;
    public float turnSpeed;
    public float acceleration;
    public int shouldMove;
    public int shouldCheckDistance;
    public int atDestination;
    public float deltaTime;
}
public struct TurretToMove
{
    public float3 targetPosition;
    public float3 gunPosition;
    public float deltaTime;
    public float rotationSpeed;
    public int canFire;
    public int shouldMove;
    public int shouldCheckDistance;
}
public struct LookForTargetRequest
{
    public int faction;
    public int military;
    public int range;
    public float2 unitPosition;
    public int closestMilitaryTarget;
    public int closestNonMilitaryTarget;
    public int distance;
    public int shouldCheckDistance;
}
public struct LookForTargetUnitList
{
    public float2 targetPosition;
    public int military;
    public int startCounting;
    public int stopCounting;
    public int totalNumberOfUnits;
}

