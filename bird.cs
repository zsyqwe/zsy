using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bird : MonoBehaviour {

    private bool isclick = false;
    public float maxdis = 3;
    [HideInInspector]
    public SpringJoint2D sp;
    protected  Rigidbody2D rg;
    

    public LineRenderer right;
    public LineRenderer left;
    public Transform rightpos;
    public Transform leftpos;

    public GameObject boom;
    protected TestMyTrail myTrail;
    [HideInInspector]
    public  bool canmove = false;
    public float smooth = 3;

    public AudioClip select;
    public AudioClip Fly;

    public Sprite hurt;
    protected  SpriteRenderer  render;
    private bool isfly = false;

    public bool isreleasev = false;
    private void Awake()
    {
        sp = GetComponent<SpringJoint2D>();
        rg = GetComponent<Rigidbody2D>();
        myTrail=GetComponent<TestMyTrail>();
        render=GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()//鼠标按下
    {
        if (canmove)
        {
            audioplay(select);
        isclick = true;
        rg.isKinematic = true;
        }
    }
    private void OnMouseUp()//鼠标抬起
    {
        if (canmove)
        {
        isclick = false;
       
        rg.isKinematic = false;
        Invoke("fly",0.1f);
        //禁用划线组件
        right.enabled = false;
        left.enabled = false;
        canmove = false;
        }
    }
    private void Update()
    {
        if (isclick )//鼠标一直按下，进行位置跟随
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);//坐标转换
           // transform.position += new Vector3(0,0,10);//方法一
            transform.position += new Vector3(0,0,-Camera.main.transform.position.z);//方法2
            if (Vector3.Distance(transform.position,rightpos.position)>maxdis)//计算两个向量之间的位移,进行位置限定。
            {
                Vector3 pos = (transform.position - rightpos.position).normalized;//单位化向量
                pos *= maxdis;//最大长度的向量
                transform.position = pos + rightpos.position;
            }
            Line();
        }
        float posX = transform.position.x;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(Mathf.Clamp(posX, 0, 15), Camera.main.transform.position.y,
            Camera.main.transform.position.z), smooth * Time.deltaTime);
        if (isfly)
        {
            if (Input.GetMouseButtonDown(0))
            {
                showskill();
            }
        }

    }
    void fly()
    {
        isreleasev = true;
        isfly = true;
        audioplay(Fly);
        myTrail.stattrail();
        sp.enabled = false;//禁用springjoint2d；
        Invoke("Next",4);
    }
    //划线
    void Line()
    {
        right.enabled = true;
        left.enabled = true;
        right.SetPosition(0,rightpos.position);
        right.SetPosition(1, transform.position);
        left.SetPosition(0, rightpos.position);
        left.SetPosition(1, transform.position);

    }
    //下一只小鸟飞出
   protected virtual  void Next()
    {
        GameManager._instance.birds.Remove(this);
        Destroy(gameObject);
        Instantiate(boom,transform.position,Quaternion.identity);
        GameManager._instance.NextBird();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isfly = false;
        myTrail.ClearTrail();
      
    }

    public void audioplay(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip,transform.position);
    }

    //技能
    public virtual  void showskill()
    {
        isfly = false;
    }
    public void Hurt()
    {
        render.sprite = hurt;
    }
   
   
}
