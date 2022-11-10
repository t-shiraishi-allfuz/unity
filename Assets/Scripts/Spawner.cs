using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
  // minoを格納する配列
  [SerializeField]
  Block[] Blocks;

  // 7つのminoから1つをランダムで取得する
  Block GetRandomBlock()
  {
    int i = Random.Range(0, Blocks.Length);

    if (Blocks[i]) {
      return Blocks[i];
    }
    else
    {
      return null;
    }
  }

  // 取得したminoをシーンに反映
  public Block SpawnBlock()
  {
    Block block = Instantiate(GetRandomBlock(),
      transform.position, Quaternion.identity);

    if (block) {
      return block;
    }
    else
    {
      return null;
    }
  }
}
