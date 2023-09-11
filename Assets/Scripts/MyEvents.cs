using System;

public static class MyEvents
{
    // PLAYER
    public static Action OnDecreaseHealth;
    public static void CallDecreaseHealth() { OnDecreaseHealth?.Invoke(); }
    public static Action OnPlayerDeath;
    public static void CallPlayerDied() { OnPlayerDeath?.Invoke(); }

    public static Action<int> OnGetBulletAmount;
    public static void CallGetBulletAmount(int bulletAmount) { OnGetBulletAmount?.Invoke(bulletAmount); }

    // SCORE
    public static Action OnIncreaseScore;
    public static void CallIncreaseScore() { OnIncreaseScore?.Invoke(); }
    public static Action<float> OnGetScore;
    public static void CallGetScore(float score) { OnGetScore?.Invoke(score); }
}
