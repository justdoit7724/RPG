using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MobMessage
{
    Die,
    CurseOn,
    CurseOff
}
public class MobMgr : MonoBehaviour
{
    public GameObject eSwordPrefab;
    public GameObject eBowPrefab;

    private static MobMgr instance = null;
    public static MobMgr Instance 
    {
        get 
        {
            if(instance==null)
            {
                instance = new GameObject("MobMgr").AddComponent<MobMgr>();
            }

            return instance;
        }
    }

    private List<Mob> mobs=new List<Mob>();

    public void RegisterMob(Mob mob)
    {
        if(mobs.Contains(mob))
        {
            Debug.LogError("this mob is already registered");
        }

        mobs.Add(mob);
    }
    public void RemoveMob(Mob mob)
    {
        if (!mobs.Contains(mob))
        {
            Debug.LogError("this mob is not registered, but trying to remove");
        }

        mobs.Remove(mob);
    }

   
    public void SendMessage(Mob sender, MobMessage msg)
    {
        foreach (Mob item in mobs)
        {
            if (item != sender)
            {
                item.GetMessage(sender, msg);
            }
        }
    }
}
