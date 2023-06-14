using System.Collections.Generic;
using UnityEngine;

public enum ShakeType
{
    Smooth,
    Rough
}

/// <summary>
/// 실행되는 Shake는 List로 Duration에 따라 각각 진행 후 최종위치로 이동
/// ShakeData는 Dictionary로 관리(Key : ShakeDataName(string))
/// </summary>
public class CameraEffect : MonoBehaviour
{
    public static string EffectDataPrfabFolderPath = "EffectDataPrefabs";

    public static CameraEffect instance = null;

    public CameraTest ct;

    Vector3 originPos;
    Vector3 originRot;

    private bool curDataStop;

    public bool zoomStart;

    [SerializeField]
    private List<EffectData> List_EffectDatas = new List<EffectData>();
    private Dictionary<string, EffectData> Dic_EffectDatas = new Dictionary<string, EffectData>();
    public EffectData curData;
    public Zoom curZoom;

    public Vector3 OriginPos { get { return originPos; } }
    public Vector3 OriginRot { get { return originRot; } }
    public bool Stop { get { return curDataStop; } set { curDataStop = value; } }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(instance);
    }

    private void Start()
    {
        Load_Dic();

        originPos =  Camera.main.transform.localPosition;
        originRot = Camera.main.transform.localEulerAngles;
    }
    private void Update()
    {
        if (curZoom != null)
        {
            if (!curZoom.isFinish)
            {
                if (!zoomStart) zoomStart = true;
                curZoom.Update();
            }
            else
            {
                zoomStart = false;
                curZoom.isFinish = false;
                curZoom = null;
            }
        }
    }
    private void LateUpdate()
    {
        if(curData) curData.Update();
    }

    private void Load_Dic()
    {
        EffectData[] Datas =  Resources.LoadAll<EffectData>(EffectDataPrfabFolderPath);
        for(int i = 0; i < Datas.Length; i++)
        {
            EffectData data = new EffectData(Datas[i].duration,Datas[i].shakeSpeed, Datas[i].magnitude, Datas[i].shakePosition,
                                             Datas[i].shakeRotation, Datas[i].Curve, Datas[i].isSeedUpdate);
            Dic_EffectDatas.Add(Datas[i].name, Datas[i]);
        }
    }

    public void PlayShake(string dataName)
    {
        if (curData != null) return;

        if (Dic_EffectDatas.ContainsKey(dataName))
        {
            EffectData data = Dic_EffectDatas[dataName];

            if (curData != null)
            {
                if (curData.roop && data.roop)
                {
                    if (curData != data) curData = data;
                }
                else curData = data;
            }
            else curData = data;

            curData.Start = true;
        }
    }

    public void PlayZoom(ZoomDir dir,float speed,float duration,Vector3 originPos)
    {
        curZoom = new Zoom(dir, speed, duration, originPos);
    }
    public void PlayZoom(ZoomDir dir, float speed,bool check)
    {
        curZoom = new Zoom(dir, speed, check);
    }


    public void PlayStepEffect()
    {
        //찌르는 이펙트(앞으로 나아가는 이펙트)
    }
    public void PlayLeftAttEffect()
    {
        //PlayShake("Player_LeftAtt");
    }
    public void PlayRightAttEffect()
    {
        //PlayShake("Player_RightAtt");
    }
    public void PlayRollAttEffect()
    {
        PlayShake("Player_RollAtt");
        PlayZoom(ZoomDir.Front, 1f, 1f,Camera.main.transform.localPosition);
    }
    public void PlaySpritAttEffect()
    {
        PlayShake("Player_SpritAtt");
    }
    public void PlayGuardEffect()
    {
        PlayShake("Player_Guard");
        PlayZoom(ZoomDir.Back, 1f, 1f, Camera.main.transform.localPosition);
    }

    public void PlayTwoHandAttEffect()
    {
        //양손공격(내려찍기,오른쪽으로 휘두르기)
    }

    public void ChargeAttEffect()
    {
        PlayZoom(ZoomDir.Front, 0.7f, true);
    }
    public void SuccessParringEffect()
    {

    }
    public void SuccessHoldEffect()
    {

    }
    public void HitEffect()
    {
        
    }
}

