#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MultiplierScript))]
public class MultiplierScriptEditor : Editor
{
    private bool showNormal = true;
    private bool showBlackHole = true;

    public override void OnInspectorGUI()
    {
        MultiplierScript script = (MultiplierScript)target;

        EditorGUI.BeginChangeCheck();

        // ============================================================
        // BASIC SETTINGS
        // ============================================================

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Multiplier Chance Calculator",
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space();

        script.spawnedByBlackHole = EditorGUILayout.Toggle(
            "Spawned By Black Hole",
            script.spawnedByBlackHole
        );

        script.blackHoleLuck = Mathf.Max(
            1f,
            script.blackHoleLuck
        );

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(script);
        }

        EditorGUILayout.Space();

        // ============================================================
        // NORMAL CHANCES
        // ============================================================

        showNormal = EditorGUILayout.Foldout(
            showNormal,
            "Normal Chances",
            true
        );

        if (showNormal)
        {
            DrawNormalTable(script);
        }

        EditorGUILayout.Space();

        // ============================================================
        // BLACK HOLE CHANCES
        // ============================================================

        showBlackHole = EditorGUILayout.Foldout(
            showBlackHole,
            "Black Hole Chances",
            true
        );

        if (showBlackHole)
        {
            DrawBlackHoleTable(script);
        }

        EditorGUILayout.Space();

        // ============================================================
        // QUICK CALCULATOR
        // ============================================================

        DrawSimulationInfo(script);

        EditorGUILayout.Space();

        // ============================================================
        // ORIGINAL INSPECTOR
        // ============================================================

        EditorGUILayout.LabelField(
            "Script Values",
            EditorStyles.boldLabel
        );

