using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCode.Persistence;
using GameCode.Persistence.Models;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public abstract class DataContext
{
    public SaveData saveData = new SaveData();
    
    public abstract Task Load();
    public abstract Task Save();

    public List<T> Set<T>()
    {
        if (typeof(T) == typeof(Level))
        {
            return saveData.levels as List<T>; 
        }
        return null;
    }
}
