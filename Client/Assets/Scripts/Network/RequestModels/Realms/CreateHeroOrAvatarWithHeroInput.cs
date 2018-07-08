using System;

namespace Assets.Scripts.Network.RequestModels.Realms
{
    [Serializable]
    public class CreateHeroOrAvatarInput
    {
        public string heroName;

        public string heroFaction;

        public string heroClass;
    }
}
