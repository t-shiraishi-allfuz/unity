using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
  // スコア変数
  Score score;
  AudioSource audioSource;

  // minoのポジションを記憶する変数
  private Transform[,] grid;

  // prefabのemptyを格納する変数
  [SerializeField]
  private Transform emptySprite;

  // ボードのサイズ指定用変数
  [SerializeField]
  private int height = 30, width = 10, header = 8;

  // スコア関連
  // 1行消して加算するスコア
  private int addScore = 100;

  // sound関連
  public AudioClip SE_002;

  private void Awake()
  {
    grid = new Transform[width, height];
  }

  private void Start()
  {
    CreateBoard();
  }

  // ボード生成
  void CreateBoard()
  {
    if (emptySprite)
    {
      for (int y = 0; y < height - header; y++)
      {
        for (int x = 0; x < width; x++)
        {
          Transform clone = Instantiate(emptySprite,
            new Vector3(x, y, 0), Quaternion.identity);

          clone.transform.parent = transform;
        }
      }
    }
  }

  // ボードの下にminoが着いたか判定
  public bool CheckPosition(Block block)
  {
    foreach (Transform item in block.transform)
    {
      Vector2 pos = Rounding.Round(item.position);

      // 枠内判定
      if (!BoardOutCheck((int)pos.x, (int)pos.y))
      {
        return false;
      }

      // mino判定
      if (BlockCheck((int)pos.x, (int)pos.y, block))
      {
        return false;
      }
    }
    return true;
  }

  // ステージ内判定
  bool BoardOutCheck(int x, int y)
  {
    // x軸は0以上かつwidth以内なら枠内
    // y軸は0以上
    return (x >= 0 && x < width && y >= 0);
  }

  // mino判定
  bool BlockCheck(int x, int y, Block block)
  {
    // 移動先にminoがないか
    // gridが空ではない場合はminoがある状態
    // gridの親が違うのは他のminoがある状態
    return (grid[x, y] != null && grid[x, y].parent != block.transform);
  }

  // minoのポジションを記憶する
  public void SaveBlockInGrid(Block block)
  {
    foreach (Transform item in block.transform)
    {
      Vector2 pos = Rounding.Round(item.position);

      grid[(int)pos.x, (int)pos.y] = item;
    }
  }

  // 全行チェックしてminoが埋まっていれば消す処理
  public void ClearBlock()
  {
    score = GameObject.FindObjectOfType<Score>();
    audioSource = GetComponent<AudioSource>();

    for (int y = 0; y < height; y++)
    {
      if (IsComplete(y))
      {
        ClearRow(y);
        ShiftRowsDown(y + 1);
        y--;

        // スコア加算
        score.AddScore(addScore);
        // SE再生
        audioSource.PlayOneShot(SE_002);
      }
    }
  }

  // 指定の行をチェックしてgridに空があれば埋まってない判定
  bool IsComplete(int y)
  {
    for (int x = 0; x < width; x++)
    {
      if (grid[x, y] == null)
      {
        return false;
      }
    }
    return true;
  }

  // 指定の行を削除
  void ClearRow(int y)
  {
    for (int x = 0; x < width; x++)
    {
      if (grid[x, y] != null)
      {
        Destroy(grid[x, y].gameObject);
      }
      grid[x, y] = null;
    }
  }

  // 消した行の1つ上の行を1行下げる
  void ShiftRowsDown(int startY)
  {
    for (int y = startY; y < height; y++)
    {
      for (int x = 0; x < width; x++) {
        if (grid[x, y] != null)
        {
          grid[x, y - 1] = grid[x, y];
          grid[x, y] = null;
          grid[x, y - 1].position += new Vector3(0, -1, 0);
        }
      }
    }
  }

  // minoが積み上がった判定
  public bool OverLimit(Block block)
  {
    foreach (Transform item in block.transform)
    {
      if (item.transform.position.y >= height - header) {
        return true;
      }
    }
    return false;
  }
}
