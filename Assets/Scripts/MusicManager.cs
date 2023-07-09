using System.Collections;
using System.Collections.Generic;
using Mimic.Audio;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {

    [SerializeField] private MusicCrossfader crossfader;
    [SerializeField] private AudioClip loopableClip;
    private AudioSource _audioSource;
    private bool _readyToSwitch;

    void Start() {
        _audioSource = GetComponent<AudioSource>();
        StartCoroutine(SwitchToLoopableClip(_audioSource.clip.length));
    }

    private IEnumerator SwitchToLoopableClip(float clipLength) {
        yield return new WaitForSeconds(clipLength-0.1f);
        _readyToSwitch = true;
    }

    void Update()
    {
        if (_readyToSwitch) {
            crossfader.Crossfade(loopableClip, true);
            _readyToSwitch = false;
        }
    }
}
