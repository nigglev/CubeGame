using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class SceneEditor : EditorWindow
{
    private GUIStyle _editor_style = new GUIStyle();
    string _scene_name = "Default Name";
    CFileManager _fm;


    [MenuItem("Scene Editor/Safe and Load Scene Menu")]
    static void Init()
    {
        var window = (SceneEditor)EditorWindow.GetWindow(typeof(SceneEditor), false, "Safe and Load Scene Menu");
        window.Show();
        
    }

    private void OnGUI()
    {
        BaseUI();
    }

    private void BaseUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Scene Name");
        _scene_name = GUILayout.TextField(_scene_name);
        _fm = new CFileManager();
        if (GUILayout.Button("Save"))
        {
            SaveObjects();
        }

        if (GUILayout.Button("Load"))
        {
            LoadObjects();
        }

        GUILayout.EndHorizontal();
    }

    private void SaveObjects()
    {
        CSceneSafeLoad new_scene_to_save = new CSceneSafeLoad(_scene_name);
        var objects_to_save = GameObject.FindGameObjectsWithTag("ToSave");
        var number_of_objects = objects_to_save.Length;

        for(int i = 0; i < number_of_objects; i++)
        {
            Transform tr = objects_to_save[i].transform;
            SRVector coords = new SRVector(tr.position, tr.rotation);

            if (objects_to_save[i].GetComponent<PlayerController>() != null)
                new_scene_to_save.AddPlayer("Player", coords);

            if (objects_to_save[i].GetComponent<CoinController>() != null)
                new_scene_to_save.AddCoin("Coin", coords);

            if (objects_to_save[i].GetComponent<GoalController>() != null)
                new_scene_to_save.AddGoal("Goal", coords);

            if (objects_to_save[i].GetComponent<EnemyController>() != null)
                new_scene_to_save.AddEnemy("Enemy", coords);

            if (objects_to_save[i].GetComponent<PlatformController>() != null)
                new_scene_to_save.AddPlatform("Platform", coords);
        }

        _fm.JsonToFile(new_scene_to_save, _scene_name);
    }

    private void LoadObjects()
    {
        string path = Application.dataPath + "/StreamingAssets/" + _scene_name + ".txt";

        CSceneSafeLoad loaded_scene = _fm.ReadJsonFromFile<CSceneSafeLoad>(path);

        for (int i = 0; i < loaded_scene.Goals.Length; i++)
        {
            string prefab_path_goal = loaded_scene.Goals[i].Prefab_path;
            float x_goal = loaded_scene.Goals[i].X;
            float y_goal = loaded_scene.Goals[i].Y;
            float z_goal = loaded_scene.Goals[i].Z;
            UnityEngine.Object prefab_goal = Resources.Load<UnityEngine.Object>(prefab_path_goal);
            GameObject new_game_object_goal = (GameObject)GameObject.Instantiate(prefab_goal, new Vector3(x_goal, y_goal, z_goal), Quaternion.identity);
        }

        string prefab_path_player = loaded_scene.Player.Prefab_path;
        float x_player = loaded_scene.Player.X;
        float y_player = loaded_scene.Player.Y;
        float z_player = loaded_scene.Player.Z;
        UnityEngine.Object prefab_player = Resources.Load<UnityEngine.Object>(prefab_path_player);
        GameObject new_game_object = (GameObject)GameObject.Instantiate(prefab_player, new Vector3(x_player, y_player, z_player), Quaternion.identity);

        for (int i = 0; i < loaded_scene.Coins.Length; i++)
        {
            string prefab_path_coin = loaded_scene.Coins[i].Prefab_path;
            float x_coin = loaded_scene.Coins[i].X;
            float y_coin = loaded_scene.Coins[i].Y;
            float z_coin = loaded_scene.Coins[i].Z;
            UnityEngine.Object prefab_coin = Resources.Load<UnityEngine.Object>(prefab_path_coin);
            GameObject new_game_object_coin = (GameObject)GameObject.Instantiate(prefab_coin, new Vector3(x_coin, y_coin, z_coin), Quaternion.Euler(90f, 0f, 0f));
        }

        for (int i = 0; i < loaded_scene.Enemies.Length; i++)
        {
            string prefab_path_enemy = loaded_scene.Enemies[i].Prefab_path;
            float x_enemy = loaded_scene.Enemies[i].X;
            float y_enemy = loaded_scene.Enemies[i].Y;
            float z_enemy = loaded_scene.Enemies[i].Z;
            UnityEngine.Object prefab_enemy = Resources.Load<UnityEngine.Object>(prefab_path_enemy);
            GameObject new_game_object_enemy = (GameObject)GameObject.Instantiate(prefab_enemy, new Vector3(x_enemy, y_enemy, z_enemy), Quaternion.identity);
        }

        for (int i = 0; i < loaded_scene.Platforms.Length; i++)
        {
            string prefab_path_platform = loaded_scene.Platforms[i].Prefab_path;
            float x_plat = loaded_scene.Platforms[i].X;
            float y_plat = loaded_scene.Platforms[i].Y;
            float z_plat = loaded_scene.Platforms[i].Z;
            UnityEngine.Object prefab_plat = Resources.Load<UnityEngine.Object>(prefab_path_platform);
            GameObject new_game_object_platform = (GameObject)GameObject.Instantiate(prefab_plat, new Vector3(x_plat, y_plat, z_plat), Quaternion.identity);
        }

    }
}
