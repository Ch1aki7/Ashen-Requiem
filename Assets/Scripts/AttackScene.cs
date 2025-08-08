using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class AttackScene : MonoBehaviour
{
    private static AttackScene instance;
    public static AttackScene Instance
    {
        get
        {
            if (instance == null)
                instance = Object.FindFirstObjectByType<AttackScene>();
            return instance;
        }
    }



    private bool isShake;

    public void HitPause(int duration)
    {
        StartCoroutine(Pause(duration));
    }

    IEnumerator Pause(int duration)
    {
        float pauseTime = duration / 60f;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(pauseTime);
        Time.timeScale = 1;
    }

    public void UseTimeShrink(int duration)
    {
        StartCoroutine(TimeShrink(duration));
    }

    IEnumerator TimeShrink(int duration)
    {
        float pauseTime = duration / 60f;
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(pauseTime);
        Time.timeScale = 1;
    }

    public void CameraShake(float duration, float magnitude)
    {
        if(!isShake)
            StartCoroutine(Shake(duration, magnitude));
    }

    //IEnumerator Shake(float duration, float magnitude)
    //{
    //    isShake = true;
    //    Transform camera = Camera.main.transform;

    //    Transform cinemaMachine = camera.transform.parent.Find("CinemachineCamera");
    //    var vm = cinemaMachine.GetComponent<CinemachineCamera>();
    //    float shackBuffer = .1f;
    //    vm.enabled = false;

    //    Vector3 startPosition = camera.position;

    //    while (duration > 0) 
    //    {
    //        camera.position = Random.insideUnitSphere * magnitude + startPosition;
    //        duration -= Time.deltaTime;

    //        yield return null;
    //    }
    //    while (shackBuffer > 0)
    //    {
    //        shackBuffer -= Time.deltaTime;

    //        yield return null;
    //    }
    //    camera.position = startPosition;
    //    isShake= false;
    //    vm.enabled = true;
    //}
    IEnumerator Shake(float duration, float magnitude)
    {
        isShake = true;
        Transform camera = Camera.main.transform;

        Transform cinemaMachine = camera.transform.parent.Find("CinemachineCamera");
        var vm = cinemaMachine.GetComponent<CinemachineCamera>();
        Vector3 startPosition = camera.position;
        //vm.enabled = false;
        vm.Follow = null;
        while (duration > 0)
        {
            cinemaMachine.position = Random.insideUnitSphere * magnitude + startPosition;
            duration -= Time.deltaTime;

            yield return null;
        }
        camera.position = startPosition;
        isShake = false;
        
        //vm.enabled = true;
        vm.Follow = PlayerManager.instance.player.transform;
    }
}
