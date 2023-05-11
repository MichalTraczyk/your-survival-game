using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
public class PlayerInteract : MonoBehaviour
{

    [SerializeField] private LayerMask layers;

    private StarterAssetsInputs input;
    private Interactable currInteract;
    private void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
    }
    // Update is called once per frame
    void Update()
    {
        if(input.escape)
        {
            GameManager.Instance.PauseClicked();
            input.escape = false;

        }
        if(currInteract != null)
            UIManager.Instance.UpdateINteractText(currInteract.Message);
        else
            UIManager.Instance.UpdateINteractText("");

        Vector2 shotPos = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(shotPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 5,layers))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();
            currInteract = interactable;
        }
        else
        {
            currInteract = null;
        }
        if (input.interact)
        {
            if(currInteract!=null)
                currInteract.Invoke();
            input.interact = false;
            //GameManager.Instance.EnableCursor();
            //input.cursorLocked = false;
        }
    }
}
