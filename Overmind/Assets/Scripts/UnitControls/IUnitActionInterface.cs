using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitActionInterface
{
    void Move(Vector3 movetarget);
    void Build(GameObject building);
    void Attack(GameObject target);

    void Stop();
    void ExtractOre();
    void UpdateScaffold();
    void EnterDirectControl();
    void ExitDirectControl();
    void UnderDirectControl();
    void Die();

    void Damage();
    void CollisionEnter(Collider collision);
    void CollisionExit(Collider collision);
    void CollisionStay(Collider collision);

}
