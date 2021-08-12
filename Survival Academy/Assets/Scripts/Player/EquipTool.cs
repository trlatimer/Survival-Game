using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;
    public float attackDistance;

    [Header("Resource Gathering")]
    public bool doesGatherResources;

    [Header("Combat")]
    public bool doesDealDamage;
    public int damage;

    private bool attacking;

    // Components
    private Animator anim;
    private Camera cam;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        cam = Camera.main;
    }

    public override void OnAttackInput()
    {
        if (!attacking)
        {
            attacking = true;
            anim.SetTrigger("Attack");
            Invoke("OnCanAttack", attackRate);
        }
    }

    public void OnHit()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance))
        {
            if (doesGatherResources && hit.collider.GetComponent<Resource>()) // Gatherable
            {
                hit.collider.GetComponent<Resource>().Gather(hit.point, hit.normal);
            }
            if (doesDealDamage && hit.collider.GetComponent<IDamagable>() != null) // Damagable
            {
                hit.collider.GetComponent<IDamagable>().TakePhysicalDamage(damage);
            }
        }
    }

    private void OnCanAttack()
    {
        attacking = false;
    }
}
