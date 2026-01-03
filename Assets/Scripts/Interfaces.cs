using UnityEngine;

public interface IMoveInput
{
    Vector2 GetMoveInput();
}
public interface IAimInput
{
    Vector2 GetAimInput();
    bool GetHandlePressed();
}




public interface IDamagable 

{ 
    void TakeDamage(float amount); 
}
public interface IKnockBackable 
{ 
    void ApplyKnockBack(Vector2 direction, float force); 
}

