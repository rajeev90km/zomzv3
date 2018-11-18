﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieExplode : ZombieBase 
{
    [SerializeField]
    private GameObject _explosionFXPrefab;

    private int _enemyLayerMask;
    private int _playerLayerMask;
    private int _enemyAndPlayerLayerMask;
    private int flammableMask;

    GameObject _explosionFXObj;

    ZomzController _zomzController;

	protected override void Awake()
	{
        base.Awake();

        flammableMask = (1 << LayerMask.NameToLayer("Flammable"));

        _zomzController = GameObject.FindWithTag("Player").GetComponent<ZomzController>();
	}

	public override void OnZomzModeUnRegister()
	{
        base.OnZomzModeUnRegister();
        IsHurting = false;
        IsAttacking = false;
	}


    public override IEnumerator Hurt(float pDamage = 0)
	{
        IsHurting = true;
        IsAttacking = false;

        if(_attackCoroutine==null)
        {
            _animator.SetTrigger("attack");
            StartCoroutine(Attack());
        }

        yield return null;
	}


	public override IEnumerator Attack()
    {
        if (IsAlive && !IsAttacking)
        {
            if (IsBeingControlled)
                _animator.SetTrigger("attack");

            IsAttacking = true;

            finalLayerMask = humanLayerMask | playerLayerMask | zombieLayerMask;


            AkSoundEngine.PostEvent("Exp_Attack", gameObject);

            yield return new WaitForSeconds(CharacterStats.AttackRate);

            Collider[] beingsHit = Physics.OverlapSphere(transform.position, CharacterStats.ExplosionRange, finalLayerMask);

            for (int i = 0; i < beingsHit.Length; i++)
            {
                Being being = beingsHit[i].GetComponent<Being>();

                if (being != null && being.transform != transform && being.IsAlive)
                {
                    float d = Vector3.Distance(being.transform.position, transform.position);

                    if (d <= CharacterStats.ExplosionRange)
                    {
                        being.StartCoroutine(being.Hurt( (CharacterStats.ExplosionRange - d )/CharacterStats.ExplosionRange * CharacterStats.AttackStrength));
                    }
                }
            }


            Collider[] otherFlammableObjects = Physics.OverlapSphere(transform.position, CharacterStats.ExplosionRange, flammableMask);

            for (int i = 0; i < otherFlammableObjects.Length; i++)
            {
                Explosive explosive = otherFlammableObjects[i].GetComponent<Explosive>();

                if(explosive)
                {
                    explosive.Explode();
                }


                Flammable flammable = otherFlammableObjects[i].GetComponent<Flammable>();

                if(flammable)
                {
                    flammable.OnCombustion();
                }

            }

            IsAttacking = false;


            if(_explosionFXPrefab!=null)
            {
                _explosionFXObj = Instantiate(_explosionFXPrefab);
                _explosionFXObj.transform.position = transform.position;
                IsAlive = false;
            }

            

            yield return new WaitForSeconds(0.3f);

            if (!IsHurting)
                _zomzController.UnregisterZomzMode();

            if (_zomzDieEvent)
                _zomzDieEvent.Raise();

            Destroy(gameObject);

        }

        yield return null;
    }
}
