using BallzGame.Bricks;
using BallzGame.Managers;
using UnityEngine;
using UnityEngine.Serialization;

public class SpecialBrickRush : CrisisBehaviour
{
	[SerializeField] private Brick NormalBrick;
	public override void DoCrisis()
	{
		Debug.Log("DoCrisis");
		GameManager.Instance.BeforeRowSpawn.AddListener(DoSpecialBrickRush);
		currentWave = 0;
	}

	[SerializeField]private int maxWave;
	private int currentWave = 0;
	public void DoSpecialBrickRush()
	{
		currentWave++;
		Debug.Log("DoSpecialBrickRush");
		SpecialBrickWave();
		if (currentWave >= maxWave)
		{
			Debug.Log("Stop");
			currentWave = 0;
			GameManager.Instance.BeforeRowSpawn.RemoveListener(DoSpecialBrickRush);
		}
	}

	public void SpecialBrickWave()
	{
		var spawner = GameManager.Instance.spawner;

		if (spawner == null || NormalBrick == null)
		{
			Debug.LogWarning("CrisisManager: Spawner 或 BlackholeBrick 没有设置！");
			return;
		}

		int width = GameManager.Instance.width;

		spawner.NextBricks.Clear();

		for (int i = 0; i < width; i++)
		{
			spawner.NextBricks.Add(NormalBrick);
			spawner.NextBricksHP.Add(GameManager.Instance.level*2);
		}
	}
}
