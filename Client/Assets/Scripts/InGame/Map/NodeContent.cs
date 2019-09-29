using UnityEngine;

public class NodeContent : MonoBehaviour
{
    [SerializeField]
    NodeContentType type = default;

    public NodeContentType Type => type;

    public bool BlocksPath => Type >= NodeContentType.Hero;

    public virtual void Interact(HeroView interactingHero) { }
}