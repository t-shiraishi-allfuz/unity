using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
  // スコア関連
  private int ScoreNum;

  [SerializeField]
  private Text ScoreText;

  // スコア加算
  public void AddScore(int num)
  {
    ScoreNum += num;
    ScoreText.text = string.Format("Score:{0}", ScoreNum);
  }

  // スコアリセット
  public void ResetScore()
  {
    ScoreNum = 0;
    ScoreText.text = string.Format("Score:{0}", ScoreNum);
  }
}
