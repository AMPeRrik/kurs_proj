using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class СOI_lvl1 : MonoBehaviour
{
    //LevelOne()
    public Transform ValveContr;
    float F_Water_FT_1_1 = 165.75f; //reference Position 4
    public float valve_angle1_1, valve_angle1_2, valve_angle1_3, valve_angle2, valve_angle3_1, valve_angle3_2;
    bool pumpON_OFF = false;
    public float L_Water_Pos1 = 100f, L_Water_Pos2, L_Water_Pos3_1, L_Water_Pos3_2;

    //Text
    //public Text Textinput, TextOutput;
    public GameObject Settings_parameters;

    //Player
    public CameraMove Player;
    public Transform Camera;
    Ray ray;
    RaycastHit info;
    public string nameObject;

    //Valve
    ValveRotation1lvl valverotation;

    //Calc_Circles()
    float sign1, sign2;
    public float angle, angle180, number;

    //Other
    public bool trackingMode;
    public bool valve_open_clos;

    public void valve_Open_Clos()
    {
        valve_open_clos = !valve_open_clos;
    }
    public void TrackingModeOnOff()
    {
        trackingMode = !trackingMode;
        Player.enabled = !Player.enabled;
    }
    public void RotateValve()
    {
        valve_Open_Clos();
        TrackingModeOnOff();
        valverotation.RotateValveModeOnOff();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Settings_parameters.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !trackingMode)
        {
            ray = new Ray(Camera.position, Camera.forward);
            if (Physics.Raycast(ray, out info))
            {
                valverotation = info.collider.gameObject.GetComponent<ValveRotation1lvl>();
                if (valverotation)
                {
                    nameObject = info.collider.gameObject.name;
                    Recalc_Circles();
                    ValveContr = GameObject.Find(nameObject).GetComponent<Transform>();
                    Settings_parameters.SetActive(true);
                    Cursor.lockState = CursorLockMode.Confined;
                    RotateValve();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (valve_open_clos)
            {
                RotateValve();
                Cursor.lockState = CursorLockMode.Locked;
                Settings_parameters.SetActive(false);
            } else
            {
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene(0);
            }
        }
    }
    private void FixedUpdate()
    {
        if (valve_open_clos)
        {
            angle = ValveContr.eulerAngles.y;
            Calc_Circles();
            //TextOutput.text = "Действительное значение \nрасхода " + F_valve_angle1_1;
        }
        LevelOne();
    }
    //CALCULATE OF INDICATORS
    public void LevelOne()
    {
        if (angle180 == 0f)
            if (angle <= 180f)
            {
                switch (nameObject)
                {
                    case "valve_2":
                        valve_angle2 = angle;
                        break;
                    case "valve3_1":
                        valve_angle3_1 = angle;
                        break;
                    case "valve3_2":
                        valve_angle3_2 = angle;
                        break;
                }
            }
            else
            {
                switch (nameObject)
                {
                    case "valve_2":
                        valve_angle2 = 0f;
                        break;
                    case "valve3_1":
                        valve_angle3_1 = 0f;
                        break;
                    case "valve3_2":
                        valve_angle3_2 = 0f;
                        break;
                }
            }
        else if (angle180 > 0f)
        {
            float angle2 = Mathf.Clamp(angle + number * 360f, 0f, 720f);
            switch (nameObject)
            {
                case "valve_2":
                    valve_angle2 = angle2;
                    break;
                case "valve3_1":
                    valve_angle3_1 = angle2;
                    break;
                case "valve3_2":
                    valve_angle3_2 = angle2;
                    break;
            }
        }
        //720 grad = 100%
        //180 m3/min = 100%
        //1 grad = 0.25 m3/min
        //Position 1
        if (valve_angle1_1 != 0f)
        {
            //60*50=3000; 60 - min->sec, 50 - FixedUpdate
            L_Water_Pos1 += 0.25f * valve_angle1_1 / 3000f;
        }
        if (valve_angle1_2 != 0f)
        {
            L_Water_Pos1 += 0.25f * valve_angle1_2 / 3000f;
        }
        if (valve_angle1_3 != 0f)
        {
            L_Water_Pos1 += 0.25f * valve_angle1_3 / 3000f;
        }

        //720 grad = 100%
        //540 m3/min = 100%
        //1 grad = 0.75 m3/min
        //Position 2
        if (valve_angle2 != 0f && L_Water_Pos1 > 0f)
        {
            L_Water_Pos1 -= 0.75f * valve_angle2 / 3000f;
            L_Water_Pos2 += 0.75f * valve_angle2 / 3000f;
        }

        //720 grad = 100%
        //540 m3/min = 100%
        //1 grad = 0.75 m3/min
        //0.75 / 2 = 0.375 (2 входа)
        //Position 3
        if (valve_angle3_1 != 0f && L_Water_Pos2 > 0f)
        {
            L_Water_Pos2 -= 0.375f * valve_angle3_1 / 3000f;
            L_Water_Pos3_1 += 0.375f * valve_angle3_1 / 3000f;
        }
        if (valve_angle3_2 != 0f && L_Water_Pos2 > 0f)
        {
            L_Water_Pos2 -= 0.375f * valve_angle3_2 / 3000f;
            L_Water_Pos3_2 += 0.375f * valve_angle3_2 / 3000f;
        }

        //1 насос = 55.25 m3/min
        //55.25 * 3 = 165.75 (3 насоса)
        //Position 4
        if (pumpON_OFF == true && L_Water_Pos3_1 > 0f && L_Water_Pos3_2 > 0f)
        {
            // 3 насоса на 2 отстойника
            L_Water_Pos3_1 -= F_Water_FT_1_1 / (3000f * 2f);
            L_Water_Pos3_2 -= F_Water_FT_1_1 / (3000f * 2f);
        }
    }
    //CALCULATE NUMBER CIRCLES
    public void Recalc_Circles()
    {
        switch (nameObject)
        {
            case "valve_2":
                angle = valve_angle2;
                break;
            case "valve3_1":
                angle = valve_angle3_1;
                break;
            case "valve3_2":
                angle = valve_angle3_2;
                break;
        }
        number = Mathf.Floor(angle / 360f);
        angle180 = Mathf.Floor(angle / 180f) - number;
    }
    public void Calc_Circles()
    {
        if (angle > 45f && angle < 135f)
        {
            sign1 = 1f;
            sign2 = 0;
        }
        if (angle > 225f && angle < 315f)
        {
            sign1 = 0;
            sign2 = 1f;
        }
        if (angle > 180f && angle < 225f )
        {
            if (number == angle180 && sign1 > 0)
                angle180 += 1f;
            if (number > angle180)
            {
                angle180 += 1f;
                if (sign1 > 0)
                {
                    sign1 = 0;
                    sign2 = 1f;
                }
            }
        }
        if (angle < 45f)
        {
            if (number != angle180 && angle180 > 0)
            {
                number += 1f;
                sign1 = 1f;
                sign2 = 2f;
            }
            if (number == angle180 && (sign1 == 0 || sign1 == 2f) && sign2 > 0 && angle180 != 0)
                number += 1f;
        }
        if (angle > 135f && angle < 180f)
        {
            if (number == angle180 && sign2 > 0)
                angle180 -= 1f;
            if (number < angle180)
            {
                angle180 -= 1f;
                if (sign2 > 0)
                {
                    sign1 = 1f;
                    sign2 = 0;
                }
            }
        }
        if (angle > 315f)
        {
            if (number != angle180 && angle180 < 0)
            {
                number -= 1f;
                sign1 = 2f;
                sign2 = 1f;
            }
            if (number == angle180 && sign1 > 0 && (sign2 == 0 || sign2 == 2f) && angle180 != 0)
                number -= 1f;
        }
    }
}
