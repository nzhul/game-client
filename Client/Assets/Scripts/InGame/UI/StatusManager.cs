using Assets.Scripts.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour
{
    public Text Account;
    public Text Hero;
    public Text Resources;

    public string accountTemplate = "Account: {0}";
    public string heroTemplate = "Hero: {0}";
    public string resourcesTemplate = "Wood: {0} Ore: {1} Gold: {2} Gems {3}";

    // Start is called before the first frame update
    private void Start()
    {
        var avatar = DataManager.Instance.Avatar;
        this.Account.text = string.Format(this.accountTemplate, DataManager.Instance.Username);
        this.Hero.text = string.Format(this.heroTemplate, avatar.Heroes.FirstOrDefault(h => h.Id == DataManager.Instance.ActiveHeroId).Class.ToString());
        this.Resources.text = string.Format(this.resourcesTemplate, avatar.Wood, avatar.Ore, avatar.Gold, avatar.Gems);
    }
}
