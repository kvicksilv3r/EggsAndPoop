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

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (gameContexts.Count > 1)
            {
                Input.backButtonLeavesApp = false;
                GotoPreviousContext();
            }

            else
            {
                Input.backButtonLeavesApp = true;
            }
        }
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

    private void ShowContext(GameContext context)
    {
        EventManager(context, true);

        var affectedObjects = contextMap.Where(c => c.context == context).FirstOrDefault().affectedObjects;

        foreach (var target in affectedObjects)
        {
            target.SetActive(true);
        }

    }

    public void HideContext(GameContext context)
    {
        EventManager(context, false);
        var affectedObjects = contextMap.Where(c => c.context == context).FirstOrDefault().affectedObjects;

        foreach (var target in affectedObjects)
        {
            target.SetActive(false);
        }
    }

    private void EventManager(GameContext context, bool activating)
    {
        switch (currentContext)
        {
            case GameContext.Main:
                break;
            case GameContext.Settings:
                break;
            case GameContext.Home:
                break;
            case GameContext.Egg:
                if (activating)
                {
                    GameManager.Instance.m_EggContextOpened.Invoke();
                }
                else
                {
                    GameManager.Instance.m_EggContextClosed.Invoke();
                }
                break;
            default:
                break;
        }

    }
}