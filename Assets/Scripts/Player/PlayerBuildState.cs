using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using MyUtils.Graph;
using MyUtils.StateMachine;


public class PlayerBuildState : PlayerBaseState
{
    private TowerData _towerData;
    private float _castCooldown;
    private float _swapCooldown;

    private int _towerIndex;
    private LayerMask unallowedTerrain;

    private GameObject _towerMesh;

    // Start is called before the first frame update
    public PlayerBuildState(ThirdPersonController owner) : base(owner)
    {
        _towerData = owner.GetComponent<TowerData>();
        _towerIndex = 0;
    }

    public override void Enter(ThirdPersonController owner, ArrayList data = null){
        _towerMesh = ThirdPersonController.Instantiate(_towerData.Meshes[_towerIndex]);

        ThirdPersonController.Destroy(owner._currentWeapon);
        

        if (owner._currentOffHand)
        {
            ThirdPersonController.Destroy(owner._currentOffHand);
        }
        else 
        {
            ThirdPersonController.print("No offhand object to destroy");
        }

        owner._currentWeapon = ThirdPersonController.Instantiate(owner.weapons[1], owner.rightHandObject.transform);
        owner._currentOffHand = null;
        if (owner._hasAnimator)
        {
            owner._animator.SetBool("Combat", false);
            owner._animator.SetBool("Build", true);
        }

        _swapCooldown = 1f;
    }

    // Update is called once per frame
    public override void Update(ThirdPersonController owner)
    {
        base.Update(owner);
        if ((_input.build && _swapCooldown <= 0) || (_castCooldown <= 0 && !_towerMesh))
        {
            swapCombat();
        }
        
        if (_towerMesh)
        {
            _towerMesh.transform.position = owner.transform.position + owner.transform.forward * 3.5f;
        }

        if (_input.attack && _castCooldown <= 0)
        {
            if (CanBuildTower(_towerMesh.transform.position, owner.stats.Money, _towerData.Costs[_towerIndex]))
            {
                owner.stats.Money -= _towerData.Costs[_towerIndex];
                ThirdPersonController.Instantiate(_towerData.Towers[_towerIndex], _towerMesh.transform.position, Quaternion.identity);
                ThirdPersonController.Destroy(_towerMesh);
                _castCooldown = 1f;
            }
            else 
            {
                // Play Error Sound
            }
        }

        if (_castCooldown > 0)
        {
            _castCooldown -= Time.deltaTime;
        } 
        if (_swapCooldown > 0)
        {
            _swapCooldown -= Time.deltaTime;
        }
        
    }

    public override void Exit(ThirdPersonController owner)
    {
        
    }

    void swapCombat()
    {
        if (_towerMesh)
        {
            ThirdPersonController.Destroy(_towerMesh);
        }
        player.stateStack.ChangeState(player.CombatState);
    }

    // Ensure the new tower doesn't collide with an Enemy, Tower, Obstacle, or Path tile
    // And that we can afford the tower
    bool CanBuildTower(Vector3 position, int money, int cost)
    {
        bool validPlace = !Physics.CheckSphere(position, 2.5f, _towerData.unallowedTerrain);
        bool validPrice = money >= cost;
        return validPlace && validPrice;
    }
}