        DrawDefaultInspector();
    }

    // ============================================================
    // NORMAL TABLE
    // ============================================================

    private void DrawNormalTable(MultiplierScript script)
    {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField(
            "NORMAL MULTIPLIERS",
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(
            "Multiplier",
            GUILayout.Width(90)
        );

        EditorGUILayout.LabelField(
            "Chance",
            GUILayout.Width(70)
        );

        EditorGUILayout.EndHorizontal();

        float multiplierTotal =
            GetTotal(script.multiplierChances);

        // ============================================================
        // MULTIPLIERS
        // ============================================================

        if (script.multipliers != null)
        {
            for (int i = 0; i < script.multipliers.Length; i++)
            {
                float weight =
                    GetArrayValue(
                        script.multiplierChances,
                        i
                    );

                float chance =
                    CalculateChance(
                        weight,
                        multiplierTotal
                    );

                DrawChanceRow(
                    script.multipliers[i].ToString("0") + "x",
                    weight,
                    chance
                );
            }
        }

        EditorGUILayout.Space();

        // ============================================================
        // PLUS
        // ============================================================

        EditorGUILayout.LabelField(
            "PLUS",
            EditorStyles.boldLabel
        );

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(
            "Plus",
            GUILayout.Width(90)
        );

        EditorGUILayout.LabelField(
            "Chance",
            GUILayout.Width(70)
        );

        EditorGUILayout.EndHorizontal();

        float plusTotal =
            GetTotal(script.plusChances);

        if (script.plus != null)
        {
            for (int i = 0; i < script.plus.Length; i++)
            {
                float weight =
                    GetArrayValue(
                        script.plusChances,
                        i
                    );

                float chance =
                    CalculateChance(
                        weight,
                        plusTotal
                    );

                DrawChanceRow(
                    script.plus[i],
                    weight,
                    chance
                );
            }
        }


        EditorGUILayout.EndVertical();
    }

    // ============================================================
    // BLACK HOLE TABLE
    // ============================================================

    private void DrawBlackHoleTable(MultiplierScript script)
    {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField(
            "BLACK HOLE CHANCES",
            EditorStyles.boldLabel
        );

        EditorGUILayout.LabelField(
            "Black Hole Luck = " +
            script.blackHoleLuck.ToString("0.##"),
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space();

        // ============================================================
        // MULTIPLIERS
        // ============================================================

        EditorGUILayout.LabelField(
            "MULTIPLIERS",
            EditorStyles.boldLabel
        );

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(
            "Multiplier",
            GUILayout.Width(70)
        );

        EditorGUILayout.LabelField(
            "Base",
            GUILayout.Width(60)
        );

        EditorGUILayout.LabelField(
            "Bonus",
            GUILayout.Width(65)
        );

        EditorGUILayout.LabelField(
            "Chance",
            GUILayout.Width(75)
        );

        EditorGUILayout.EndHorizontal();

        float multiplierTotal =
            GetBlackHoleMultiplierTotal(script);

        if (script.multipliers != null)
        {
            for (int i = 0;
                 i < script.multipliers.Length;
                 i++)
            {
                float baseChance =
                    GetArrayValue(
                        script.multiplierChances,
                        i
                    );

                float bonus =
                    GetBonus(
                        script.blackHoleLuck,
                        i,
                        script.multipliers.Length
                    );

                float weighted =
                    baseChance * bonus;

                float chance =
                    CalculateChance(
                        weighted,
                        multiplierTotal
                    );

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(
                    script.multipliers[i].ToString("0") + "x",
                    GUILayout.Width(70)
                );

                EditorGUILayout.LabelField(
                    baseChance.ToString("0.##"),
                    GUILayout.Width(60)
                );

                EditorGUILayout.LabelField(
                    "x" + bonus.ToString("0.##"),
                    GUILayout.Width(65)
                );

                EditorGUILayout.LabelField(
                    FormatChance(chance),
                    GUILayout.Width(75)
                );

                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space();

        // ============================================================
        // PLUS
        // ============================================================

        EditorGUILayout.LabelField(
            "PLUS",
            EditorStyles.boldLabel
        );

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(
            "Plus",
            GUILayout.Width(70)
        );

        EditorGUILayout.LabelField(
            "Base",
            GUILayout.Width(60)
        );

        EditorGUILayout.LabelField(
            "Bonus",
            GUILayout.Width(65)
        );

        EditorGUILayout.LabelField(
            "Chance",
            GUILayout.Width(75)
        );

        EditorGUILayout.EndHorizontal();

        float plusTotal =
            GetBlackHolePlusTotal(script);

        if (script.plus != null)
        {
            for (int i = 0;
                 i < script.plus.Length;
                 i++)
            {
                float baseChance =
                    GetArrayValue(
                        script.plusChances,
                        i
                    );

                float bonus =
                    GetBonus(
                        script.blackHoleLuck,
                        i,
                        script.plus.Length
                    );

                float weighted =
                    baseChance * bonus;

                float chance =
                    CalculateChance(
                        weighted,
                        plusTotal
                    );

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(
                    script.plus[i],
                    GUILayout.Width(70)
                );

                EditorGUILayout.LabelField(
                    baseChance.ToString("0.##"),
                    GUILayout.Width(60)
                );

                EditorGUILayout.LabelField(
                    "x" + bonus.ToString("0.##"),
                    GUILayout.Width(65)
                );

                EditorGUILayout.LabelField(
                    FormatChance(chance),
                    GUILayout.Width(75)
                );

                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndVertical();
    }

    // ============================================================
    // QUICK CALCULATOR
    // ============================================================

    private void DrawSimulationInfo(MultiplierScript script)
    { }

    // ============================================================
    // ROW
    // ============================================================

    private void DrawChanceRow(
        string label,
        float weight,
        float chance
    )
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(
            label,
            GUILayout.Width(90)
        );

        EditorGUILayout.LabelField(
            FormatChance(chance),
            GUILayout.Width(70)
        );

        EditorGUILayout.EndHorizontal();
    }

    // ============================================================
    // ARRAY VALUE
    // ============================================================

    private float GetArrayValue(
        float[] array,
        int index
    )
    {
        if (array == null)
            return 0f;

        if (index < 0 ||
            index >= array.Length)
            return 0f;

        return Mathf.Max(
            0f,
            array[index]
        );
    }

    // ============================================================
    // TOTAL
    // ============================================================

    private float GetTotal(
        float[] values
    )
    {
        if (values == null)
            return 0f;

        float total = 0f;

        for (int i = 0;
             i < values.Length;
             i++)
        {
            total += Mathf.Max(
                0f,
                values[i]
            );
        }

        return total;
    }

    // ============================================================
    // NORMAL CHANCE
    // ============================================================

    private float CalculateChance(
        float weight,
        float total
    )
    {
        if (total <= 0f)
            return 0f;

        return
            Mathf.Max(0f, weight) /
            total *
            100f;
    }

    // ============================================================
    // BLACK HOLE BONUS
    // ============================================================

    private float GetBonus(
        float luck,
        int index,
        int length
    )
    {
        if (length <= 1)
            return 1f;

        float normalizedIndex =
            (float)index /
            (length - 1);

        return
            1f +
            ((luck - 1f) *
            normalizedIndex);
    }

    // ============================================================
    // BLACK HOLE MULTIPLIER TOTAL
    // ============================================================

    private float GetBlackHoleMultiplierTotal(
        MultiplierScript script
    )
    {
        if (script.multiplierChances == null)
            return 0f;

        int count = Mathf.Min(
            script.multiplierChances.Length,
            script.multipliers != null
                ? script.multipliers.Length
                : 0
        );

        float total = 0f;

        for (int i = 0;
             i < count;
             i++)
        {
            float baseChance =
                Mathf.Max(
                    0f,
                    script.multiplierChances[i]
                );

            float bonus =
                GetBonus(
                    script.blackHoleLuck,
                    i,
                    count
                );

            total +=
                baseChance * bonus;
        }

        return total;
    }

    // ============================================================
    // BLACK HOLE PLUS TOTAL
    // ============================================================

    private float GetBlackHolePlusTotal(
        MultiplierScript script
    )
    {
        if (script.plusChances == null)
            return 0f;

        int count = Mathf.Min(
            script.plusChances.Length,
            script.plus != null
                ? script.plus.Length
                : 0
        );

        float total = 0f;

        for (int i = 0;
             i < count;
             i++)
        {
            float baseChance =
                Mathf.Max(
                    0f,
                    script.plusChances[i]
                );

            float bonus =
                GetBonus(
                    script.blackHoleLuck,
                    i,
                    count
                );

            total +=
                baseChance * bonus;
        }

        return total;
    }

    // ============================================================
    // NORMAL CHANCE
    // ============================================================

    private float GetNormalChance(
        float[] chances,
        int index
    )
    {
        if (chances == null)
            return 0f;

        if (index < 0 ||
            index >= chances.Length)
            return 0f;

        float total =
            GetTotal(chances);

        if (total <= 0f)
            return 0f;

        return
            Mathf.Max(
                0f,
                chances[index]
            ) /
            total *
            100f;
    }

    // ============================================================
    // BLACK HOLE CHANCE
    // ============================================================

    private float GetBlackHoleChance(
        float[] chances,
        int index,
        float luck
    )
    {
        if (chances == null)
            return 0f;

        if (index < 0 ||
            index >= chances.Length)
            return 0f;

        float total = 0f;

        for (int i = 0;
             i < chances.Length;
             i++)
        {
            float baseChance =
                Mathf.Max(
                    0f,
                    chances[i]
                );

            float bonus =
                GetBonus(
                    luck,
                    i,
                    chances.Length
                );

            total +=
                baseChance * bonus;
        }

        if (total <= 0f)
            return 0f;

        float selectedBonus =
            GetBonus(
                luck,
                index,
                chances.Length
            );

        float selectedWeighted =
            Mathf.Max(
                0f,
                chances[index]
            ) *
            selectedBonus;

        return
            selectedWeighted /
            total *
            100f;
    }

    // ============================================================
    // FIND MULTIPLIER
    // ============================================================

    private int FindMultiplierIndex(
        MultiplierScript script,
        float value
    )
    {
        if (script.multipliers == null)
            return -1;

        for (int i = 0;
             i < script.multipliers.Length;
             i++)
        {
            if (Mathf.Approximately(
                script.multipliers[i],
                value
            ))
            {
                return i;
            }
        }

        return -1;
    }

    // ============================================================
    // FIND PLUS
    // ============================================================

    private int FindPlusIndex(
        MultiplierScript script,
        string value
    )
    {
        if (script.plus == null)
            return -1;

        for (int i = 0;
             i < script.plus.Length;
             i++)
        {
            if (string.Equals(
                script.plus[i],
                value,
                System.StringComparison.OrdinalIgnoreCase
            ))
            {
                return i;
            }
        }

        return -1;
    }

    // ============================================================
    // FORMAT
    // ============================================================

    private string FormatChance(
        float chance
    )
    {
        if (chance <= 0f)
            return "0%";

        if (chance < 0.01f)
            return chance.ToString("0.####") + "%";

        if (chance < 1f)
            return chance.ToString("0.###") + "%";

        return chance.ToString("0.##") + "%";
    }
}
#endif