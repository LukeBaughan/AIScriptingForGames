using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Player playerCharacter;

    // Start is called before the first frame update
    void Start()
    {
        playerCharacter = this.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        playerCharacter.m_Horizontal = Input.GetAxis("Horizontal");
        playerCharacter.m_Vertical = Input.GetAxis("Vertical");
    }
}
