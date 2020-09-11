using System.Collections;
using UnityEngine;

/// <summary>
/// Set toggle of monkey animation to joy
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class MonkeyJoyTrigger : MonoBehaviour
{
    private Animator monkeyAnimator;
    private AudioSource monkeyVoice;
    private static readonly int joy = Animator.StringToHash("Joy");

    private void Start()
    {
        monkeyAnimator = GetComponent<Animator>();
        monkeyVoice = GetComponent<AudioSource>();
        monkeyVoice.spatialBlend = 1f;
        StartCoroutine(Joy());
    }

    /// <summary>
    /// Start monkey joy animation and audio 
    /// </summary>
    /// <returns></returns>
    public IEnumerator Joy()
    {
        yield return new WaitForSeconds(5f);
        PlayOneShotMonkeyVoice();
        monkeyAnimator.SetBool(joy, true);
        yield return new WaitForSeconds(5f);
        monkeyAnimator.SetBool(joy, false);
        StartCoroutine(Joy());
    }

    private void PlayOneShotMonkeyVoice()
    {
        monkeyVoice.PlayOneShot(monkeyVoice.clip);
    }
}