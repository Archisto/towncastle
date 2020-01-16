using System;
using UnityEngine;
using UnityEditor;

public class ToolsMenu : ScriptableObject
{
    [Flags]
    public enum FlagsTest
    {
        First = 1 << 0,
        Second = 1 << 1,
        Third = 1 << 2,
        Fourth = 1 << 3,
        Fifth = 1 << 4
    }

    [MenuItem("Tools/Create Stage Center")]
    static void CreateStageCenter()
    {
        HexGrid grid = FindObjectOfType<HexGrid>();
        if (grid != null && grid.GridSizeX > 0 && grid.GridSizeY > 0)
        {
            GameObject stageCenter = new GameObject("StageCenter");
            float x = 0.5f * grid.CellSize * (grid.GridSizeX - 1);
            float z = 0.5f * grid.CellGapZ * (grid.GridSizeY - 1);
            Vector3 position = grid.transform.position + new Vector3(x, 2.5f, z);
            stageCenter.transform.position = position;

            Debug.Log(string.Format("GridSizeX: {0}, GridSizeY: {1}, CellSize: {2}, CellGapZ: {3}",
                grid.GridSizeX, grid.GridSizeY, grid.CellSize, grid.CellGapZ));
        }
    }

    [MenuItem("Tools/Test/Binary Test")]
    static void BinaryTest()
    {
        // bitsA == bitsB            ->  all same
        // bitsA == (bitsB | bitsC)  ->  all same
        // (bitsA & bitsB) == bitsA  ->  both have all in A
        // (bitsA & bitsB) > 0       ->  some same
        //                           ->  one same // TODO
        // bitsA != bitsB            ->  not all same
        // ((bitsA & bitsB) > 0
        // && (bitsA & bitsC) > 0)   ->  some same in [A and B] and [A and C]
        // (bitsA & bitsB) == 0      ->  none same

        // bitsA |= bitsB            ->  add B to A
        // bitsA &= ~bitsB           ->  clear B from A
        // bitsA ^= bitsB            ->  add what's different, clear what's same

        int bits1 = 1 << 0;
        int bits2 = 0x2;
        int bits3 = 0x4;
        int bits4 = 1 << 3;
        int bits5 = 1 << 4;

        int bits6 = bits1 | bits3;
        int bits7 = bits2 | bits3 | bits5;

        Debug.Log(string.Format("Bits1: {0}", bits1));
        Debug.Log(string.Format("Bits2: {0}", bits2));
        Debug.Log(string.Format("Bits3: {0}", bits3));
        Debug.Log(string.Format("Bits4: {0}", bits4));
        Debug.Log(string.Format("Bits5: {0}", bits5));
        Debug.Log(string.Format("Bits6 ({0}): {1}", "1, 3", bits6));
        Debug.Log(string.Format("Bits7 ({0}): {1}", "2, 3, 5", bits7));
        Debug.Log(string.Format("Bits2 included in 6: {0}", (bits2 & bits6) > 0));
        Debug.Log(string.Format("Bits3 included in 6: {0}", (bits3 & bits6) > 0));
        Debug.Log(string.Format("Bits1 and 3 only in 6: {0}", (bits1 | bits3) == bits6));
        Debug.Log(string.Format("Bits2 and 5 only in 7: {0}", (bits2 | bits5) == bits7));
        Debug.Log(string.Format("Bits2 and 5 included in 7: {0}", ((bits2 | bits5) & bits7) > 0));
        Debug.Log(string.Format("Bits1 and 3 included in 6: {0}", ((bits1 | bits3) & bits6) == (bits1 | bits3)));
        Debug.Log(string.Format("Bits3 and 4 included in 6: {0}", ((bits3 | bits4) & bits6) == (bits3 | bits4)));
        Debug.Log(string.Format("Bits3 or 4 included in 6: {0}", ((bits3 | bits4) & bits6) > 0));
        Debug.Log(string.Format("Bits1 not included in 7: {0}", (bits1 & bits7) == 0));
        Debug.Log(string.Format("Every bit in use: {0} / {1}", (bits1 | bits2 | bits3 | bits4 | bits5), ~bits6));
    }

