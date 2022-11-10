using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  // 各変数
  Spawner spawner;
  Block activeBlock;
  Board board;
  Score score;
  AudioSource audioSource;

  // キー入力受け付けタイマー
  float nextKeyDownTimer, nextKeyLeftRightTimer, nextKeyRotateTimer;

  // キー入力インターバル
  [SerializeField]
  private float nextKeyDownInterval, nextKeyLeftRightInterval, nextKeyRotateInterval;

  // minoのドロップタイマー
  [SerializeField]
  private float dropInterval = 0.25f;
  float nextdropTimer;

  // GameOverPanelを格納する変数
  [SerializeField]
  private GameObject gameOverPanel;

  // GameOver判定
  bool gameOver;

  // sound関連
  public AudioClip SE_001, SE_003, SE_004;

  private void Start()
  {
    // 変数にオブジェクトを代入
    spawner = GameObject.FindObjectOfType<Spawner>();
    board = GameObject.FindObjectOfType<Board>();
    score = GameObject.FindObjectOfType<Score>();
    audioSource = GetComponent<AudioSource>();

    spawner.transform.position = Rounding.Round(spawner.transform.position);

    // タイマー初期化
    nextKeyDownTimer = Time.time + nextKeyDownInterval;
    nextKeyLeftRightTimer = Time.time + nextKeyLeftRightInterval;
    nextKeyRotateTimer = Time.time + nextKeyRotateInterval;

    // スコア初期化
    score.ResetScore();

    // ランダムで取得したminoを変数に反映
    if (!activeBlock) {
      activeBlock = spawner.SpawnBlock();
    }

    // GameOverPanelがアクティブな場合非表示にする
    if (gameOverPanel.activeInHierarchy) {
      gameOverPanel.SetActive(false);
    }
  }

  private void Update()
  {
    // GameOver判定
    if (gameOver) {
      return;
    }

    KeyInput();
  }

  // キー入力を受け取ってminoを動かす
  void KeyInput()
  {
    // 右矢印で右移動
    if (Input.GetKey(KeyCode.RightArrow) && (Time.time > nextKeyLeftRightTimer)
      || Input.GetKeyDown(KeyCode.RightArrow))
    {
      // SE再生
      audioSource.PlayOneShot(SE_001);

      activeBlock.MoveRight();

      nextKeyLeftRightTimer = Time.time + nextKeyLeftRightInterval;

      // ポジション判定（NGなら逆方向に戻す）
      if (!board.CheckPosition(activeBlock))
      {
        activeBlock.MoveLeft();
      }
    }
    // 左矢印で左移動
    else if (Input.GetKey(KeyCode.LeftArrow) && (Time.time > nextKeyLeftRightTimer)
      || Input.GetKeyDown(KeyCode.LeftArrow))
    {
      // SE再生
      audioSource.PlayOneShot(SE_001);

      activeBlock.MoveLeft();

      nextKeyLeftRightTimer = Time.time + nextKeyLeftRightInterval;

      // ポジション判定（NGなら逆方向に戻す）
      if (!board.CheckPosition(activeBlock))
      {
        activeBlock.MoveRight();
      }
    }
    // Spaceで右回転
    else if (Input.GetKey(KeyCode.Space) && (Time.time > nextKeyRotateTimer)
      || Input.GetKeyDown(KeyCode.Space))
    {
      // SE再生
      audioSource.PlayOneShot(SE_001);

      activeBlock.RotateRight();

      nextKeyRotateTimer = Time.time + nextKeyRotateInterval;

      // ポジション判定（NGなら逆方向に戻す）
      if (!board.CheckPosition(activeBlock))
      {
        activeBlock.RotateLeft();
      }
    }
    // 自動ダウンしながら下矢印でもダウン
    else if (Input.GetKey(KeyCode.DownArrow) && (Time.time > nextKeyDownTimer)
      || (Time.time > nextdropTimer))
    {
      // 矢印押してる時だけSE再生
      if (Input.GetKey(KeyCode.DownArrow)) {
        // SE再生
        audioSource.PlayOneShot(SE_001);
      }

      activeBlock.MoveDown();

      nextKeyDownTimer = Time.time + nextKeyDownInterval;
      nextdropTimer = Time.time + dropInterval;

      // ポジション判定（NGなら逆方向に戻す）
      if (!board.CheckPosition(activeBlock))
      {
        // 上まで積み上がったらGameOver
        if (board.OverLimit(activeBlock)) {
          GameOver();
        }
        else
        {
          BottomBoard();
        }
      }
    }
  }

  // 下についた時の処理
  void BottomBoard()
  {
    // 戻す
    activeBlock.MoveUp();

    // 最終的なポジションをgridに記憶
    board.SaveBlockInGrid(activeBlock);

    // 次のmino呼び出し
    activeBlock = spawner.SpawnBlock();

    // タイマー初期化
    nextKeyDownTimer = Time.time;
    nextKeyLeftRightTimer = Time.time;
    nextKeyRotateTimer = Time.time;

    // minoが埋まった行を削除
    board.ClearBlock();
  }

  // ゲームオーバー
  void GameOver()
  {
    // SE再生
    audioSource.PlayOneShot(SE_003);

    // 戻す
    activeBlock.MoveUp();

    // GameOverPanelが非アクティブな場合表示する
    if (!gameOverPanel.activeInHierarchy) {
      gameOverPanel.SetActive(true);
    }

    gameOver = true;
  }

  // Retry
  public void Restart()
  {
    // SE再生
    audioSource.PlayOneShot(SE_004);

    // SEを再生した後にロード
    StartCoroutine("LoadGameScene", 0);
  }

  // シーンロード（遅延）
  IEnumerator LoadGameScene(int stageNo)
  {
    yield return new WaitForSeconds(1.0f);
    SceneManager.LoadScene(stageNo);
  }
}
