using UnityEngine;

public abstract class PlayerState
{
    protected NewPlayerController controller;

    public PlayerState(NewPlayerController controller)
    {
        this.controller = controller;
    }

    public virtual void Enter() 
    { 
    
    }

    public virtual void Exit() 
    { 
    
    }

    public virtual void Update() 
    { 

    }

    public virtual void HandleInput() 
    {

    }
}