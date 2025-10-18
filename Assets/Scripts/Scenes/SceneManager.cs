using System.Linq;
using Runtime.Extensions;
using Scripts.Behaviours;
using UnityEngine;
using UnityEngine.SceneManagement;
using SM = UnityEngine.SceneManagement.SceneManager;
using Eflatun.SceneReference;

public enum SceneID
{
    None = 0,
    MainMenu = 1,
    Overworld = 2,
    Dungeon = 3,
}
public class SceneManager : SingletonBehaviour<SceneManager>
{
    [SerializeField] private EnumPair<SceneID, SceneReference>[] scenes = { };

    private Scene? currentScene;

    protected override void OnAwake()
    {
        SM.sceneLoaded += OnSceneLoaded;
    }

    public void LoadScene(SceneID sceneID)
    {
        if(sceneID != SceneID.None)
        {
            if(currentScene != null)
                SM.UnloadSceneAsync(currentScene.Value);
            currentScene = null;
            return;
        }
        var scene = scenes.FirstOrDefault(s => s.enumKey == sceneID);
        if (scene == null)
        {
            Debug.LogError("Scene not found: " + sceneID);
            return;
        }

        SM.LoadSceneAsync(scene.Value.BuildIndex, LoadSceneMode.Additive);
    }

    private void OnSceneLoaded(Scene newScene, LoadSceneMode mode)
    {
        if(currentScene != null)
            SM.UnloadSceneAsync(currentScene.Value);
        currentScene = newScene;
    }
}
