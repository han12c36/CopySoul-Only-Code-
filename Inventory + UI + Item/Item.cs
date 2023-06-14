using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public Enums.ObjNameTable objName;
    public Enums.ObjectType ObjectType;
    public Rigidbody rigid;
    public Enums.ItemType itemType;
    public Sprite itemImage;
    public Collider collider;
    public bool isHand = false;
    private int count = 1;

    public float speed = 10f;

    public abstract void Initialize();

    protected virtual void Awake()
    {
        Initialize();
    }
    protected virtual void Start()
    {
        collider = GetComponent<Collider>();
        rigid = GetComponent<Rigidbody>();
    }
    protected virtual void Update()
    {
        if (!isHand && ObjectType == Enums.ObjectType.Item) Orbit_Rotation();
    }
    protected virtual void FixedUpdate()
    {
        if (!isHand && ObjectType == Enums.ObjectType.Item) Bounce();
    }
    private void Orbit_Rotation()
    {
        //Vector3.down : 반시계 방향
        //Vector3.up : 시계 방향
        transform.Rotate(Vector3.down * speed * Time.deltaTime);
    }
    private void Bounce()
    {
        if(rigid)
        rigid.AddForce(Vector3.up * 0.1f, ForceMode.Impulse);
    }

    public virtual void PlayFuncs()
    {

    }

    public int Count
    {
        get{ return count; }
        set{ count = value; }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            collider.enabled = false;
            rigid.isKinematic = true;
        }
    }
}
