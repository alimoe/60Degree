using UnityEngine;
using System.Collections;
// only work in runtime
public class LevelControl : Core.MonoSingleton<LevelControl> {
    private LevelReader reader;
    public void LoadLevel(int level)
    {
        reader.Load(Board.Instance, "Assets/Resources/Levels/Level" + level + ".xml");
    }

    public void StartTest()
    {
        Board.Instance.autoBirth = false;
        Board.Instance.autoGenerateObstacle = false;
        Board.Instance.autoUpdateGrid = false;
        Board.Instance.autoUpdateWall = false;
        Board.Instance.InitEnviorment();
        reader.Load(Board.Instance, "Assets/Resources/Levels/Temp.xml");
    }

	protected override void Awake () {
        base.Awake();
        reader = new LevelReader();
	}

    public void StartPlay()
    {
        Board.Instance.autoBirth = false;
        Board.Instance.autoGenerateObstacle = false;
        Board.Instance.autoUpdateGrid = false;
        Board.Instance.autoUpdateWall = false;

        Board.Instance.InitEnviorment();
        LoadLevel(1);
    }
	
}
