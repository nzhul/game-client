using System;
using UnityEngine;

namespace Assets.Scripts.UI.HeroCreation
{
    [Serializable]
    public class FactionData
    {
        public string name;
        public string description;
        public Sprite icon;
        public FactionClass[] heroes;
    }

    [Serializable]
    public class FactionClass
    {
        public string name;
        public string description;
        public Sprite icon;
        public Mesh model3D;
        public Resource resource;
        public ClassSkillTree[] skillTrees;
    }

    [Serializable]
    public class ClassSkillTree
    {
        public string name;
        public string description;
    }

    public enum Resource
    {
        Rage,
        Mana,
        Combo
    }
}
