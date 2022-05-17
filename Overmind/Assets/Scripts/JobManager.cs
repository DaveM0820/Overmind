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
    Vector2[] targets = new Vector2[400];
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
    float3[] gunForward_moveTurret = new float3[400];
    float3[] turretForward_moveTurret = new float3[400];
    float[] rotationspeed_moveTurret = new float[400];
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
    MoveVehicles moveVehicles;

    int totalVehicles;
    int vehiclesToMove = 0;

    Transform[] vehicles_moveVehicle = new Transform[400];
    bool[] shouldMove_moveVehicle = new bool[400];
    float3[] unitposition_moveVehicle = new float3[400];
    Vector3[] destination_moveVehicle = new Vector3[400];
    float3[] unitforward_moveVehicle = new float3[400];
    float[] unitVelocity_moveVehicle = new float[400];
    float[] unitMaxSpeed_moveVehicle = new float[400];
    float[] unitTurnSpeed_moveVehicle = new float[400];
    float[] deltaTime_moveVehicle = new float[400];
    float[] acceleration_moveVehicle = new float[400];
    float[] dampening_moveVehicle = new float[400];
    bool[] atDestination_moveVehicle = new bool[400];



    int shouldCheckDistanceCounter = 0;
    bool shouldCheckDistance = false;
    float elapsed;
    float updateTimestep;
    float halfTimestep;



    //   JobHandle createParticlesJobHAndle = new JobHandle();
    JobHandle createProjectilesJobHandle;

    Projectile[] projectiles_createProjectile = new Projectile[200];
    ParticleSystem[] shots_createProjectile = new ParticleSystem[200];
    ParticleSystem[] hits_createProjectile = new ParticleSystem[200];
    UnitBehaviour[] targets_createProjectile = new UnitBehaviour[200];
    int projectilesToCreate = 0;
    int nextProjectileHitTime = 0;
    List<ProjectileHit> projectileHitList = new List<ProjectileHit>();
    // NativeArray<ParticleSystem.Particle> particleArray_createParticles = new NativeArray<ParticleSystem.Particle>(5000, Allocator.Persistent);

    // EntityManager entityManager;
    void Awake() {
        gameInformation = gameObject.GetComponent<GlobalGameInformation>();
        numberOfPlayers = gameInformation.numberOfPlayers;
        updateTimestep = gameInformation.updateTimestep;
        halfTimestep = updateTimestep / 2;
        //  entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        // Entity entity = entityManager.CreateEntity();





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
        rotationspeed_moveTurret[totalTurrets] = rotateSpeed;
        shouldFire_moveTurret[totalTurrets] = false;
        unit.GetComponent<UnitBehaviour>().turretNumber = totalTurrets;
        totalTurrets++;
    }
    public void MoveTurret(int index, Transform target) {
        if (!shouldMove_moveTurret[index])
        {
            turretsToMove++;

            shouldMove_moveTurret[index] = true;

        }
        // UnityEngine.Debug.Log("moveturret " + totalTurrets);

        if (target != target_moveTurret[index])
        {
            target_moveTurret[index] = target;
        }
        //    UnityEngine.Debug.Log("should move turret of index" + index + " total turrets to move " + turretsToMove);

    }
    public void StopMovingTurret(int index) {
        if (shouldMove_moveTurret[index])
        {
            shouldMove_moveTurret[index] = false;
            turretsToMove--;
        }
    }
    public void AddVehicleToMove(Transform vehicleTransform, float maxSpeed, float turnSpeed, float acceleration, float dampening) {
        vehicles_moveVehicle[totalVehicles] = vehicleTransform;
        unitMaxSpeed_moveVehicle[totalVehicles] = maxSpeed;
        unitTurnSpeed_moveVehicle[totalVehicles] = turnSpeed;
        acceleration_moveVehicle[totalVehicles] = acceleration;
        dampening_moveVehicle[totalVehicles] = dampening;
        shouldMove_moveVehicle[totalVehicles] = false;
        vehicleTransform.gameObject.GetComponent<UnitBehaviour>().vehicleNumber = totalVehicles;

        totalVehicles++;
    }
    public void MoveVehicle(int vehicleNumber, Vector3 destination) {
        if (!shouldMove_moveVehicle[vehicleNumber])
        {
            vehiclesToMove++;

            shouldMove_moveVehicle[vehicleNumber] = true;

        }

        if (destination != destination_moveVehicle[vehicleNumber])
        {
            destination_moveVehicle[vehicleNumber] = destination;
        }
    }
    public void StopMovingVehicle(int vehicleNumber) {
        if (shouldMove_moveVehicle[vehicleNumber])
        {
            shouldMove_moveVehicle[vehicleNumber] = false;
            vehiclesToMove--;
        }

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
            if (numberOfUnitsToLookForTarget > 0)
            {
                performanceTimer.Restart();
                int lookfortargetnum = numberOfUnitsToLookForTarget;
                LookForTargets();
                if (performanceTimer.Elapsed.TotalMilliseconds > 0.2f)
                {

                    //  UnityEngine.Debug.Log("PerformanceTimer, LookForTargets  " + performanceTimer.Elapsed.TotalMilliseconds + " how many? " + lookfortargetnum);
                }
            }
            elapsed++;

        }

        if (elapsed == 1) //run the order every updateTimestep using the UpdateUnit method
        {

            if (vehiclesToMove > 0)
            {
                performanceTimer.Restart();

                MoveVehicles();
                if (performanceTimer.Elapsed.TotalMilliseconds > 0.2f)
                {

                    //     UnityEngine.Debug.Log("PerformanceTimer, MoveVehicles  " + performanceTimer.Elapsed.TotalMilliseconds + " how many? " + vehiclesToMove);
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

            elapsed += 1;

        }
        if (elapsed == 2)
        {

            if (turretsToMove > 0)
            {

                performanceTimer.Restart();
                MoveTurrets();
                if (performanceTimer.Elapsed.TotalMilliseconds > 0.2f)
                {

                    //  UnityEngine.Debug.Log("PerformanceTimer, MoveTurrets  " + performanceTimer.Elapsed.TotalMilliseconds + " how many? " + turretsToMove);
                }
            }
            elapsed = 0;
        }

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

            targets = new Vector2[400];
            military = new bool[400];

            numberOfTargetsPerFaction = new int[numberOfPlayers];
            totalNumberOfTargets = 0;
            for (int currentfaction = 0; currentfaction < numberOfPlayers; currentfaction++)
            {
                foreach (Unit unitToCheck in gameInformation.unitList[currentfaction])
                {
                    targets[totalNumberOfTargets] = new Vector2(unitToCheck.gameObject.transform.position.x, unitToCheck.gameObject.transform.position.z);
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
    void ProjectileHit(ProjectileHit projectileHit) {
        projectileHit.animation.Play();
        if (projectileHit.aoe == 0)
        {
            if (projectileHit.target != null)
            {
            projectileHit.target.changeHP(-projectileHit.damage);
            }
        }
        else
        {
        //schedule spherecast
        }
        projectileHitList.Remove(projectileHit);
       // projectileHitList.RemoveAll(ProjectileHit => ProjectileHit.Contains(projectileHit));

    }
    void CreateProjectiles() {
        foreach (ProjectileHit projectileHit in projectileHitList.ToArray())
        {
            if (Time.time > projectileHit.time)
            {
                ProjectileHit(projectileHit);
            }
        
        }



        NativeArray<Projectile> projectiles_projectilesToCreate_Native = new NativeArray<Projectile>(projectilesToCreate, Allocator.TempJob);
        TransformAccessArray projectileTransforms_projectilesToCreate_Native = new TransformAccessArray(projectilesToCreate * 2);
        NativeArray<float> distances_projectilesToCreate_Native = new NativeArray<float>(projectilesToCreate, Allocator.TempJob);
        NativeArray<float> time_projectilesToCreate_Native = new NativeArray<float>(projectilesToCreate, Allocator.TempJob);

        for (int i = 0; i < projectilesToCreate; i++)
        {

            projectiles_projectilesToCreate_Native[i] = projectiles_createProjectile[i];
            projectileTransforms_projectilesToCreate_Native.Add(shots_createProjectile[i].gameObject.transform);
            projectileTransforms_projectilesToCreate_Native.Add(hits_createProjectile[i].gameObject.transform);
        }

        CreateProjectile createProjectile = new CreateProjectile() {
            projectile = projectiles_projectilesToCreate_Native,
            distance = distances_projectilesToCreate_Native,
            time = time_projectilesToCreate_Native,
        };
        createProjectilesJobHandle = createProjectile.Schedule(projectileTransforms_projectilesToCreate_Native);
        createProjectilesJobHandle.Complete();

        for (int i = 0; i < projectilesToCreate; i++)
        {
            var main = shots_createProjectile[i].main;
            main.startLifetime = createProjectile.time[i];
            shots_createProjectile[i].Play();

            projectileHitList.Add(new ProjectileHit { time = createProjectile.time[i] + Time.time, animation = hits_createProjectile[i], aoe = projectiles_createProjectile[i].aoe, damage = projectiles_createProjectile[i].damage, target = targets_createProjectile[i] });
        }
        projectilesToCreate = 0;
        time_projectilesToCreate_Native.Dispose();
        distances_projectilesToCreate_Native.Dispose();
        projectiles_projectilesToCreate_Native.Dispose();
        projectileTransforms_projectilesToCreate_Native.Dispose();
    }

    void MoveTurrets() {


        // UnityEngine.Debug.Log("moveturrets got here1 turrets to move" + turretsToMove);
        NativeArray<float3> turretPosition_moveTurret_Native = new NativeArray<float3>(turretsToMove, Allocator.TempJob);
        NativeArray<float3> targetPosition_moveTurret_Native = new NativeArray<float3>(turretsToMove, Allocator.TempJob);
        NativeArray<quaternion> gunRotation_moveTurret_Native = new NativeArray<quaternion>(turretsToMove, Allocator.TempJob);
        NativeArray<quaternion> turretRotation_moveTurret_Native = new NativeArray<quaternion>(turretsToMove, Allocator.TempJob);
        NativeArray<float> deltaTime_moveTurret_Native = new NativeArray<float>(turretsToMove, Allocator.TempJob);
        NativeArray<float> rotationSpeed_moveTurret_Native = new NativeArray<float>(turretsToMove, Allocator.TempJob);
        NativeArray<bool> canFire_moveTurret_Native = new NativeArray<bool>(turretsToMove, Allocator.TempJob);
        NativeArray<bool> shouldCheckDistance_moveTurret_Native = new NativeArray<bool>(1, Allocator.TempJob);

        shouldCheckDistance_moveTurret_Native[0] = shouldCheckDistance;
        // UnityEngine.Debug.Log("moveturrets got here2");

        int turretsToMoveCounter = 0;
        for (int i = 0; i < totalTurrets; i++)
        {
            if (shouldMove_moveTurret[i])
            {
                if (unit_moveTurret[i] != null)
                {

                    if (target_moveTurret[i] != null)
                    {
                        turretPosition_moveTurret_Native[turretsToMoveCounter] = turretsToMove_moveTurret[i].position;
                        targetPosition_moveTurret_Native[turretsToMoveCounter] = target_moveTurret[i].position;
                        gunRotation_moveTurret_Native[turretsToMoveCounter] = gunToMove_moveTurret[i].rotation;
                        turretRotation_moveTurret_Native[turretsToMoveCounter] = turretsToMove_moveTurret[i].rotation;
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
            gunRotation = gunRotation_moveTurret_Native,
            turretRotation = turretRotation_moveTurret_Native,
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

                turretsToMove_moveTurret[i].rotation = moveTurrets.turretRotation[turretsToMoveCounter];
                gunToMove_moveTurret[i].rotation = moveTurrets.gunRotation[turretsToMoveCounter];

                if (moveTurrets.canfire[turretsToMoveCounter])
                {
                    unit_moveTurret[i].GetComponent<UnitBehaviour>().canFire = true;
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
        shouldCheckDistance_moveTurret_Native.Dispose();
        turretPosition_moveTurret_Native.Dispose();
        targetPosition_moveTurret_Native.Dispose();
        gunRotation_moveTurret_Native.Dispose();
        turretRotation_moveTurret_Native.Dispose();
        deltaTime_moveTurret_Native.Dispose();
        rotationSpeed_moveTurret_Native.Dispose();
        canFire_moveTurret_Native.Dispose();


    }
    void MoveVehicles() {


        // UnityEngine.Debug.Log("moveturrets got here1 turrets to move" + turretsToMove);
        NativeArray<bool> shouldMove_moveVehicle_Native = new NativeArray<bool>(vehiclesToMove, Allocator.TempJob);
        NativeArray<float3> unitposition_moveVehicle_Native = new NativeArray<float3>(vehiclesToMove, Allocator.TempJob);
        NativeArray<float3> destination_moveVehicle_Native = new NativeArray<float3>(vehiclesToMove, Allocator.TempJob);
        NativeArray<quaternion> unitforward_moveVehicle_Native = new NativeArray<quaternion>(vehiclesToMove, Allocator.TempJob);
        NativeArray<float> unitVelocity_moveVehicle_Native = new NativeArray<float>(vehiclesToMove, Allocator.TempJob);
        NativeArray<float> unitMaxSpeed_moveVehicle_Native = new NativeArray<float>(vehiclesToMove, Allocator.TempJob);
        NativeArray<float> unitTurnSpeed_moveVehicle_Native = new NativeArray<float>(vehiclesToMove, Allocator.TempJob);
        NativeArray<float> deltaTime_moveVehicle_Native = new NativeArray<float>(vehiclesToMove, Allocator.TempJob);
        NativeArray<float> acceleration_moveVehicle_Native = new NativeArray<float>(vehiclesToMove, Allocator.TempJob);
        NativeArray<bool> atDestination_moveVehicle_Native = new NativeArray<bool>(vehiclesToMove, Allocator.TempJob);
        NativeArray<bool> shouldCheckDistance_moveVehicle_Native = new NativeArray<bool>(1, Allocator.TempJob);

        shouldCheckDistance_moveVehicle_Native[0] = shouldCheckDistance;
        // UnityEngine.Debug.Log("moveturrets got here2");

        int vehiclesToMoveCounter = 0;
        for (int i = 0; i < totalVehicles; i++)
        {
            if (shouldMove_moveVehicle[i])
            {
                if (vehicles_moveVehicle[i].gameObject.activeSelf)
                {
                    unitposition_moveVehicle_Native[vehiclesToMoveCounter] = vehicles_moveVehicle[i].position;
                    destination_moveVehicle_Native[vehiclesToMoveCounter] = destination_moveVehicle[i];
                    unitforward_moveVehicle_Native[vehiclesToMoveCounter] = vehicles_moveVehicle[i].rotation;
                    unitVelocity_moveVehicle_Native[vehiclesToMoveCounter] = unitVelocity_moveVehicle[i];
                    unitMaxSpeed_moveVehicle_Native[vehiclesToMoveCounter] = unitMaxSpeed_moveVehicle[i];
                    unitTurnSpeed_moveVehicle_Native[vehiclesToMoveCounter] = unitTurnSpeed_moveVehicle[i];
                    deltaTime_moveVehicle_Native[vehiclesToMoveCounter] = updateTimestep;
                    acceleration_moveVehicle_Native[vehiclesToMoveCounter] = acceleration_moveVehicle[i];
                    atDestination_moveVehicle_Native[vehiclesToMoveCounter] = false;
                    vehiclesToMoveCounter++;
                }
                else
                {
                    shouldMove_moveVehicle[i] = false;
                }

            }

        }
        //UnityEngine.Debug.Log("moveturrets got here3");

        moveVehicles = new MoveVehicles {

            unitposition = unitposition_moveVehicle_Native,
            targetposition = destination_moveVehicle_Native,
            unitforward = unitforward_moveVehicle_Native,
            unitVelocity = unitVelocity_moveVehicle_Native,
            unitMaxSpeed = unitMaxSpeed_moveVehicle_Native,
            unitTurnSpeed = unitTurnSpeed_moveVehicle_Native,
            deltaTime = deltaTime_moveVehicle_Native,
            acceleration = acceleration_moveVehicle_Native,
            atTarget = atDestination_moveVehicle_Native,


        };



        moveVehiclesJobHandle = moveVehicles.Schedule(vehiclesToMove, 4);
        moveVehiclesJobHandle.Complete();
        //  Debug.Log("moveTurretJobHandle is done");

        vehiclesToMoveCounter = 0;
        for (int i = 0; i < totalVehicles; i++)
        {
            if (shouldMove_moveVehicle[i])
            {
                vehicles_moveVehicle[i].position = moveVehicles.unitposition[vehiclesToMoveCounter];
                vehicles_moveVehicle[i].rotation = moveVehicles.unitforward[vehiclesToMoveCounter];
                unitVelocity_moveVehicle[i] = moveVehicles.unitVelocity[vehiclesToMoveCounter];
                if (moveVehicles.atTarget[vehiclesToMoveCounter])
                {
                }
                vehiclesToMoveCounter++;
                if (vehiclesToMoveCounter > vehiclesToMove)
                {
                    break;
                }
            }
        }
        //  Debug.Log("gave movement orders to turrets");
        // UnityEngine.Debug.Log("moveturrets got here5");

        shouldMove_moveVehicle_Native.Dispose();
        unitposition_moveVehicle_Native.Dispose();
        destination_moveVehicle_Native.Dispose();
        unitforward_moveVehicle_Native.Dispose();
        unitVelocity_moveVehicle_Native.Dispose();
        unitMaxSpeed_moveVehicle_Native.Dispose();
        unitTurnSpeed_moveVehicle_Native.Dispose();
        deltaTime_moveVehicle_Native.Dispose();
        acceleration_moveVehicle_Native.Dispose();
        atDestination_moveVehicle_Native.Dispose();
        shouldCheckDistance_moveVehicle_Native.Dispose();

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


        NativeArray<int> faction_LookforTarget_Native = new NativeArray<int>(numberOfUnitsToLookForTarget, Allocator.TempJob);
        NativeArray<bool> isMilitary_LookforTarget_Native = new NativeArray<bool>(totalNumberOfTargets, Allocator.TempJob);
        NativeArray<float2> unitPosition_LookforTarget_Native = new NativeArray<float2>(numberOfUnitsToLookForTarget, Allocator.TempJob);
        NativeArray<float2> targetPositions_LookforTarget_Native = new NativeArray<float2>(totalNumberOfTargets, Allocator.TempJob);
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



            unitPosition_LookforTarget_Native[i] = new Vector2(unitsToLookForTarget[i].gameObject.transform.position.x, unitsToLookForTarget[i].gameObject.transform.position.z);

            //   UnityEngine.Debug.Log( unitsToLookForTarget[i].gameObject.name + " looking");

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



        lookForTargetJobHandle = lookForTarget.Schedule(numberOfUnitsToLookForTarget, 2);
        lookForTargetJobHandle.Complete();


        for (int i = 0; i < numberOfUnitsToLookForTarget - 1; i++)
        {

            if (closestMilitaryTarget_LookforTarget_Native[i] != 1000 && unitsToLookForTarget[i].gameObject != null)
            {

                //  unitsToLookForTarget[i].gameObject.GetComponent<UnitBehaviour>().Attack(currentTarget);

                unitsToLookForTarget[i].gameObject.GetComponent<UnitBehaviour>().Attack(target[lookForTarget.closestMilitaryTarget[i]].gameObject);
                // Debug.Log("FINAL ASSIGNMENT unit " + unitsToLookForTarget[i].gameObject.name + " is targeting " + target[lookForTarget.closestMilitaryTarget[i]].gameObject);


                // Debug.Log("FINAL ASSIGNMENT unit " + unitsToLookForTarget[i].gameObject.name + " is targeting " + target[lookForTarget.closestMilitaryTarget[i]].gameObject);


            }
            else
            {
                 unitsToLookForTarget[i].gameObject.GetComponent<UnitBehaviour>().TargetGone();
            }
            //   Debug.Log(unitsToLookForTarget[i].gameObject.name + " found target with unit number " + lookForTarget.closestTarget[i]);
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
    [ReadOnly]
    public NativeArray<float3> destination;
    [ReadOnly]
    public NativeArray<float> speed;
    [ReadOnly]
    public NativeArray<float3> direction;
    [ReadOnly]
    public NativeArray<float> timeStep;
    [ReadOnly]
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

public struct MoveTurrets : IJobParallelForBurstSchedulable
{
    [ReadOnly]
    public NativeArray<float3> unitposition;
    [ReadOnly]
    public NativeArray<float3> targetposition;
    public NativeArray<quaternion> gunRotation;
    public NativeArray<quaternion> turretRotation;
    [ReadOnly]

    public NativeArray<float> deltaTime;


    public NativeArray<float> rotationspeed;
    public NativeArray<bool> canfire;
    [ReadOnly]
    public NativeArray<bool> checkDistance;
    public void Execute(int index) {

        rotationspeed[index] *= deltaTime[index];
        Vector3 turretForward = math.forward(turretRotation[index]);
        Vector3 gunForward = math.forward(gunRotation[index]);

        float3 direction = math.normalize(targetposition[index] - unitposition[index]);


        turretForward = math.lerp(turretForward, new float3(direction.x, 0, direction.z), rotationspeed[index]);
        gunForward = math.lerp(gunForward, new float3(0, direction.y, 0), rotationspeed[index]);
        gunForward = new float3(turretForward.x, gunForward.y, turretForward.z);
        turretRotation[index] = quaternion.LookRotation(turretForward, new float3(0, 1, 0));
        gunRotation[index] = quaternion.LookRotation(gunForward, new float3(0, 1, 0));

        if (checkDistance[0])
        {
            if (math.distance(turretForward, direction) < 0.2f)
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
[BurstCompile]

public struct MoveVehicles : IJobParallelFor
{
    public NativeArray<float3> unitposition;
    public NativeArray<float3> targetposition;
    public NativeArray<quaternion> unitforward;
    public NativeArray<float> unitVelocity;
    public NativeArray<float> unitMaxSpeed;
    public NativeArray<float> unitTurnSpeed;
    public NativeArray<float> deltaTime;
    public NativeArray<float> acceleration;
    public NativeArray<bool> atTarget;


    public void Execute(int index) {
        unitTurnSpeed[index] *= deltaTime[index];
        unitMaxSpeed[index] *= deltaTime[index];
        acceleration[index] *= deltaTime[index];

        targetposition[index] = new float3(targetposition[index].x, 0, targetposition[index].z);
        unitposition[index] = new float3(unitposition[index].x, 0, unitposition[index].z);

        float3 direction = math.normalize(targetposition[index] - unitposition[index]);
        float3 forward = math.forward(unitforward[index]);
        unitforward[index] = math.slerp(unitforward[index], quaternion.LookRotation(direction, new float3(0, 1, 0)), unitTurnSpeed[index]);


        float distanceToTarget = math.distance(unitposition[index], targetposition[index]);
        if (distanceToTarget < 5)// if the vehicle is facing the target
        {
            unitVelocity[index] *= 0.7f;
        }
        else
        {
            if (unitVelocity[index] < unitMaxSpeed[index])
            {
                unitVelocity[index] += acceleration[index];
            }

        }
        if (distanceToTarget < 1)// if the vehicle is facing the target
        {
            unitVelocity[index] *= 0.2f;
            atTarget[index] = true;
        }
        float directionDistance = math.distance(forward, direction);
        if (directionDistance < 0.8f)
        {

            unitposition[index] += unitVelocity[index] * forward;
        }
        else if (directionDistance > 1.5f)
        {
            unitposition[index] -= unitVelocity[index] * forward;

        }
        else
        {
            unitVelocity[index] = 0;
        }
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
            //     UnityEngine.Debug.Log("transform shot " + i + " position " + transformAccessArray.position);
            // UnityEngine.Debug.Log("transform shot " + i + " rotation " + transformAccessArray.rotation);
            int index = i / 2;

            //transformAccessArray.position = projectile[i / 2].startLocation;
     
            float3 direction = projectile[index].endLocation - projectile[index].startLocation;
            transformAccessArray.rotation = Quaternion.LookRotation(direction);
                float distance = math.distance(projectile[index].startLocation, projectile[index].endLocation);
                time[index] = (distance / projectile[index].speed);
            //    UnityEngine.Debug.Log("transform shot " + i + " rotation 2 " + transformAccessArray.rotation);

            }
            else
            {
            //  UnityEngine.Debug.Log("transform hit " + i + " position " + transformAccessArray.position);
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

    [ReadOnly]
    public NativeArray<int> faction;
    [ReadOnly]
    public NativeArray<float2> unitposition;
    [ReadOnly]
    public NativeArray<bool> isMilitary;
    [ReadOnly]
    public NativeArray<float2> targetpositions;
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
                            if (currentDistance[index] < range[index])
                            {
                                return;
                            }
                        }
                        else
                        {
                            closestNonMilitaryTarget[index] = i;
                        }
                    }
                }
            }
        }
        /* if (minDistance[index] > range[index])
         {
             if (math.distance(unitposition[index], targetpositions[closestNonMilitaryTarget[index]]) < range[index])
             {
                 closestMilitaryTarget[index] = closestNonMilitaryTarget[index];
             }
             else
             {
                 closestMilitaryTarget[index] = 1000;
             }
         }*/
        closestMilitaryTarget[index] = 1000;
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
    public ParticleSystem animation;
    public int aoe;
    public int damage;
    public UnitBehaviour target;
}

