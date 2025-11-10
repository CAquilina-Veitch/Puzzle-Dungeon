using System.Linq;
using Runtime.Extensions;
using Scripts.Behaviours;
using UnityEngine;
using UnityEngine.SceneManagement;
using SM = UnityEngine.SceneManagement.SceneManager;
using Eflatun.SceneReference;

public class SceneManager : SingletonBehaviour<SceneManager>
{
    [SerializeField] private EnumPair<SceneID, SceneReference>[] scenes = { };

    public readonly RORP<Scene?> CurrentScene = new(null);
    
    protected override void OnAwake()
    {
        SM.sceneLoaded += OnSceneLoaded;
    }

    public void LoadScene(SceneID sceneID)
    {
        if(sceneID != SceneID.None)
        {
            if(CurrentScene.Get != null)
                SM.UnloadSceneAsync(CurrentScene.Get.Value);
            CurrentScene.NewValue = null;
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
        if(CurrentScene.Get != null)
            SM.UnloadSceneAsync(CurrentScene.Get.Value);
        CurrentScene.NewValue = newScene;
    }
}
