using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatActionInvoker
{
    public void ExecuteCommand(ICombatActionCommand command)
    {
        command.ExecuteAction();
    }
}
