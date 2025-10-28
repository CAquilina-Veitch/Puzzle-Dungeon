using UnityEngine;

public class Weapon : MonoBehaviour
{

    public MeshRenderer mesh;
    public BoxCollider hbox;
    public bool attacking;
   public void attack()
    {
        attacking = true;
        mesh.enabled = true;
        hbox.enabled = true;
        Invoke("endAttack", 0.2f);
    }
    public void endAttack()
    {
        attacking = false;
        mesh.enabled = false;
        hbox.enabled = false;    
    }
    void Start()
    {
        hbox = GetComponent<BoxCollider>();
        mesh = GetComponent<MeshRenderer>();
        
        
        attacking = false;
        mesh.enabled = false;
        hbox.enabled = false;
    }

 
    void Update()
    {
        
    }
}
