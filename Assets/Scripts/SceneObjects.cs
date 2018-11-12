using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


[JsonObject(MemberSerialization.Fields)]
public class CSceneSafeLoad
{
    
    [JsonProperty("Scene Name")]
    string _scene_name;
    [JsonProperty("Player")]
    CScenePlayerObjects _player;
    [JsonProperty("Coins")]
    List<CSceneCoinObjects> _coins_list;
    [JsonProperty("Goals")]
    List<CSceneGoalObjects> _goals_list;
    [JsonProperty("Enemies")]
    List<CSceneEnemyObjects> _enemies_list;
    [JsonProperty("Platforms")]
    List<CScenePlatformObjects> _platforms_list;

    public CSceneSafeLoad(string in_scene_name)
    {
        _scene_name = in_scene_name;
        
        _coins_list = new List<CSceneCoinObjects>();
        _goals_list = new List<CSceneGoalObjects>();
        _enemies_list = new List<CSceneEnemyObjects>();
        _platforms_list = new List<CScenePlatformObjects>();
    }


    public CSceneGoalObjects[] Goals { get { return _goals_list.ToArray(); } }
    public CScenePlayerObjects Player { get { return _player; } }
    public CSceneCoinObjects[] Coins { get { return _coins_list.ToArray(); } }
    public CSceneEnemyObjects[] Enemies { get { return _enemies_list.ToArray(); } }
    public CScenePlatformObjects[] Platforms { get { return _platforms_list.ToArray(); } }



    public void AddPlayer(string in_prefab_path, SRVector in_coord)
    {
        _player = new CScenePlayerObjects
        {
            Prefab_path = in_prefab_path,
            X = in_coord.position.x,
            Y = in_coord.position.y,
            Z = in_coord.position.z
        };
    }

    public void AddCoin(string in_prefab_path, SRVector in_coord)
    {
        CSceneCoinObjects new_coin = new CSceneCoinObjects();
        new_coin.Prefab_path = in_prefab_path;
        new_coin.X = in_coord.position.x;
        new_coin.Y = in_coord.position.y;
        new_coin.Z = in_coord.position.z;

        _coins_list.Add(new_coin);
    }

    public void AddGoal(string in_prefab_path, SRVector in_coord)
    {
        CSceneGoalObjects new_goal = new CSceneGoalObjects();
        new_goal.Prefab_path = in_prefab_path;
        new_goal.X = in_coord.position.x;
        new_goal.Y = in_coord.position.y;
        new_goal.Z = in_coord.position.z;

        _goals_list.Add(new_goal);
    }

    public void AddEnemy(string in_prefab_path, SRVector in_coord)
    {
        CSceneEnemyObjects new_enemy = new CSceneEnemyObjects();
        new_enemy.Prefab_path = in_prefab_path;
        new_enemy.X = in_coord.position.x;
        new_enemy.Y = in_coord.position.y;
        new_enemy.Z = in_coord.position.z;

        _enemies_list.Add(new_enemy);
    }

    public void AddPlatform(string in_prefab_path, SRVector in_coord)
    {
        CScenePlatformObjects new_platform = new CScenePlatformObjects();
        new_platform.Prefab_path = in_prefab_path;
        new_platform.X = in_coord.position.x;
        new_platform.Y = in_coord.position.y;
        new_platform.Z = in_coord.position.z;

        _platforms_list.Add(new_platform);
    }

    
}

[JsonObject(MemberSerialization.Fields)]
public class CSceneGoalObjects
{
    [JsonProperty("Prefab Path")]
    string _prefab_path;

    [JsonProperty("Coordinate X")]
    float _coord_x;
    [JsonProperty("Coordinate Y")]
    float _coord_y;
    [JsonProperty("Coordinate Z")]
    float _coord_z;


    public string Prefab_path { get { return _prefab_path; } set { _prefab_path = value; } }
    public float X { get { return _coord_x; } set { _coord_x = value; } }
    public float Y { get { return _coord_y; } set { _coord_y = value; } }
    public float Z { get { return _coord_z; } set { _coord_z = value; } }
}

[JsonObject(MemberSerialization.Fields)]
public class CScenePlayerObjects
{
    [JsonProperty("Prefab Path")]
    string _prefab_path;

    [JsonProperty("Coordinate X")]
    float _coord_x;
    [JsonProperty("Coordinate Y")]
    float _coord_y;
    [JsonProperty("Coordinate Z")]
    float _coord_z;


    public string Prefab_path { get { return _prefab_path; } set { _prefab_path = value; } }
    public float X { get { return _coord_x; } set { _coord_x = value; } }
    public float Y { get { return _coord_y; } set { _coord_y = value; } }
    public float Z { get { return _coord_z; } set { _coord_z = value; } }
}

[JsonObject(MemberSerialization.Fields)]
public class CSceneCoinObjects
{
    [JsonProperty("Prefab Path")]
    string _prefab_path;

    [JsonProperty("Coordinate X")]
    float _coord_x;
    [JsonProperty("Coordinate Y")]
    float _coord_y;
    [JsonProperty("Coordinate Z")]
    float _coord_z;


    public string Prefab_path { get { return _prefab_path; } set { _prefab_path = value; } }
    public float X { get { return _coord_x; } set { _coord_x = value; } }
    public float Y { get { return _coord_y; } set { _coord_y = value; } }
    public float Z { get { return _coord_z; } set { _coord_z = value; } }
}

[JsonObject(MemberSerialization.Fields)]
public class CSceneEnemyObjects
{
    [JsonProperty("Prefab Path")]
    string _prefab_path;

    [JsonProperty("Coordinate X")]
    float _coord_x;
    [JsonProperty("Coordinate Y")]
    float _coord_y;
    [JsonProperty("Coordinate Z")]
    float _coord_z;


    public string Prefab_path { get { return _prefab_path; } set { _prefab_path = value; } }
    public float X { get { return _coord_x; } set { _coord_x = value; } }
    public float Y { get { return _coord_y; } set { _coord_y = value; } }
    public float Z { get { return _coord_z; } set { _coord_z = value; } }
}

[JsonObject(MemberSerialization.Fields)]
public class CScenePlatformObjects
{
    [JsonProperty("Prefab Path")]
    string _prefab_path;

    [JsonProperty("Coordinate X")]
    float _coord_x;
    [JsonProperty("Coordinate Y")]
    float _coord_y;
    [JsonProperty("Coordinate Z")]
    float _coord_z;


    public string Prefab_path { get { return _prefab_path; } set { _prefab_path = value; } }
    public float X { get { return _coord_x; } set { _coord_x = value; } }
    public float Y { get { return _coord_y; } set { _coord_y = value; } }
    public float Z { get { return _coord_z; } set { _coord_z = value; } }
}

