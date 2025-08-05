using System;
using _Scripts.Entities.MainMenu.Enums;
using UnityEngine;

namespace _Scripts.Entities.MainMenu.DataClasses
{
    [Serializable]
    public class MainMenuPage
    {
        public MainMenuPageType type;
        public GameObject gameObject;
    }
}