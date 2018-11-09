using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeView : MonoBehaviour
{
    public GameObject graphic; // main graphic
    public Node node;

    public Material openMat;
    public Material wallMat;
    public Material contactMat;
    public Material occupiedMat;

    bool isFocus = false;
    MeshRenderer mr;

    private Dictionary<NodeType, Material> materials;

    private void Awake()
    {
        mr = graphic.GetComponent<MeshRenderer>();
        materials = new Dictionary<NodeType, Material>();
        materials.Add(NodeType.Open, openMat);
        materials.Add(NodeType.Wall, wallMat);
        materials.Add(NodeType.ContactPoint, contactMat);
        materials.Add(NodeType.Occupied, occupiedMat);
    }

    public void Init(Node node)
    {
        if (graphic != null)
        {
            this.node = node;
            gameObject.name = "Node (" + node.gridX + "," + node.gridY + ")";
            gameObject.transform.position = node.worldPosition;

            InitGraphic(node);
        }
    }

    private void InitGraphic(Node node)
    {
        graphic.layer = this.ResolveLayer(node.nodeType);
        switch (node.nodeType)
        {
            case NodeType.Open:
                this.InitOpen();
                break;
            case NodeType.Wall:
                this.InitWall();
                break;
            case NodeType.ContactPoint:
                this.InitContact();
                break;
            case NodeType.Occupied:
                this.InitOccupied();
                break;
            default:
                break;
        }
    }

    private void InitOpen()
    {
        this.ChangeMaterial(openMat);
    }

    private void InitWall()
    {
        this.ChangeMaterial(wallMat);
    }

    private void InitContact()
    {
        this.ChangeMaterial(contactMat);
        graphic.transform.localScale = new Vector3(1, .5f, 1);
        graphic.transform.localPosition = new Vector3(0, .2f, 0);
    }

    private void InitOccupied()
    {
        this.ChangeMaterial(occupiedMat);
        graphic.transform.localScale = Vector3.one;
        graphic.transform.localPosition = new Vector3(0, .4f, 0);
    }

    private void ChangeMaterial(Material material)
    {
        //var mr = graphic.GetComponent<MeshRenderer>();
        mr.sharedMaterial = material;
    }

    public void OnDefocused()
    {
        isFocus = false;
        this.ResetGraphics();
    }

    public void ResetGraphics()
    {
        this.ChangeMaterial(this.materials[node.nodeType]);
    }

    public void OnFocused()
    {
        isFocus = true;
        this.MarkAsFocused();
    }

    private void MarkAsFocused()
    {
        mr.material.color = Color.green;
    }

    internal void Highlight()
    {
        mr.material.color = Color.magenta;
    }

    private int ResolveLayer(NodeType nodeType)
    {
        string layerName = nodeType == NodeType.Open ? "Interactable" : "Default";
        return LayerMask.NameToLayer(layerName);
    }
}