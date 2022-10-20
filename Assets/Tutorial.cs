using UnityEngine;
using TMPro;
using System.Collections;

public class Tutorial : MonoBehaviour
{
    [Header("Welcome")]
    public TextMeshProUGUI welcome;
    bool welcomed = false;
    [Header("Movement")]
    public TextMeshProUGUI movementIntro;
    public TextMeshProUGUI sprint;
    public TextMeshProUGUI jump;
    bool inputZ = false;
    bool inputQ = false;
    bool inputS = false;
    bool inputD = false;
    bool movementIntrodone = false;
    bool jumpinput = false;
    bool jumpdone = false;
    bool sprintdone = false;
    bool sprintinput = false;


    [Header("Game Mechanic")]
    public TextMeshProUGUI winCondition;
    public TextMeshProUGUI capturing;
    public TextMeshProUGUI fire;
    public TextMeshProUGUI aim;
    public TextMeshProUGUI kill;
    public GameObject capturearea;
    public GameObject virusToKill;
    bool capturingdone = false;
    bool captureisInstantiate = false;

    [Header("Finish")]
    public TextMeshProUGUI end;
    
    
    private GameManager gameManager;
    private int scientistScore;
    private float timer = 0f;
    private bool winConditiondone;
    private bool firedone;
    private bool aimdone;
    private int scientistkill;
    private bool killdone;
    public Target target;
    public bool targetisKilled = false;

    void Awake()
    {
        welcome.enabled = false;
        movementIntro.enabled = false;
        sprint.enabled = false;
        jump.enabled = false;
        winCondition.enabled = false;
        capturing.enabled = false;
        fire.enabled = false;
        aim.enabled = false;
        kill.enabled = false;
        end.enabled = false;
        gameManager = GameManager.Instance;
        scientistScore = gameManager.nbContaminatedAreaByScientist;
        target = virusToKill.GetComponent<Target>();
        
        

    }

    private void Start()
    {
        welcome.enabled = true;
    }


    void Update()
    {
        scientistScore = gameManager.nbContaminatedAreaByScientist;
        
        timer += Time.deltaTime;

        if(timer > 5 && !welcomed)
        {
            welcome.enabled = false;
            welcomed = true;
            
            
        }

        if(welcomed && !movementIntrodone)
        { 
          MovementIntro();
        }

        if(movementIntrodone && !sprintdone)
        {
            Sprint();
        }

        if(sprintdone && !jumpdone)
        {
            Jump();
        }

        if(jumpdone && !winConditiondone)
        {
            WinCondition(timer);
        }
        
        if (winConditiondone && !capturingdone)
        {
            Capturing();
        }
        if(capturingdone && !firedone)
        {
            Fire();
        }
        if(firedone && !aimdone)
        {
            Aim();
        }
        if(aimdone && !killdone)
        {
            Kill();
        }

        if (killdone)
            end.enabled = true;
          
                
    }


    void MovementIntro ()
    {
        
        movementIntro.enabled = true;
        
            if (Input.GetKey(KeyCode.Z))
            {
                inputZ = true;
                Debug.Log("Input z");
            }
            if (Input.GetKey(KeyCode.Q))
            {
                inputQ = true;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                inputS = true;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                inputD = true;
            }
            if (inputZ && inputD && inputQ && inputS)
            {
                movementIntro.enabled = false;
                movementIntrodone = true;
                

        }
      


    }

    void Sprint()
    {
        sprint.enabled = true;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            sprint.enabled = false;
            sprintdone = true;
        }
    }

    void Jump()
    {
        jump.enabled = true;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpdone = true;
            jump.enabled = false;
            timer = 0f;
        }
    }
    
    void WinCondition(float time)
    {
        winCondition.enabled = true;
        if(time > 5)
        {
            winConditiondone = true;
            winCondition.enabled = false;
        }
    }

    void Capturing()
    {
        capturing.enabled = true;
     

        if(scientistScore >= 1)
        {
            capturing.enabled = false;
            capturingdone = true;
        }
    
    }

    void Fire()
    {
        fire.enabled = true;

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            fire.enabled = false;
            firedone = true;
        }

    }

    void Aim()
    {
        aim.enabled = true;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            aim.enabled = false;
            aimdone = true;
        }

    }

    void Kill()
    {
        kill.enabled = true;
        virusToKill.SetActive(true);
        if(targetisKilled)
        {
            kill.enabled = false;
            killdone = true;
        }
    }


   public void targetIsKilled()
    {

        targetisKilled = true;
    }


}
