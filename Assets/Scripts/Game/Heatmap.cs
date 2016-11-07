using UnityEngine;
using System.Collections;

public class Heatmap : MonoBehaviour
{
    public int Width = 5;
    public int Height = 5;
    public float CellSize = 0.1f;

    private float m_halfWidth = 0.0f;

    private bool[,] m_blockers;
    private float[,] m_heat;

    public void Begin()
    {
        m_halfWidth = ((float)Width * CellSize) / 2.0f;

        m_blockers = new bool[Width, Height];
        m_heat = new float[Width, Height];
    }
	
	public void UpdateHeatmap ()
    {
        Vector3 size = new Vector3(CellSize / 2.0f, CellSize / 2.0f, CellSize / 2.0f);

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                float xPos = -m_halfWidth + (CellSize / 2.0f) + x * CellSize;
                float yPos = (CellSize / 2.0f) + y * CellSize;

                Collider[] overlaps = Physics.OverlapBox(new Vector3(transform.position.x + xPos, 0.01f, transform.position.z + yPos), size);

                if (overlaps.Length > 0)
                {
                    m_blockers[x, y] = true;
                }
                else
                {
                    m_blockers[x, y] = false;
                }
            }
        }

        ProcessHeatmapBlockers();
    }

    void ProcessHeatmapBlockers()
    {
        // Heat conditions todo:
        // Change proximity to be a kernel rather than just a cardinal axis check, so it can have variable
        // Weight cells according to the dot-product of their direction Vs global target to pick cells in the desired direction
        // Do I collapse the heat-map down or something? How do I stop it going for cells behind blocked cells?

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                float total = 0.0f;
                int contributions = 0;

                m_heat[x, y] = 0.0f;

                float centeredX = (float)(x) - ((float)Width) / 2.0f;

                Vector2 toCell = new Vector2(centeredX, y);
                toCell.Normalize();

                float angleToCell = Mathf.Atan2(toCell.x, toCell.y);

                // If the angle is too steep, flood the heat for that cell
                if (Mathf.Abs(angleToCell * Mathf.Rad2Deg) > 45.0f)
                {
                    m_heat[x, y] += 100.0f;
                }

                // Only have non-zero heat if not explicitly blocked
                if (!m_blockers[x, y])
                {
                    
                    if (x > 1) { total += m_blockers[x - 1, y] ? 1.0f : 0.0f; contributions++; }
                    if (x < Width - 1) { total += m_blockers[x + 1, y] ? 1.0f : 0.0f; contributions++; }
                    if (y > 1) { total += m_blockers[x, y - 1] ? 1.0f : 0.0f; contributions++; }
                    if (y < Height - 1) { total += m_blockers[x, y + 1] ? 1.0f : 0.0f; contributions++; }
                    
                    m_heat[x, y] += total / (float)contributions;
                }
                else
                {
                    m_heat[x, y] += 100.0f;

                    // Batter the score of cells in front of the blocker
                    if (y > 0) m_heat[x, y - 1] += 0.8f;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        return;
        HSBColor red = new HSBColor(Color.red);
        HSBColor green = new HSBColor(Color.green);

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                float xPos = -m_halfWidth + (CellSize / 2.0f) + x * CellSize;
                float yPos = (CellSize / 2.0f) + y * CellSize;

                Color cellColor = HSBColor.Lerp(green, red, Mathf.Clamp01(m_heat[x, y])).ToColor();
                cellColor.a = 1.0f;

                Gizmos.color = cellColor;
                Gizmos.DrawCube(new Vector3(transform.position.x + xPos, transform.position.y, transform.position.z + yPos), new Vector3(CellSize, 0.001f, CellSize));
            }
        }
        
        
    }
}
