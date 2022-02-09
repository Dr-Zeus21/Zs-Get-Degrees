using DG.Tweening;
using System.Linq;
using UnityEngine;
using RengeGames.HealthBars;

[DisallowMultipleComponent]
public class PlayerCombat : MonoBehaviour
{
    ApplesPlayer player;
    BoxCollider2D hitBox;

    //Player Max and current health
    [SerializeField] int MaxHealth;
    [SerializeField] int currentHealth;

    [Header("Atack Stats")]
    //attack stats
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackCooldown = 1; //how many seconds the player must wait until they can attack
    [SerializeField, ReadOnly] float cooldownPercent = 1; //what percent offCooldown is the player (if this is one, the player can attack)
    [SerializeField] bool offCooldown = true;  //if this is true, the player can attack
    [SerializeField] float attackAngle = 180;
    [SerializeField] float attackRange = 4;

    [Header("")]
    [SerializeField, ReadOnly] bool facingRight;

    //the player turns invincible for a bit so they dont take damage
    [SerializeField, ReadOnly] bool invincible = false;
    [SerializeField] float invincibleTime;  //how long the player stays invincible
    [SerializeField] float flashingSpeed;  //how fast the player flashes when hes invincible

    //how far and how fast the player gets knockedback
    [SerializeField, Range(0f, 5.0f)] float knockbackDistance;
    [SerializeField, Range(0f, 2.0f)] float knockbackSpeed;

    //how far attack knocks back zombies;
    [SerializeField] float attackKnockback;

    //how long injection should take
    [SerializeField] float InjectionTime;
    [SerializeField, ReadOnly] bool injecting = false;

    [Header("Unity Assignments")]
    //The healthbar and cooldown bar
    [SerializeField] UltimateCircularHealthBar healthbar;
    [SerializeField] UltimateCircularHealthBar cooldownBar;

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

        healthbar.SegmentCount = MaxHealth;
        healthbar.Color = Color.green;
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
        if (!offCooldown || injecting) return; //if the players attack is on cooldown, or the player is currently injecting, cancel attack

        //turns on the slash graphical effect
        slash.GetChild(0).GetComponent<Renderer>().material.color = Color.white;  //recolors slash based on weather this is a damage attack or a convert attack
        slash.eulerAngles = new Vector3(0,cam.eulerAngles.y+ (facingRight ? 0 : +180), 0);                 //turns slash towards player direction
        /*
        slash.localScale = Vector3.Scale((facingRight ? 1 : -1) * new Vector3(1, 0, 1), //flips the scale to face iether to the left or right of the player
                                         slash.transform.localScale.Abs()) + new Vector3(0, 1, 0);*/
        slash.gameObject.SetActive(true);  //turns on the slash
        Timer.SimpleTimer(() => slash.gameObject.SetActive(false), .2f); //turns off the slash effect after a set amount of time

        offCooldown = false; //Sets the attack to be on cooldown
        Timer.SimpleTimer(() => offCooldown = true, attackCooldown);//after attackCooldown seconds, the player can attack again
        cooldownPercent = 0;
        cooldownBar.Color = Color.grey;
        DOTween.To(() => cooldownPercent, (x) => { cooldownPercent = x; cooldownBar.SetPercent(x); }, 1, attackCooldown).SetEase(Ease.Linear).OnComplete(() => cooldownBar.Color = Color.white);

        //NOW the attack checks for enemies
        Collider[] enemies = Physics.OverlapSphere(transform.position, attackRange).Where(col => col.gameObject.tag == "Zombie").ToArray();  //gets objects with the tag "Zombie" around the player
        enemies = enemies.Where(enemy => Mathf.Abs(Vector3.Angle(enemy.transform.position - transform.position, //gets zombies that are in front of the player, and in the attack angle
            player.MovementAxis * (facingRight ? 1 : -1))) < (attackAngle / 2)).ToArray();
        if (enemies.Length == 0) return;  //if the attack hit nothing, return

        foreach (var enemy in enemies)  //for each enemy that got hit, damage them and knock them back
        {
            enemy.GetComponent<Zombie>().Damage(attackDamage);
            enemy.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(enemy.transform.position - transform.position) * attackKnockback, ForceMode.Impulse);
        }
    }

    //this attack attempts to convert zombies to human
    public void AttackConvert()
    {
        if (!offCooldown || injecting) return;  //if the players attack is on cooldown, or the player is currently injecting, cancel attack

        //turns on the slash graphical effect
        slash.GetChild(0).GetComponent<Renderer>().material.color = Color.green;//recolors slash based on weather this is a damage attack or a convert attack
        slash.eulerAngles = new Vector3(0, cam.eulerAngles.y, 0);               //turns slash towards player direction
        slash.localScale = Vector3.Scale(((facingRight ? 1 : -1) * new Vector3(1, 0, 1)), //flips the scale to face iether to the left or right of the player
            slash.transform.localScale.Abs()) + new Vector3(0, 1, 0);
        slash.gameObject.SetActive(true); //turns on the slash
        Timer.SimpleTimer(() => slash.gameObject.SetActive(false), .2f);//turns off the slash effect after a set amount of time

        offCooldown = false;//Sets the attack to be on cooldown
        Timer.SimpleTimer(() => offCooldown = true, attackCooldown);//after attackCooldown seconds, the player can attack again
        cooldownPercent = 0;
        cooldownBar.Color = Color.grey;
        DOTween.To(() => cooldownPercent, (x) => { cooldownPercent = x; cooldownBar.SetPercent(x); }, 1, attackCooldown).SetEase(Ease.Linear).OnComplete(()=> cooldownBar.Color = Color.white);

        //NOW the attack checks for enemies
        Collider[] enemies = Physics.OverlapSphere(transform.position, attackRange).Where(col => col.gameObject.tag == "Zombie").ToArray();
        enemies = enemies.Where(enemy => Mathf.Abs(Vector3.Angle(enemy.transform.position - transform.position, //whole lotta dumb nerd shit
            player.MovementAxis * (facingRight ? 1 : -1))) < (attackAngle / 2)
        && enemy.GetComponent<Zombie>().convertible).ToArray();
        if (enemies.Length == 0) return;
        foreach (var enemy in enemies)//for each enemy that got hit, begin injecting them
        {
            enemy.GetComponent<Zombie>().StartConversion(InjectionTime);
        }
        injecting = true;
        player.canMove = false;
        Timer.SimpleTimer(() =>  //after the injection is done, the player can start moving again and is no longer injecting
        {
            player.canMove = true;
            injecting = false;
        }
        , InjectionTime); //must convert to couroutine to allow for cancelation upon taking damage OR test using a dotween along with the Kill() method
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
        if (Application.isPlaying) Gizmos.DrawLine(transform.position, transform.position + (player.MovementAxis * (facingRight ? 1 : -1)) * 5);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Zombie" && !invincible)
        {
            //subtracts health from the player
            currentHealth--;
            healthbar.SetPercent((float)currentHealth / MaxHealth);
            if (((float)currentHealth / MaxHealth) < .33) healthbar.Color = Color.red;
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
