using UnityEngine;
using System.Collections;

public class instructions : MonoBehaviour
{
    public GameObject[] Slides;
    public UnityEngine.UI.HorizontalLayoutGroup ToggleButtonGroup;
    public GameObject ToggleButtonPrefab;
    public bool firstTimeViewer = false;
    public int currentSlide = 0;
    public float sensitivity = 0.10f;
    float prevTouchTime = 0f;
    public SceneObject sceneToLoadOnComplete;
    public bool touchActive = true;
    public Vector2 initialPosition;

    void Awake()
    {
        currentSlide = 0;
        DeactivateAllSlides();
        //CheckToggleButtonsLengthMatchSlides();
        SetActiveSlide();
        touchActive = true;
        /*if (PlayerPrefs.GetString("ViewInstructions") == "false")
        {            
            currentSlide = 0;
            //DeactivateAllSlides();
            //CheckToggleButtonsLengthMatchSlides();
            SetActiveSlide();
        }
        else
            LoadNextScene();*/
    }

    void DeactivateAllSlides()
    {
        foreach(GameObject slide in Slides)
        {
            slide.SetActive(false);
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadIngameScene(sceneToLoadOnComplete);
    }

    public void CheckToggleButtonsLengthMatchSlides()
    {
        if (ToggleButtonGroup.transform.childCount != Slides.Length)
        {
            if (ToggleButtonGroup.transform.childCount <= Slides.Length)
                while (ToggleButtonGroup.transform.childCount < Slides.Length)
                {
                    GameObject toggleButton;
                    UnityEngine.UI.Toggle toggle;
                    toggleButton = (GameObject)Instantiate(ToggleButtonPrefab, ToggleButtonGroup.transform);
                    toggleButton.GetComponent<RectTransform>().localScale = Vector3.one;
                    toggleButton.GetComponent<RectTransform>().localPosition = new Vector3(toggleButton.GetComponent<RectTransform>().localPosition.x, toggleButton.GetComponent<RectTransform>().localPosition.y, 0);
                    toggle = toggleButton.GetComponent<UnityEngine.UI.Toggle>();
                    toggle.group = ToggleButtonGroup.GetComponent<UnityEngine.UI.ToggleGroup>();
                    //toggle.isOn = false;
                }
            else if (ToggleButtonGroup.transform.childCount > Slides.Length)
                for (int i = ToggleButtonGroup.transform.childCount-1; i > Slides.Length-1; i--)
                    DestroyImmediate(ToggleButtonGroup.transform.GetChild(i).gameObject);
        }
        SetActiveToggle();
    }

    public void SetActiveSlide()
    {
        SetActiveSlide(currentSlide);
        SetActiveToggle();
    }

    public void SetActiveSlide(int iSlide)
    {
        currentSlide = iSlide;
        //TODO Add Swipe Transition to transition slide from right or to left
        if (iSlide >= 0 && iSlide < Slides.Length)
        {
            for (int i = 0; i < Slides.Length; i++)
            {
                if (i != iSlide)
                    ToggleSlide(i, false);
                else
                    ToggleSlide(i, true);
            }
        }
        else
        {
            throw new System.Exception("iSlide = " + iSlide + " is not a valid slide number");
        }
        //TODO Activate dot associated with slide
    }

    void ToggleSlide(int iSlide, bool onOff)
    {
        Slides[iSlide].SetActive(onOff);
    }

    void SetActiveToggle()
    {
        SetActiveToggle(currentSlide);
    }

    void SetActiveToggle(int iButton)
    {
        Debug.Log("Setting Active Toggle " + iButton);
        ToggleButtonGroup.GetComponent<UnityEngine.UI.ToggleGroup>().SetAllTogglesOff();
        try
        {
            ToggleButtonGroup.transform.GetChild(iButton).GetComponent<UnityEngine.UI.Toggle>().isOn = true;
        }
        catch
        {
            Debug.Log("Unable to get child " + iButton + " from ToggleButtonGroup");
        }
    }

    void OnTap()
    {
        NextSlide();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            /*if (Time.time > prevTouchTime + sensitivity)
            {
                prevTouchTime = Time.time;
                currentSlide++;
            }*/
            if (!touchActive)
            {
                initialPosition = Input.mousePosition;
                touchActive = true;
            }
            //NextSlide();
        }

        else if (Input.GetMouseButtonUp(0))
        {
            CalculateSwipeDirection(Input.mousePosition);
            touchActive = false;
            ResetInitialPos();
        }

        if (Input.GetMouseButtonDown(1))
        {
            PreviousSlide();
        }
        try
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                touchActive = true;
                initialPosition = Input.GetTouch(0).position;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (touchActive)
                {
                    CalculateSwipeDirection(Input.GetTouch(0).position);
                }
            }
        }
        catch
        {
            Debug.Log("No touch detected");
        }
        
        /*if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            NextSlide();
        }
        if (Input.touchCount > 1 && Input.GetTouch(1).phase == TouchPhase.Began)
        {
            PreviousSlide();
        }*/
    }

    /*void OnMouseDown()
    {
        touchActive = true;
        initialPosition = Input.mousePosition;
    }

    void OnMouseUp()
    {
        CalculateSwipeDirection(Input.mousePosition);
        ResetInitialPos();
    }*/

    void CalculateSwipeDirection(Vector2 pos)
    {
        if (initialPosition.x - pos.x < 0)
        {
            touchActive = false;
            PreviousSlide();
            ResetInitialPos();
        }
        else if (initialPosition.x - pos.x > 0)
        {
            touchActive = false;
            NextSlide();
            ResetInitialPos();
        }
    }

    void ResetInitialPos()
    {
        initialPosition = new Vector2();
    }

    public void NextSlide()
    {
        ++currentSlide;        
        if (currentSlide > Slides.Length-1)
        {
            currentSlide = Slides.Length-1;
            ToggleButtonGroup.gameObject.SetActive(false);
            SlideComplete();
        }
        else
        {
            SetActiveSlide();
        }
    }

    public void SlideComplete()
    {
        //TODO Disable Object and Save as tutorial Done
        PlayerPrefs.SetString("SlideComplete", "true");
        LoadNextScene();
    }

    public void PreviousSlide()
    {
        --currentSlide;
        if (currentSlide < 0)
        {
            currentSlide = 0;
            //Do nothing
        }
        else
        {
            SetActiveSlide();
        }
    }
}