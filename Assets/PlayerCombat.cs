using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class PlayerCombat : MonoBehaviour
{
    ApplesPlayer player;
    [SerializeField] float attackAngle = 180;
    [SerializeField] float attackRange = 4;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackCooldown = 1;
    [SerializeField] bool offCooldown = true;  //if this is true, the player can attack
    [SerializeField, ReadOnly] bool facingRight;
    BoxCollider2D hitBox;

    //the player turns invincible for a bit so they dont take damage
    [SerializeField, ReadOnly] bool invincible = false;
    [SerializeField] float invincibleTime;  //how long the player stays invincible
    [SerializeField] float flashingSpeed;  //how fast the player flashes when hes invincible

    //Player Max and current health
    [SerializeField] int MaxHealth;
    [SerializeField] int currentHealth;

    //how far and how fast the player gets knockedback
    [SerializeField, Range(0f, 5.0f)] float knockbackDistance;
    [SerializeField, Range(0f, 2.0f)] float knockbackSpeed;

    //how far attack knocks back zombies;
    [SerializeField] float attackKnockback;

    //how long injection should take
    [SerializeField] float InjectionTime;
    [SerializeField] bool injecting = false;


    [SerializeField] Transform cam;
    [SerializeField] Transform slash;

    Rigidbody _rb;
    private void Awake()
    {
        player = GetComponent<ApplesPlayer>();

        //grabbing the players rigidbody component
        _rb = GetComponent<Rigidbody>();

        //set players current health to max health
        currentHealth = MaxHealth;
    }

    private void Update()
    {
        //condition for it input == 0 intentionally left out
        if (Input.GetAxis("Horizontal") > 0) facingRight = true;
        else if (Input.GetAxis("Horizontal") < 0) facingRight = false;
        if (Input.GetKeyDown(KeyCode.Space)) AttackDamage();
        if (Input.GetKeyDown(KeyCode.C)) AttackConvert();

        GetComponent<Renderer>().material.SetFloat("Invincible", invincible ? 1 : 0);
    }

    //this attack attempts to damage the zombie rather than cure
    public void AttackDamage()
    {
        if (!offCooldown || injecting) return;  //if the players attack is on cooldown, cancel attack

        //turns on the slash graphical effect
        slash.GetChild(0).GetComponent<Renderer>().material.color = Color.white;
        slash.eulerAngles = new Vector3(0, cam.eulerAngles.y, 0);
        slash.localScale = Vector3.Scale(((facingRight ? 1 : -1) * new Vector3(1, 0, 1)), slash.transform.localScale.Abs()) + new Vector3(0,1,0);
        slash.gameObject.SetActive(true);
        //turns off the slash effect after a set amount of time
        Timer.SimpleTimer(() => slash.gameObject.SetActive(false), .2f);

        //Sets the attack to be on cooldown
        offCooldown = false;

        Timer.SimpleTimer(() => offCooldown = true, attackCooldown);//after attackCooldown seconds, the player can attack again

        //NOW the attack checks for enemies
        Collider[] enemies =  Physics.OverlapSphere(transform.position, attackRange).Where(col => col.gameObject.tag == "Zombie").ToArray();
        enemies = enemies.Where(enemy => Mathf.Abs(Vector3.Angle(enemy.transform.position - transform.position, player.MovementAxis * (facingRight ? 1 : -1))) < (attackAngle / 2)).ToArray();
        if (enemies.Length == 0) return;
        //print(Vector2.Angle((enemies[0].transform.position - transform.position).XZ(), (player.MovementAxis * (facingRight ? 1 : -1)).XZ()));
        foreach (var enemy in enemies)
        {
            enemy.GetComponent<Zombie>().Damage(attackDamage);
            enemy.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(enemy.transform.position - transform.position) * attackKnockback, ForceMode.Impulse);
        }
    }

    //this attack attempts to convert zombies to human
    public void AttackConvert()
    {
        if (!offCooldown || injecting) return;  //if the players attack is on cooldown, cancel attack

        //turns on the slash graphical effect
        slash.GetChild(0).GetComponent<Renderer>().material.color = Color.green;
        slash.eulerAngles = new Vector3(0, cam.eulerAngles.y, 0);
        slash.localScale = Vector3.Scale(((facingRight ? 1 : -1) * new Vector3(1, 0, 1)), slash.transform.localScale.Abs()) + new Vector3(0, 1, 0);
        slash.gameObject.SetActive(true);
        //turns off the slash effect after a set amount of time
        Timer.SimpleTimer(() => slash.gameObject.SetActive(false), .2f);

        //Sets the attack to be on cooldown
        offCooldown = false;

        Timer.SimpleTimer(() => offCooldown = true, attackCooldown);//after attackCooldown seconds, the player can attack again


        Collider[] enemies = Physics.OverlapSphere(transform.position, attackRange).Where(col => col.gameObject.tag == "Zombie").ToArray();
        enemies = enemies.Where(enemy => Mathf.Abs(Vector3.Angle(enemy.transform.position - transform.position, player.MovementAxis * (facingRight ? 1 : -1))) < (attackAngle / 2) 
        && enemy.GetComponent<Zombie>().convertible).ToArray();
        if (enemies.Length == 0) return;
        //print(Vector2.Angle((enemies[0].transform.position - transform.position).XZ(), (player.MovementAxis * (facingRight ? 1 : -1)).XZ()));
        foreach (var enemy in enemies)
        {
            enemy.GetComponent<Zombie>().Damage(attackDamage);
            enemy.GetComponent<Zombie>().StartConversion(InjectionTime);
        }
        injecting = true;
        player.canMove = false;
        Timer.SimpleTimer(() =>
        {
            player.canMove = true;
            injecting = false;
        }
        , InjectionTime); //must convert to couroutine to allow for cancelation upon taking damage OR test using a dotween along with the Kill() method
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
        if(Application.isPlaying) Gizmos.DrawLine(transform.position, transform.position + (player.MovementAxis * (facingRight ? 1 : -1)) * 5);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Zombie" && !invincible)
        {
            //subtracts health from the player
            currentHealth--;
            if (currentHealth <= 0) {/*============ INSERT GAME OVER ACTIONS HERE ================*/}

            //sets the player to be invincible, then turns off invincibility after invincibleTime seconds
            invincible = true;
            Timer.SimpleTimer(() => invincible = false, invincibleTime);

            //gets the direction the zombie is in accordance to where the zombie is compared tot he player
            //then knocks the playerback
            Vector3 knockbackDirection = Vector3.Scale((transform.position - other.transform.position), player.MovementAxis.Abs());
            _rb.DOMove(transform.position + (knockbackDirection * knockbackDistance), knockbackSpeed).SetEase(Ease.OutQuad);

        }
    }
}