public enum ZoomDir
{
    Front,
    Back,
}
public class Zoom
{
    private bool check;
    public ZoomDir dir;
    public float speed;
    public float power;
    public float duration;
    public Vector3 originPos = new Vector3(0.0f,0.0f,-2f);
    private float startTimer;

    public bool isFinish;
    public bool isFinishDir;
    public bool startBack;

    private Vector3 Dir;
    private Vector3 tempPos;

    public bool Check { get{ return check; } set{ check = value; } }

    public Zoom(ZoomDir _dir, float _speed,bool _check)
    {
        dir = _dir;
        speed = _speed;
        check = _check;
        if (dir == ZoomDir.Front)
        {
            Vector3 vec = Player.instance.transform.position - Camera.main.transform.localPosition;
            Dir = vec.normalized;
        }
        else
        {
            Vector3 vec = Camera.main.transform.localPosition - Player.instance.transform.position;
            Dir = vec.normalized;
        }
    }
    public Zoom(ZoomDir _dir, float _speed, float _duration,Vector3 _originPos)
    {
        dir = _dir;
        speed = _speed;
        duration = _duration;
        originPos = _originPos;
        if (dir == ZoomDir.Front)
        {
            Vector3 vec = Player.instance.transform.position - Camera.main.transform.localPosition;
            Dir = vec.normalized;
        }
        else
        {
            Vector3 vec = Camera.main.transform.localPosition - Player.instance.transform.position;
            Dir = vec.normalized;
        }
    }
    public void Update()
    {
        if (!check) Play();
        else CheckPlay();
    }
    private void Play()
    {
        if (startTimer < duration)
        {
            startTimer += Time.deltaTime;
            if (!isFinishDir)
            {
                if (startTimer <= duration * 0.5f)
                {
                    Vector3 vec = new Vector3(0.0f, 0.0f, originPos.z + -Dir.z * speed * startTimer);
                    if (vec.z > -1f) vec.z = -1f;
                    else if(vec.z < -13f) vec.z = -13f;
                    Camera.main.transform.localPosition = vec;
                    Debug.Log(vec);
                }
                else
                {
                    tempPos = Camera.main.transform.localPosition;
                    isFinishDir = true;
                    startTimer = 0.0f;
                }
            }
            else
            {
                startTimer += Time.deltaTime;
                Vector3 vec = new Vector3(0.0f, 0.0f, tempPos.z + Dir.z * speed * startTimer);
                if (vec.z > -1f) vec.z = -1f;
                else if (vec.z < -13f) vec.z = -13f;
                Camera.main.transform.localPosition = vec;
                Debug.Log(vec);
            }
        }
        else
        {
            //Camera.main.transform.localPosition = originPos;
            isFinish = true;
            isFinishDir = false;
            startTimer = 0.0f;
        }
    }

    public void CheckPlay()
    {
        if(!isFinishDir)
        {
            //진행
            startTimer += Time.deltaTime;
            Vector3 vec = new Vector3(0.0f, 0.0f, originPos.z + -Dir.z * speed * startTimer);
            if (vec.z > -1f) vec.z = -1f;
            else if (vec.z < -13f) vec.z = -13f;
            Camera.main.transform.localPosition = vec;
        }
        else
        {
            //외부에서 강제로 꺼
            tempPos = Camera.main.transform.localPosition;
            startTimer = 0.0f;
            startBack = true;
        }

        if(startBack)
        {
            startTimer += Time.deltaTime;
            Vector3 vec = new Vector3(0.0f, 0.0f, tempPos.z + Dir.z * speed * startTimer);
            if (vec.z > -1f) vec.z = -1f;
            else if (vec.z < -13f) vec.z = -13f;
            Camera.main.transform.localPosition = vec;
            if (originPos == Camera.main.transform.localPosition)
            {
                startBack = false;
                isFinish = true;
            }
        }
    }
}
