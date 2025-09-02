using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ContextMap
{
    public GameContext context;
    public GameObject[] affectedObjects;
}

public class GameStateController : MonoBehaviour
{
    private GameContext currentContext;
    private Stack<GameContext> gameContexts = new Stack<GameContext>();
    public List<ContextMap> contextMap = new List<ContextMap>();

    void Start()
    {
        currentContext = GameContext.Main;
        gameContexts.Push(currentContext);
    }

    public void OpenEgg()
    {
        SetContext(GameContext.Egg);
    }

    public void SetContext(GameContext context)
    {
        if (context == currentContext)
        {
            print("Context is already active");
            return;
        }

        HideContext(currentContext);
        gameContexts.Push(context);
        currentContext = context;
        ShowContext(currentContext);
    }

    public void GotoPreviousContext()
    {
        if (gameContexts.Count > 1)
        {
            HideContext(gameContexts.Pop());
            currentContext = gameContexts.Peek();
            ShowContext(currentContext);
        }

        else
        {
            print("No context to pop");
        }
    }

    private void ShowContext(GameContext currentContext)
    {
        var affectedObjects = contextMap.Where(c => c.context == currentContext).FirstOrDefault().affectedObjects;

        foreach (var target in affectedObjects)
        {
            target.SetActive(true);
        }
    }

    public void HideContext(GameContext context)
    {
        var affectedObjects = contextMap.Where(c => c.context == context).FirstOrDefault().affectedObjects;

        foreach (var target in affectedObjects)
        {
            target.SetActive(false);
        }
    }
}
