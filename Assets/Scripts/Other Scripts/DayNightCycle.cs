using UnityEngine;
using TMPro;
using System;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] float timeMultiply;

    [SerializeField] float firstHour;

    [SerializeField] TextMeshProUGUI timeText;

    [SerializeField] Light sunLight;

    [SerializeField] float sunRiseHour;

    [SerializeField] float sunSetHour;

    [SerializeField] Color dayLight;

    [SerializeField] Color nightLight;

    [SerializeField] AnimationCurve lightChangeCurve;

    [SerializeField] float maxSunIntensity;

    [SerializeField] Light moonLight;

    [SerializeField] float maxMoonIntensity;

    TimeSpan dayTime;
    TimeSpan nightTime;

    DateTime currentTime;

    GameObject lightController;
    bool verify;
    public static DayNightCycle instance;


    void Awake() 
    {

        lightController = GameObject.Find("LightController");
        verify = sunLight != null && moonLight != null && lightController != null;
    }

    void Start() 
    {
        if (verify) 
        {
            currentTime = DateTime.Now.Date + TimeSpan.FromHours(firstHour);
            dayTime = TimeSpan.FromHours(sunRiseHour);
            nightTime = TimeSpan.FromHours(sunSetHour);
        }
        else 
        {
            throw new MissingComponentException("Some variables are null.");
        }
    }

    void Update() 
    {
        UpdateTime();
        rotateSun();
        changeLightSettings();
    }

    void UpdateTime() 
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiply);

        if (timeText != null) 
        {
            timeText.text = currentTime.ToString("HH:mm");
        }
        else 
        {
            throw new UnityException("timeText is null.");
        } 
    }

    void rotateSun() 
    {
        float sunRotation = 0;

        if (currentTime.TimeOfDay > dayTime && currentTime.TimeOfDay < nightTime) 
        {
            TimeSpan dayToNightDuration = calculateTimeDifference(dayTime, nightTime);
            TimeSpan timeSinceSunrise = calculateTimeDifference(dayTime, currentTime.TimeOfDay);

            double timePercentage = timeSinceSunrise.TotalMinutes / dayToNightDuration.TotalMinutes;

            sunRotation = Mathf.Lerp(0, 180, (float) timePercentage);
        }
        else 
        {
            TimeSpan nightToDayDuration = calculateTimeDifference(nightTime, dayTime);
            TimeSpan timeSinceSunset = calculateTimeDifference(nightTime, currentTime.TimeOfDay);

            double timePercentage = timeSinceSunset.TotalMinutes / nightToDayDuration.TotalMinutes;

            sunRotation = Mathf.Lerp(180, 360, (float) timePercentage);
        }
        
        sunLight.transform.rotation = Quaternion.AngleAxis(sunRotation, Vector3.right);
    }

    void changeLightSettings() 
    {
        float dot = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        sunLight.intensity = Mathf.Lerp(0, maxSunIntensity, lightChangeCurve.Evaluate(dot));
        moonLight.intensity = Mathf.Lerp(maxMoonIntensity, 0, lightChangeCurve.Evaluate(dot));
        RenderSettings.ambientLight = Color.Lerp(nightLight, dayLight, lightChangeCurve.Evaluate(dot));
    }

    TimeSpan calculateTimeDifference(TimeSpan fromTime, TimeSpan toTime) 
    {
        TimeSpan result = toTime - fromTime;

        if (result.TotalSeconds < 0) 
        {  
            result += TimeSpan.FromHours(24);
        }

        return result;
    }
}
