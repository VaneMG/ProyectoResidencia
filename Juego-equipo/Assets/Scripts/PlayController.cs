using UnityEngine;

public class PlayController : MonoBehaviour
{
    // Crear variables para mover al jugador
    bool isLeft = false;
    bool isRigth = false;
    bool isJump = false;
    bool canJump = true;


    public float speedForce;
    public float jumpForce;
    public Rigidbody2D rPlayer;
    public float waitJump;
    public Animator anim;
    public SpriteRenderer spr;

    // Metodo para detectar si lo pulso izquierdo
    public void clickLeft()
    {
        isLeft = true;
        anim.SetTrigger("run");
        spr.flipX = true; // Para que volve a direccion correspondiente ya sea atras o enfrente
    }
    
    public void relaseLeft()  //Detecta si se solto el botón
    {
        isLeft = false;
        anim.SetTrigger("Ide");
    }

    // Metodo para detectar si pulso el derecho
    public void clickRigth()
    {
        isRigth = true;
        anim.SetTrigger("run");
        spr.flipX = false;
    }

    public void relaseRigth()
    {
        isRigth = false;
        anim.SetTrigger("Ide");
    }

    // Metodo para saltar
    public void clickJump()
    {
        isJump = true;
        anim.SetTrigger("jump");
    }


    private void FixedUpdate()
    {
        if (isLeft)
        {
            rPlayer.AddForce(new Vector2(-speedForce, 0) * Time.deltaTime);
        }

        if (isRigth)
        {
            rPlayer.AddForce(new Vector2(speedForce, 0) * Time.deltaTime);
        }

        if (isJump && canJump)
        {
            isJump = false; // Para que no salte infinatamente
            rPlayer.AddForce(new Vector2(0, jumpForce));
            canJump = false;
            Invoke("waitToJump", waitJump);
        }
    }

    void waitToJump()
    {
        canJump = true;
        //anim.SetTrigger("jump");
    }

}

