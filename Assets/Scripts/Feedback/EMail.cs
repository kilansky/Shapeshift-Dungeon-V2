using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine.UI;

public class EMail : SingletonPattern<EMail>
{
    public Slider levelRatingSlider;
    public TMP_InputField inputField;

    private PlayerHealth ph;
    private PlayerController pc;
    string specialItem;
    string headItem;
    string torsoItem;
    string footItem;
    string pocket1Item;
    string pocket2Item;

    private void Start()
    {
        ph = PlayerHealth.Instance;
        pc = PlayerController.Instance;
    }

    public void SubmitReview()
    {
        GetEquippedItemNames();

        int levelRating = (int)levelRatingSlider.value;
        string emailSubject = LevelManager.Instance.currMapName + " Feedback";
        string emailBody = "Player ID: " + PlayerPrefs.GetInt("UserID") + " \n" +
            "Level Rating: " + levelRating + " \n" +
            "Floor #: " + LevelManager.Instance.currFloor + " \n" +
            "Player Health: " + ph.Health + " \n" +
            "Potion Count: " + ph.GetPotionCount().ToString() + " \n" +
            "Items: " + specialItem + ", " + headItem + ", " + torsoItem + ", " + 
            footItem + ", " + pocket1Item + ", " + pocket2Item + " \n" +
            "Health Upgrades: " + pc.StatMaxHealthCount + " \n" +
            "Attack Upgrades: " + pc.StatAttackCount + " \n" +
            "Speed Upgrades: " + pc.StatSpeedCount + " \n" +
            "Run Time: " + RunTimer.Instance.TimerTextValue + " \n" + inputField.text;

        try { SendEmail(emailSubject, emailBody); }
        catch { Debug.LogError("Failed to send email feedback"); }

        HUDController.Instance.HideLevelReviewPanel();
        LevelManager.Instance.TransitionLevel();
    }

    private void GetEquippedItemNames()
    {
        specialItem = (pc.SpecialSlot ? pc.SpecialSlot.ItemName : "---");
        headItem = (pc.HeadSlot ? pc.HeadSlot.ItemName : "---");
        torsoItem = (pc.TorsoSlot ? pc.TorsoSlot.ItemName : "---");
        footItem = (pc.FootSlot ? pc.FootSlot.ItemName : "---");
        pocket1Item = (pc.PocketSlot1 ? pc.PocketSlot1.ItemName : "---");
        pocket2Item = (pc.PocketSlot2 ? pc.PocketSlot2.ItemName : "---");
    }

    public void SkipReview()
    {
        HUDController.Instance.HideLevelReviewPanel();
        LevelManager.Instance.TransitionLevel();
        inputField.text = "<i>Enter text... help us make this level better!</i>";
    }

    public void SendEmail(string subject, string body)
    {
        MailMessage mail = new MailMessage();

        mail.From = new MailAddress("shapeshiftdungeon@gmail.com");
        mail.To.Add("shapeshiftdungeon@gmail.com");
        mail.Subject = subject;
        mail.Body = body;

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential("shapeshiftdungeon@gmail.com", "cagd4952021") as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
        smtpServer.Send(mail);
        Debug.Log("success");

        inputField.text = "<i>Enter text... help us make this level better!</i>";
    }
}