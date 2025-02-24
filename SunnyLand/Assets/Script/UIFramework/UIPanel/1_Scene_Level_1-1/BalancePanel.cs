﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class BalancePanel : BasePanel
{
    private Text score;
    private Text reward;
    private Text money;
    private Image[] star = new Image[3];

    protected override void Awake()
    {
        base.Awake();
        score = transform.Find("ScorePanel/Score").GetComponent<Text>();
        reward = transform.Find("RewardPanel/Reward").GetComponent<Text>();
        money = transform.Find("MoneyPanel/Money").GetComponent<Text>();
        money.text = 0.ToString();

        for (int i = 0; i < star.Length; i++)
        {
            star[i] = transform.Find("StarPanel/Star" + (i + 1) + "/star").GetComponent<Image>();
            star[i].gameObject.SetActive(false);
        }

        ctrl.Model.OnBalance += OnBalance;
    }

    private void OnBalance(int score, int specialCoin)
    {
        StartCoroutine(ScoreAnim(score, this.score));
        StartCoroutine(ScoreAnim(score, reward, true));
        StartCoroutine(StarAnim(specialCoin));
    }

    #region Coroutine Animation

    private IEnumerator ScoreAnim(int score, Text text, bool isStartMoneyAnim = false)
    {
        for (int i = 0; i < score + 1; i += 10)
        {
            text.text = i.ToString();

            yield return new WaitForSeconds(0.01f);
        }

        if (isStartMoneyAnim)
        {
            StartCoroutine(MoneyAnim(score / 10));
        }

        StopCoroutine(ScoreAnim(score, text, isStartMoneyAnim));
    }

    private IEnumerator MoneyAnim(int addMoney)
    {
        for (int i = 1; i <= addMoney; i += 1)
        {
            GameRecord.Instance.money += 1;
            money.text = i.ToString();
            yield return new WaitForSeconds(0.01f);
        }

        StopCoroutine(MoneyAnim(addMoney));
    }

    private IEnumerator StarAnim(int specialCoin)
    {
        for (int i = 0; i < star.Length; i++)
        {
            if (i < specialCoin)
            {
                star[i].gameObject.SetActive(true);
                star[i].transform.DOScale(Vector3.one, 0.25f);
                ctrl.audioManager.Play(ctrl.audioManager.GetStar, ctrl.audioSource);

                yield return new WaitForSeconds(0.2f);
            }
            else
                break;
        }

        StopCoroutine(StarAnim(specialCoin));
    }

    #endregion

    #region button click event

    public void OnClickReturnButton()
    {       
        GameRecord.Instance.Save(GameRoot.GameRecordJsonSavePath);
        SceneManager.LoadScene(1);
    }

    public void OnClickRestartButton()
    {
        GameRecord.Instance.currentLevelIndex--;

        if (GameRecord.Instance.currentLevelIndex <= 3)
        {
            GameRecord.Instance.Save(GameRoot.GameRecordJsonSavePath);
            SceneManager.LoadScene(GameRecord.Instance.currentLevelIndex + 1);
        }        
    }

    public void OnClickNextLevelButton()
    {
        Debug.Log(GameRecord.Instance.currentLevelIndex);

        if (GameRecord.Instance.currentLevelIndex <= 3)
        {
            GameRecord.Instance.Save(GameRoot.GameRecordJsonSavePath);
            SceneManager.LoadScene(GameRecord.Instance.currentLevelIndex + 1);
        }
    }

    #endregion
}
