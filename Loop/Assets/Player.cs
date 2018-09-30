using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public bool started;
    [Space(10)]

    public float holdTimer = 3;
    Matrix4x4 calibrationMatrix;

    private bool mtrailActive;
    public bool TrailActive
    {
        get {
            return mtrailActive;
        }
        set
        {
            mtrailActive = value;

            if(mtrailActive)
            {
                topTrail.gameObject.SetActive(true);
                bottomTrail.gameObject.SetActive(true);

            }
            else
            {
                topTrail.gameObject.SetActive(false);
                bottomTrail.gameObject.SetActive(false);
            }
        }
    }


    Vector3 wantedDeadZone = Vector3.zero;
    Vector3 _InputDir;
    Vector3 dragOrigin;
    public float dragSpeed;
    public float accellSpeed;

    private int curTouchID;

    public SpriteRenderer frontSRend;
    public SpriteRenderer backSRend;
    public bool validTouch;

    public ManualTrail topTrail;
    public ManualTrail bottomTrail;

    public Color otherColor;

    public bool rainbow;
    public HueShifter hueShifter;

    public Material normalMat;
    public Material rainbowMat;


    void Start()
    {
        hueShifter = GetComponent<HueShifter>();
        GameManager.instance.SetInitalValues();
    }

    public void Update()
    {
        if (started)
        {
            
            if (GameManager.instance.saveData.accelEnabled)
            {
                _InputDir = getAccelerometer(Input.acceleration);
                Vector3 accellMove = new Vector3(_InputDir.y * accellSpeed, -_InputDir.x * accellSpeed, 0);

                transform.Translate(accellMove, Space.World);
            }
            else
            {
                Vector3 pos = Vector3.zero;
                Vector3 move = Vector3.zero;

                // Mobile Controls
                #if UNITY_ANDROID && !UNITY_EDITOR || UNITY_IOS && !UNITY_EDITOR


            Touch touch = Input.GetTouch(0);

            if(curTouchID != touch.fingerId)
            {
                curTouchID = touch.fingerId;
            }
            if (touch.phase == TouchPhase.Began && touch.fingerId == curTouchID)
            {
                dragOrigin = Input.GetTouch(0).position;
            }
                if (touch.phase == TouchPhase.Moved && touch.fingerId == curTouchID)
            {
                    pos = Camera.main.ScreenToViewportPoint((Vector3)touch.position - dragOrigin);
                move = new Vector3(pos.y * dragSpeed, -pos.x * dragSpeed, 0);
            }

            transform.Translate(move, Space.World);
            dragOrigin = touch.position;
#endif
                // PC Test Controls
#if UNITY_EDITOR

                if (Input.GetMouseButtonDown(0))
                {
                    dragOrigin = Input.mousePosition;
                    return;
                }

                if (!Input.GetMouseButton(0)) return;

                pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
                move = new Vector3(pos.y * dragSpeed, -pos.x * dragSpeed, 0);

                transform.Translate(move, Space.World);
                dragOrigin = Input.mousePosition;
#endif

            }
            DeathzoneCheck();
        }
        else
        {
            // Check for hold to start
            #if UNITY_ANDROID && !UNITY_EDITOR || UNITY_IOS && !UNITY_EDITOR


            Touch touch = Input.GetTouch(0);

            if(curTouchID != touch.fingerId)
            {
                curTouchID = touch.fingerId;
            }
            if (touch.phase == TouchPhase.Began && touch.fingerId == curTouchID)
            {
                dragOrigin = touch.position;
                validTouch |= !IsPointerOverUIObject();   
                if(validTouch)
                {
                    StartCoroutine(Countdown());
                }  

            }
            if (touch.phase == TouchPhase.Moved && touch.fingerId == curTouchID && validTouch || touch.phase == TouchPhase.Stationary && touch.fingerId == curTouchID && validTouch)
            {
                if (holdTimer <= 0)
                {
            for (int i = 0; i < GameManager.instance.UIManager.startUpLayout.Length; i++)
                    {
                        GameManager.instance.UIManager.startUpLayout[i].GetComponent<Image>().color = Color.green;
                    }
                    dragOrigin = touch.position;
                    GameManager.instance.UIManager.startText.text = "BEGIN!";
                    GameManager.instance.UIManager.StartButtonPressed();
                    return;
                }
            }

            if(touch.phase == TouchPhase.Ended && holdTimer > 0)
            {
                StopAllCoroutines();
                validTouch = false;
                holdTimer = 3;
            for (int i = 0; i < GameManager.instance.UIManager.startUpLayout.Length; i++)
                {
                    GameManager.instance.UIManager.startUpLayout[i].GetComponent<Image>().color = Color.white;
                }
            }
#endif
            // PC Test Controls
#if UNITY_EDITOR

            if (Input.GetMouseButtonDown(0) )
            {
                dragOrigin = Input.mousePosition;
                validTouch |= !IsPointerOverUIObject();   
                if(validTouch)
                {
                    StartCoroutine(Countdown());
                }
            }
            if (Input.GetMouseButton(0) && validTouch)
            {
                if (holdTimer <= 0)
                {
                    StopAllCoroutines();
                    for (int i = 0; i < GameManager.instance.UIManager.startUpLayout.Length; i++)
                    {
                        GameManager.instance.UIManager.startUpLayout[i].GetComponent<Image>().color = otherColor;
                    }
                    dragOrigin = Input.mousePosition;
                    GameManager.instance.UIManager.startText.text = "BEGIN!";
                    GameManager.instance.UIManager.StartButtonPressed();
                    return;
                }
                return;
            }

            if(holdTimer > 0)
            {
                StopAllCoroutines();
                validTouch = false;
                holdTimer = 3;
                for (int i = 0; i < GameManager.instance.UIManager.startUpLayout.Length; i++)
                {
                    GameManager.instance.UIManager.startUpLayout[i].GetComponent<Image>().color = Color.white;
                }
            }
#endif
        }
    }

    public IEnumerator Countdown ()
    {
        while (true)
        {
            holdTimer -= 1;

            if(!GameManager.instance.gameStarted)
            {
                for (int i = 0; i < GameManager.instance.UIManager.startUpLayout.Length; i++)
                {
                    if (i >= holdTimer)
                    {
                        GameManager.instance.UIManager.startUpLayout[i].GetComponent<Image>().color = otherColor;
                        //GameManager.instance.UIManager.startUpLayout[i].GetComponent<Image>().color = Color.green;
                    }
                }
                GameManager.instance.soundManager.PlayClip(GameManager.instance.soundManager.pulseSound, 0.1f);

                yield return new WaitForSeconds(0.5f);
                yield return true;
            }
            else
            {

                yield break;
            }
        }

    }

    Vector3 getAccelerometer(Vector3 accelerator)
    {
        Vector3 accel = this.calibrationMatrix.MultiplyVector(accelerator);
        return accel;
    }

    public void calibrateAccelerometer()
    {
        wantedDeadZone = Input.acceleration;
        Quaternion rotateQuaternion = Quaternion.FromToRotation(new Vector3(0f, 0f, -1f), wantedDeadZone);
        //create identity matrix ... rotate our matrix to match up with down vec
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, rotateQuaternion, new Vector3(1f, 1f, 1f));
        //get the inverse of the matrix
        calibrationMatrix = matrix.inverse;

    }

    public void CollisionDetected(PlayerCollider childScript)
    {
        if(started)
        {
            GameManager.instance.soundManager.PlayClipWithPitch(GameManager.instance.soundManager.deathSound, 0.1f, 0.8f);

            enabled = false;

            frontSRend.color = Color.black;
            backSRend.color = Color.black;
            GameManager.instance.PlayerLoss(); 
        }

    }

    public void PickUpCollisionDetected () {
        GameManager.instance.gemsFound += 1;
        GameManager.instance.UIManager.gemTrackerText.text = GameManager.instance.gemsFound.ToString();
    }

    void DeathzoneCheck ()
    {
        Vector3 deathTop = GameManager.instance.UIManager.deathzone.GetComponent<RectTransform>().offsetMax;

        if (transform.position.x <= Camera.main.ScreenToWorldPoint(deathTop).x - 2)
        {
            GameManager.instance.soundManager.PlayClipWithPitch(GameManager.instance.soundManager.deathSound, 0.1f, 0.8f);

            enabled = false;

            frontSRend.color = Color.black;
            backSRend.color = Color.black;
            GameManager.instance.PlayerLoss(); 
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
