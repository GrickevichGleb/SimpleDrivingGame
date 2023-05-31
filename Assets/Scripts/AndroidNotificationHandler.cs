using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif


public class AndroidNotificationHandler : MonoBehaviour
{
#if UNITY_ANDROID
    
    private const string ChannelID = "notification_channel";
    public void ScheduleNotification(DateTime datetime)
    {
        // Creating notification channel
        AndroidNotificationChannel notificationChannel = new AndroidNotificationChannel
        {
            Id = ChannelID,
            Name = "NotificationChannel",
            Description = "SimpleDrivingGame_TestNotification",
            Importance =  Importance.Default
        };
        AndroidNotificationCenter.RegisterNotificationChannel(notificationChannel);
        
        // Creating notification itself
        AndroidNotification notification = new AndroidNotification
        {
            Title = "Energy Recharged",
            Text = "Your energy has been recharged, come back and play",
            SmallIcon = "default",
            LargeIcon = "default",
            FireTime = datetime
        };

        AndroidNotificationCenter.SendNotification(notification, ChannelID);
        //For settingUp icons go Edit/Project_Settings/Mobile_Notifications
        //And add your icons
    }

    public void CancelDisplayedNotifications()
    {
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
    }

#endif
}
