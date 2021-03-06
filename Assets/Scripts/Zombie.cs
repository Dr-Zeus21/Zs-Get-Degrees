using DG.Tweening;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public int maxHealth = 5;
    public int health = 5;
    public int conversionThreshhold = 5;  //if health is <= this, the zombie can be cured

    //turns true if the zombie is able to be converted or not
    public bool convertible = false;

    public bool converted = false;


    Material _mat;  //the material of the zombie
    NavMeshNavigation _navMnav;  //the NavMeshNavigation of the zombie
    [SerializeField, ReadOnly] float ConversionPercent = 0;  //whate percent of the zombie is converted

    //these are buttons for testing purposes
    public bool startInjection = false;
    public bool reset = false;


    [SerializeField] GameObject Syringe; //the syringe
    [SerializeField] Transform syringStartPos;
    [SerializeField] Transform syringEndPos;

    [SerializeField] Transform van;

    [SerializeField] ZombieAnimationHandler ZAH;

    FieldOfView _FOV;

    public bool stunned;

    private void Awake()
    {
        //_mat = GetComponent<Renderer>().material;
        _navMnav = GetComponent<NavMeshNavigation>();
        _FOV = GetComponent<FieldOfView>();
    }
    public void StartConversion(float time)
    {
        //stops the zombie from moving
        _navMnav.canMove = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        //THIS BLOCK MOVES ConversionPercent (which starts at 0) TO 1 OVER THE COURSE OF time SECONDS
        DOTween.To(() => ConversionPercent, 
            (x) => {
            _mat.SetFloat("ConversionRate", x);  //the conversion percent has a visual aspect, which is inserted into the material here
            ConversionPercent = x;},
            1, time).OnComplete(() =>
            {  //this block occurs once the previous block is complete (i.e. when ConversionPercent ==1)
                _navMnav.canMove = true;
                Syringe.SetActive(false);
                _mat.SetInt("Convertible", 0);
            });
        Syringe.SetActive(true);

        //moves the syringe into the zombie
        Syringe.transform.DOMove(syringEndPos.position, time * .1f).SetEase(Ease.InQuad).
            OnComplete(() => Syringe.GetComponent<Syringe>().
            StartInjection(time * .8f, () =>
            {//once the animatione is done, the zombie is considered to be Converted
                gameObject.tag = "Converted";
                Syringe.transform.DOMove(syringStartPos.position, time * .1f);
                _navMnav.destination = van;
                _navMnav.baseSpeed = 2.5f;
                converted = true;
                _FOV.viewMeshFilter.gameObject.SetActive(false);
            }));
    }
    public void ResetConversion()
    {
        ConversionPercent = 1;
        _mat.SetFloat("ConversionRate", 1);
    }

    private void Update()
    {
        //if nothing is found, rotate around
        //if (!_navMnav.destination) transform.eulerAngles += (new Vector3(0, .5f, 0));

        //if player is found, walk towards him and begin to walk
        if (_FOV.visibleTargets.Count > 0 && !converted)
        {
            _navMnav.destination = _FOV.visibleTargets[0];
            if(!stunned) ZAH.Walk();

        }
        if (stunned) _navMnav.baseSpeed = 0;
        else _navMnav.baseSpeed = 1;










        //BUTTONS
        if (startInjection)
        {
            startInjection = false;
            StartConversion(5);
        }
        if (reset)
        {
            reset = false;
            ResetConversion();
        }
    }

    //use this function to damage zombies
    //returns true is this attack killed the zombie
    public bool Damage(int amount)
    {
        ZAH.Idle();
        stunned = true;
        Timer.SimpleTimer(() => stunned = false, .5f);

        health--;
        if (health <= 0)
        {
            Destroy(gameObject);
            return true;
        }
        if (health <= conversionThreshhold)
        {
            convertible = true;
            _mat.SetInt("Convertible", 1);
        }
        return false;
    }
}
