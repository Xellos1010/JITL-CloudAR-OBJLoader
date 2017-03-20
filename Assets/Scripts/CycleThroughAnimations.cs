using UnityEngine;
using System.Collections;

public class CycleThroughAnimations : MonoBehaviour {
    Animation animationLegacy;
    public string[] animations;
    int animationPlaying;
    public float animationSwitchTime = 5;

    void Awake()
    {
        Initialize();
    }

    void Start()
    {
        Initialize();
    }

    void OnEnable()
    {
        Initialize();
    }

    // Use this for initialization
    public void Initialize()
    {
        animationLegacy = GetComponent<Animation>();
        animationPlaying = 0;
        PlayAnimation();
        StartCoroutine(CycleThroughAnimation());
    }

    void OnDisable()
    {
        StopCoroutine(CycleThroughAnimation());
    }

    IEnumerator CycleThroughAnimation()
    {
        yield return new WaitForSeconds(animationSwitchTime);
        if (animationPlaying + 1 > animations.Length - 1)
            animationPlaying = 0;
        else
            animationPlaying += 1;
        PlayAnimation();
        StartCoroutine(CycleThroughAnimation());
    }

    void PlayAnimation()
    {
        animationLegacy.Play(animations[animationPlaying]);
        animationLegacy.wrapMode = WrapMode.Loop;
    }

    // Update is called once per frame
    void Update () {
	
	}
}
