using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomEditor(typeof(AnimationManager), true)]
public class AnimationManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AnimationManager animationManager = (AnimationManager)target;

        DrawDefaultInspector();

        if (animationManager.animator != null)
        {
            AnimatorController ac = animationManager.animator.runtimeAnimatorController as AnimatorController;
            if (ac != null)
            {
                if (GUILayout.Button("Populate Available Animation States"))
                {
                    PopulateAvailableAnimationStates(animationManager);
                }

                foreach (var array in animationManager.animationArrays)
                {
                    EditorGUILayout.LabelField($"Animations for {array.animationType}");

                    if (animationManager.availableAnimationStates.Count > 0)
                    {
                        for (int i = 0; i < array.selectedAnimations.Count; i++)
                        {
                            int selectedIndex = Mathf.Max(animationManager.availableAnimationStates.IndexOf(array.selectedAnimations[i]), 0);
                            selectedIndex = EditorGUILayout.Popup($"Select Animation {i + 1}", selectedIndex, animationManager.availableAnimationStates.ToArray());
                            array.selectedAnimations[i] = animationManager.availableAnimationStates[selectedIndex];

                            if (GUILayout.Button($"Remove Animation {i + 1}"))
                            {
                                array.selectedAnimations.RemoveAt(i);
                                break;
                            }
                        }

                        if (GUILayout.Button($"Add Animation for {array.animationType}"))
                        {
                            if (animationManager.availableAnimationStates.Count > 0)
                            {
                                array.selectedAnimations.Add(animationManager.availableAnimationStates[0]);
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No animation states found in the Animator Controller. Click the button above.", MessageType.Warning);
                    }
                }
            }
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Animation Setup Import/Export", EditorStyles.boldLabel);

        if (GUILayout.Button("Export Animation Setup"))
        {
            ExportAnimationSetup(animationManager);
        }

        if (GUILayout.Button("Import Animation Setup"))
        {
            ImportAnimationSetup(animationManager);
        }
    }

    private void PopulateAvailableAnimationStates(AnimationManager animationManager)
    {
        AnimatorController ac = animationManager.animator.runtimeAnimatorController as AnimatorController;
        if (ac != null)
        {
            animationManager.availableAnimationStates.Clear();

            foreach (var layer in ac.layers)
            {
                var states = layer.stateMachine.states.Select(state => state.state.name);
                animationManager.availableAnimationStates.AddRange(states);
            }
        }
    }

    private void ExportAnimationSetup(AnimationManager animationManager)
    {
        string path = EditorUtility.SaveFilePanel("Save Animation Setup", "", "AnimationSetup.json", "json");
        if (!string.IsNullOrEmpty(path))
        {
            AnimationSetupData data = new AnimationSetupData(animationManager);
            File.WriteAllText(path, JsonUtility.ToJson(data, true));
            Debug.Log($"Animation setup exported to: {path}");
        }
    }

    private void ImportAnimationSetup(AnimationManager animationManager)
    {
        string path = EditorUtility.OpenFilePanel("Load Animation Setup", "", "json");
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            string json = File.ReadAllText(path);
            AnimationSetupData data = JsonUtility.FromJson<AnimationSetupData>(json);
            data.ApplyTo(animationManager);
            Debug.Log($"Animation setup imported from: {path}");

            EditorUtility.SetDirty(animationManager);
        }
    }

    [System.Serializable]
    private class AnimationSetupData
    {
        public AnimationArrayData[] animationArrays;

        public AnimationSetupData(AnimationManager animationManager)
        {
            animationArrays = animationManager.animationArrays.Select(a => new AnimationArrayData(a)).ToArray();
        }

        public void ApplyTo(AnimationManager animationManager)
        {
            animationManager.animationArrays = animationArrays.Select(a => a.ToAnimationArray()).ToArray();
        }
    }

    [System.Serializable]
    private class AnimationArrayData
    {
        public AnimationType animationType;
        public List<string> selectedAnimations;

        public AnimationArrayData(AnimationArray array)
        {
            animationType = array.animationType;
            selectedAnimations = new List<string>(array.selectedAnimations);
        }

        public AnimationArray ToAnimationArray()
        {
            return new AnimationArray { animationType = this.animationType, selectedAnimations = new List<string>(this.selectedAnimations) };
        }
    }
}