    [MenuItem("Tools/Test/Flags Binary Test")]
    static void FlagsBinaryTest()
    {
        FlagsTest flags1 = FlagsTest.First;
        FlagsTest flags2 = FlagsTest.First | FlagsTest.Second;
        FlagsTest flags3 = FlagsTest.Third | FlagsTest.Fifth;
        FlagsTest flags4 = FlagsTest.Second | FlagsTest.Third | FlagsTest.Fourth;
        FlagsTest flags5 = flags2 | FlagsTest.Fourth;

        Debug.Log(string.Format("Flags1: {0}", flags1));
        Debug.Log(string.Format("Flags2: {0}", flags2));
        Debug.Log(string.Format("Flags3: {0}", flags3));
        Debug.Log(string.Format("Flags4: {0}", flags4));
        Debug.Log(string.Format("Flags1 included in 2: {0}", (flags1 & flags2) > 0));
        Debug.Log(string.Format("Fifth included in Flags4: {0}", (FlagsTest.Fifth & flags4) > 0));
        Debug.Log(string.Format("Second not included in 3: {0}", (FlagsTest.Second & flags3) == 0));
        Debug.Log(string.Format("Flags1 and Second only in 2: {0}", (flags1 | FlagsTest.Second) == flags2));
        Debug.Log(string.Format("Flags3 and Fourth only in 4: {0}", (flags3 | FlagsTest.Fourth) == flags4));
        Debug.Log(string.Format("Flags2 and 4 share values: {0}", (flags2 & flags4) > 0));
        Debug.Log(string.Format("Flags3 and 5 don't share values: {0}", (flags3 & flags5) == 0));
        Debug.Log(string.Format("Flags3 and 5 don't share values: {0}", flags3 == ~flags5));
        Debug.Log(string.Format("Both Flags1 and 3 share values with 4: {0}", ((flags1 & flags4) > 0 && (flags3 & flags4) > 0)));
        Debug.Log(string.Format("Values of Flags1 or 2 included in 4: {0}", ((flags1 | flags2) & flags4) > 0));
    }

    [MenuItem("Tools/Test/Print")]
    static void Print()
    {
        int a = 0b_11011011;
        int b = 0b_01101010;
        int c = a ^ b;        // 10110001
        Debug.Log(string.Format("C: {0}", Binary(c)));

        //Debug.Log(string.Format("Please edit ToolsMenu.Print()"));
    }

    static string Binary(int number)
    {
        return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
            (number & (1 << 7)) == 0 ? 0 : 1,
            (number & (1 << 6)) == 0 ? 0 : 1,
            (number & (1 << 5)) == 0 ? 0 : 1,
            (number & (1 << 4)) == 0 ? 0 : 1,
            (number & (1 << 3)) == 0 ? 0 : 1,
            (number & (1 << 2)) == 0 ? 0 : 1,
            (number & (1 << 1)) == 0 ? 0 : 1,
            (number & (1 << 0)) == 0 ? 0 : 1);
    }

    [MenuItem("Tools/Test/Progress Test")]
    static void ProgressTest()
    {
        float progress = 0;
        bool ok = EditorUtility.DisplayDialog("Progress Test", "Do It in C# !", "OK", "Nope");
        if (ok)
        {
            while (ok)
            {
                progress += 0.25f;

                if (progress < 1)
                {
                    EditorUtility.DisplayProgressBar("Progress Test", "LOADING", progress);
                    ok = EditorUtility.DisplayDialog("Progress Test", "Continue loading?", "Yes", "No");
                    if (!ok)
                    {
                        EditorUtility.ClearProgressBar();
                        break;
                    }
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Progress Test", "I hope this helped.", "Not really", "");
                    break;
                }
            }
        }
    }
}
