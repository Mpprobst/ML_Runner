using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.TerrainGeneration;

public class BlockFinder : MonoBehaviour
{
    public List<Block> blocks;

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        blocks = new List<Block>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Block block = collision.GetComponentInParent<Block>();
        if (block)
        {
            if (!blocks.Contains(block))
                blocks.Add(block);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Block block = collision.GetComponentInParent<Block>();
        if (block)
            blocks.Remove(block);
    }
}
