using Assets.Scripts.InGame.Map.Entities;
using UnityEngine;

public class NodeContent : MonoBehaviour
{
    [SerializeField]
    NodeContentType type = default;

    public NodeContentType Type => type;

    public bool BlocksPath => Type >= NodeContentType.Army;

    public virtual void Interact(EntityView interactingHero) { }
}