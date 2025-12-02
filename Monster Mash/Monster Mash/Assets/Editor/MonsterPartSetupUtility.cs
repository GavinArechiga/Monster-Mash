using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class MonsterPartSetupUtility : EditorWindow
{
    private GameObject monsterPartPrefab;
    private MonsterPartType monsterPartType;

    private enum NeutralAttackType
    {
        None,
        Jab,
        Slash,
        Projectile,
        Boomerang,
        Spray
    }

    private enum HeavyAttackType
    {
        None,
        Jab,
        Slash,
        Projectile,
        Boomerang,
        Spray,
        Beam,
        Grapple,
        Reel,
        Spinning
    }

    private NeutralAttackType neutralAttackType;
    private HeavyAttackType heavyAttackType;

    private string newAnimControllerPath = "";
    private string newMonsterPartAnimationPath = "";
    private AnimatorController newAnimationController;

    private bool animationControllerSetup;
    private bool mainScriptsAndComponentsSetup;
    private bool colliderTagsAndScriptsSetup;

    private bool templateAssetsMissing;

    float buttonHeight = 50;
    float buttonWidth = 300;

    float fieldWidth = 300;

    [MenuItem("Tools/Monster Part Setup Utility")]
    public static void ShowWindow()
    {
        var window = GetWindow<MonsterPartSetupUtility>("Monster Part Setup Utility", true);
        window.minSize = new Vector2 (790, 700);
        
    }

    private void OnGUI()
    {
        GUILayout.Label("Part Info", EditorStyles.boldLabel);
        GUILayout.Space(5);

        monsterPartPrefab = (GameObject)EditorGUILayout.ObjectField(
        "Monster Part Prefab",
        monsterPartPrefab,
        typeof(GameObject),
        false,
        GUILayout.Width(fieldWidth)
    );

        monsterPartType = (MonsterPartType)EditorGUILayout.EnumPopup("Monster Part Type", monsterPartType, GUILayout.Width(fieldWidth));
        neutralAttackType = (NeutralAttackType)EditorGUILayout.EnumPopup("Neutral Attack Type", neutralAttackType, GUILayout.Width(fieldWidth));
        heavyAttackType = (HeavyAttackType)EditorGUILayout.EnumPopup("Heavy Attack Type", heavyAttackType, GUILayout.Width(fieldWidth));

        GUILayout.Space(10);

        GUILayout.Label("", GUI.skin.horizontalScrollbar);

        EditorGUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Monster Part Setup", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        
        AnimationControllerSetupButton();
        MainScriptsAndComponentsSetupButton();
        ColliderTagsAndScriptSetupButton();

        GUILayout.Space(15);

        GUILayout.Label("", GUI.skin.horizontalScrollbar);

        DisplayHelpInfo();

        EditorGUILayout.Space(10);
        ResetButton();
    }

    #region Buttons
    private void AnimationControllerSetupButton()
    {
        EditorGUI.BeginDisabledGroup(animationControllerSetup);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Setup Animation Controller", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
        {
            if (!CheckForMissingRefs())
            {
                animationControllerSetup = true;
                SetupAnimationController();
                UpdatePrefab();

                if (!templateAssetsMissing)
                {
                    EditorUtility.DisplayDialog("Success", $"The animation controller for {monsterPartPrefab.name} has been setup successfully. " +
                        "You may need to manually move the animator to the top if other components were already attached.", "Ok");
                }
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();
    }

    private void MainScriptsAndComponentsSetupButton()
    {
        EditorGUI.BeginDisabledGroup(mainScriptsAndComponentsSetup);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Main Scripts and Components", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
        {
            if (CheckForMissingRefs()) { return; }
            mainScriptsAndComponentsSetup = true;

            AddScriptsAndTags();
            AddComponents();
            UpdatePrefab();

            EditorUtility.DisplayDialog("Success", "The required scripts and components have been setup on the monster part. " +
                "Not every field is automatically set so you should still look over the monster part to see if everything looks right.", "Ok");
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        EditorGUI.EndDisabledGroup();
    }

    private void ColliderTagsAndScriptSetupButton()
    {
        EditorGUI.BeginDisabledGroup(colliderTagsAndScriptsSetup);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Tags and Scripts to Colliders", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
        {
            if (CheckForMissingRefs()) { return; }
            colliderTagsAndScriptsSetup = true;

            ColliderSetup();
            UpdatePrefab();

            EditorUtility.DisplayDialog("Success", "All colliders have been tagged and the monster part refrence script has been added and setup. " +
                "The setup utility is not perfect and can make mistakes if colliders are not named properly. It is recomended that you look over all the colliders to check for mistakes.", "Ok");
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        EditorGUI.EndDisabledGroup();
    }

    private void ResetButton()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Reset Buttons", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
        {
            animationControllerSetup = false;
            mainScriptsAndComponentsSetup = false;
            colliderTagsAndScriptsSetup = false;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
    #endregion

    #region Animation Controller Setup
    private void SetupAnimationController()
    {
        Dictionary<MonsterPartType, string> partTypeToPath = new Dictionary<MonsterPartType, string>
        {
            { MonsterPartType.Arm, "Assets/Monster Parts/Arms" },
            { MonsterPartType.Leg, "Assets/Monster Parts/Legs" },
            { MonsterPartType.Wing, "Assets/Monster Parts/Wings" },
            { MonsterPartType.Mouth, "Assets/Monster Parts/Mouths" },
            { MonsterPartType.Decor, "Assets/Monster Parts/Decor" },
            { MonsterPartType.Horn, "Assets/Monster Parts/Horns" },
            { MonsterPartType.Tail, "Assets/Monster Parts/Tails" }

        };

        if (partTypeToPath.TryGetValue(monsterPartType, out string monsterPartPath))
        {
            string templatePath = monsterPartPath + "/Template Assets";

            if (!Directory.Exists(templatePath)) 
            {
                if (EditorUtility.DisplayDialog("Template Assets Missing", $"The template assets for {monsterPartType.ToString()}s is missing or the folder structure is not setup properly. " +
                    $"Look at how the template assets for arm 1 are setup if you need an example", "Ok"))
                {
                    animationControllerSetup = false;
                    templateAssetsMissing = true;
                    return;
                }
            }

            AnimationClip[] templateClips = GetAnimationClips(templatePath + "/Template Clips");

            CopyTemplateAssets(templatePath, templateClips, monsterPartPath);
            SwapAnimationControllerClips();
            AddNewAnimationController();
        }
        else
        {
            // Heads and torso use universal rigs
            string torsoControllerPath = "Assets/Monster Parts/Torsos/Torso 1/Torso 1.controller";
            string headControllerPath = "Assets/Monster Parts/Heads/Head 1/Animations/Head 1.controller";

            if (monsterPartType == MonsterPartType.Torso)
            {
                newAnimationController = AssetDatabase.LoadAssetAtPath<AnimatorController>(torsoControllerPath);
            }
            else if (monsterPartType == MonsterPartType.Head)
            {
                newAnimationController = AssetDatabase.LoadAssetAtPath<AnimatorController>(headControllerPath);
            }

            AddNewAnimationController();
        }
    }

    private AnimationClip[] GetAnimationClips(string folderPath)
    {
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { folderPath });
        List<AnimationClip> clips = new List<AnimationClip>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip != null)
            {
                clips.Add(clip);
            }
        }
        return clips.ToArray();
    }

    private void CopyTemplateAssets(string templatePath, AnimationClip[] templateClips, string partPath)
    {
        string templateAnimationControllerPath = templatePath + $"/{monsterPartType.ToString()} Template.controller";
        newMonsterPartAnimationPath = $"{partPath}/{monsterPartPrefab.name}/Animations";
        newAnimControllerPath = $"{newMonsterPartAnimationPath}/{monsterPartPrefab.name}.controller";

        if (!Directory.Exists(newMonsterPartAnimationPath))
        {
            Directory.CreateDirectory(newMonsterPartAnimationPath);
        }

        AssetDatabase.CopyAsset(templateAnimationControllerPath, newAnimControllerPath);

        string allTemplateClipsPath = templatePath + "/Template Clips";

        foreach (AnimationClip clip in templateClips)
        {
            string templateClipPrefix = "Template ";
            string clipName = clip.name;
            string removedPrefixClipName = "";

            if (clipName.ToLower().StartsWith(templateClipPrefix.ToLower()))
            {
                removedPrefixClipName = clipName.Substring(templateClipPrefix.Length);
                removedPrefixClipName = removedPrefixClipName.Trim();
            }
            else
            {
                if (EditorUtility.DisplayDialog("Template Clip Not Named Correctly", 
                    "In order for the setup utility to find the template clips they need to start with the word \"Template\"", "Ok"))
                {
                    animationControllerSetup = false;
                    return;
                }
            }

            string templateClipPath = $"{allTemplateClipsPath}/{clipName}.anim";
            string newClipPath = $"{newMonsterPartAnimationPath}/{monsterPartPrefab.name} {removedPrefixClipName}.anim";

            AssetDatabase.CopyAsset(templateClipPath, newClipPath);
        }
    }

    private void SwapAnimationControllerClips()
    {
        newAnimationController = AssetDatabase.LoadAssetAtPath<AnimatorController>(newAnimControllerPath);
        AnimationClip[] newAnimationClips = GetAnimationClips(newMonsterPartAnimationPath);

        foreach (var layer in newAnimationController.layers)
        {
            var stateMachine = layer.stateMachine;
            foreach (var currentState in stateMachine.states)
            {
                var motion = currentState.state.motion;
                if (motion is AnimationClip oldClip)
                {
                    string oldName = oldClip.name;
                    int sepIndex = oldName.IndexOf(" - ");
                    if (sepIndex < 0) continue;

                    string suffix = oldName.Substring(sepIndex);

                    AnimationClip newClip = Array.Find(newAnimationClips,
                        c =>
                        {
                            int newSepIndex = c.name.IndexOf(" - ");
                            return newSepIndex >= 0 && c.name.Substring(newSepIndex) == suffix;
                        });
                    if (newClip != null)
                    {
                        currentState.state.motion = newClip;
                    }
                }
            }
        }
    }

    private void AddNewAnimationController()
    {
        if (!monsterPartPrefab.TryGetComponent<Animator>(out var animator))
        {
            animator = monsterPartPrefab.AddComponent<Animator>();
        }

        animator.runtimeAnimatorController = newAnimationController;
    }
    #endregion

    #region Main Script and Component Setup
    private void AddScriptsAndTags()
    {
        if (!monsterPartPrefab.TryGetComponent<NewMonsterPart>(out var newMonsterPartScript))
        {
            newMonsterPartScript = monsterPartPrefab.AddComponent<NewMonsterPart>();
        }

        if (!monsterPartPrefab.TryGetComponent<MonsterPartVisual>(out var monsterPartVisualScript))
        {
            monsterPartPrefab.AddComponent<MonsterPartVisual>();
        }

        if (!monsterPartPrefab.TryGetComponent<TempPartData>(out var tempPartDataScript)) 
        {
            tempPartDataScript = monsterPartPrefab.AddComponent<TempPartData>();
        }

        if (!monsterPartPrefab.TryGetComponent<PartFlipped>(out var partFlippedScript))
        {
            monsterPartPrefab.AddComponent<PartFlipped>();
        }

       Type neutralClassType = AttackEnumToClassRef(neutralAttackType);
       Type heavyClassType = AttackEnumToClassRef(heavyAttackType);

        if (!monsterPartPrefab.TryGetComponent(neutralClassType, out _))
        {
            newMonsterPartScript.neutralAttack = (NeutralAttack)monsterPartPrefab.AddComponent(neutralClassType);
        }

        if (!monsterPartPrefab.TryGetComponent(heavyClassType, out _))
        {
            newMonsterPartScript.heavyAttack = (HeavyAttack)monsterPartPrefab.AddComponent(heavyClassType);
        }

        monsterPartPrefab.tag = "Connection - Monster Part";
        newMonsterPartScript.PartType = monsterPartType;


        string partTypeName = monsterPartType.ToString();
        string tempPartDataPath = $"Build-A-Scare Parts/{partTypeName}s/{monsterPartPrefab.name}";

        tempPartDataScript.monsterPart.partPrefabPath = tempPartDataPath;
    }

    private void AddComponents()
    {
        if (!monsterPartPrefab.TryGetComponent<Rigidbody>(out var rigidbody))
        {
            rigidbody = monsterPartPrefab.AddComponent<Rigidbody>();
        }

        if (!monsterPartPrefab.TryGetComponent<SphereCollider>(out var sphereCollider))
        {
            sphereCollider = monsterPartPrefab.AddComponent<SphereCollider>();
        }

        if (!monsterPartPrefab.TryGetComponent<AudioSource>(out var audioSource)) 
        {
            audioSource = monsterPartPrefab.AddComponent<AudioSource>();
        }

        rigidbody.useGravity = true;
        rigidbody.isKinematic = true;

        sphereCollider.radius = 0.1f;

        audioSource.volume = 0.1f;
        audioSource.spatialBlend = 0.5f;
    }
    #endregion

    #region Collider Setup
    private void ColliderSetup()
    {
        Collider[] colliders = monsterPartPrefab.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            //Skip root of prefab because its getting the sphere collider
            if (collider.name.Equals(collider.transform.root.name)) { continue; }

            if (!collider.TryGetComponent<monsterPartReference>(out var monsterPartReference))
            {
                monsterPartReference = collider.AddComponent<monsterPartReference>();
            }

            monsterPartReference.partReference = collider.transform.root.GetComponent<NewMonsterPart>();
            CheckIfHitboxOrHurtBox(collider, monsterPartReference);
            HitboxSetup(collider, monsterPartReference);
        }
    }

    private void HitboxSetup(Collider collider, monsterPartReference monsterPartReference)
    {
        if ((collider.name.ToLower().Contains("neutral") || collider.name.ToLower().Contains("heavy")) && monsterPartReference.isHitbox)
        {
            switch ((neutralAttackType, heavyAttackType))
            {
                case (NeutralAttackType.Jab, HeavyAttackType.Jab):
                    monsterPartReference.isJabOrSlash = true;
                    break;
                case (NeutralAttackType.Slash, HeavyAttackType.Slash):
                    monsterPartReference.isJabOrSlash = true;
                    break;
                case (NeutralAttackType.Projectile, HeavyAttackType.Projectile):
                    monsterPartReference.isProjectile = true;
                    break;
                case (NeutralAttackType.Boomerang, HeavyAttackType.Boomerang):
                    monsterPartReference.isBoomerang = true;
                    break;
                case (_, HeavyAttackType.Reel):
                    monsterPartReference.isReel = true;
                    break;
                case (_, HeavyAttackType.Grapple):
                    monsterPartReference.isGrapple = true;
                    break;
            }
        }
    }

    private void CheckIfHitboxOrHurtBox(Collider collider, monsterPartReference monsterPartReference)
    {
        if (collider.name.ToLower().Contains("hitbox"))
        {
            collider.gameObject.tag = "Hitbox";
            monsterPartReference.isHitbox = true;
        }
        else if (collider.name.ToLower().Contains("hurtbox"))
        {
            collider.gameObject.tag = "Hurtbox";
            monsterPartReference.isHitbox = false;
        }
    }
    #endregion

    #region Helper Functions
    private void UpdatePrefab()
    {
        EditorUtility.SetDirty(monsterPartPrefab);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void DisplayHelpInfo()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Important Info:", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox("This tool aims to automate mostly all aspects of setting up a monster part but there are some things that can not be automated like placing colliders." +
            " So please look over the monster part after using the setup utility to make sure everything is correct.", MessageType.Info);
        EditorGUILayout.HelpBox("In order for the animation controller setup to work please make sure that the part type has template assets setup. " +
            "Please refer to the folder structure for arm 1 as an example. The template clips need to start with the word \"Template\" in order for the setup utility to find them.", MessageType.Info);
        EditorGUILayout.HelpBox("In order for the animation clips to be Copied an empty folder needs to exit for the monster part under the monster parts folder.", MessageType.Info);
        EditorGUILayout.HelpBox("After clicking the button for \"setup animation controller\" it may look like the window has stalled. " +
            "This is because setting up the animation controller involves a lot of steps and it accesses the file system which can be slow. " +
            "While the setup process is executing please do not try to click anything or try to spam the button as this could cause a crash.", MessageType.Info);
        EditorGUILayout.HelpBox("In order for hitboxes and hurtboxes to be tagged correctly the game object they are attached to needs to have the word \"Hitbox\" or \"Hurtbox\" in the name. Not case sensitive", MessageType.Info);
        EditorGUILayout.HelpBox("In order for neutral and heavy hitboxes to have special attacks automatically enabled, the game object they are attached to needs to have the word \"Neutral\" or \"Heavy\" in the name. Not case sensitive", MessageType.Info);
    }

    private bool CheckForMissingRefs()
    {
        if (monsterPartType == MonsterPartType.None)
        {
            if (EditorUtility.DisplayDialog("Monster Part Type Not Set", "The monster part type has not been assigned. This is required in order for the setup utility to work.", "Ok"))
            {
                return true;
            }
        }

        if (monsterPartPrefab == null)
        {
            if (EditorUtility.DisplayDialog("Monster Part Prefab Not Set", 
                "The monster part prefab has not been assigned. This is required for the setup utility to work. you can't really setup a monster part without well... a monster part.", "Ok"))
            {
                return true;
            }
        }

        if (monsterPartType != MonsterPartType.Torso && monsterPartType != MonsterPartType.Head)
        {
            if (neutralAttackType == NeutralAttackType.None || heavyAttackType == HeavyAttackType.None)
            {
                if (EditorUtility.DisplayDialog("Neutral or Heavy Attack Type Not Assigned",
                    "The neutral or heavy attack is not assigned. This is required for the setup utility to work unless the part is a head or torso.", "Okay"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private Type AttackEnumToClassRef(NeutralAttackType neutralAttackType)
    {
        return neutralAttackType switch
        {
            NeutralAttackType.Jab => typeof(JabNeutral),
            NeutralAttackType.Slash => typeof(SlashNeutral),
            NeutralAttackType.Projectile => typeof(ProjectileNeutral),
            NeutralAttackType.Boomerang => typeof(BoomerangNeutral),
            NeutralAttackType.Spray => typeof(SprayNeutral),
            _ => typeof(NeutralAttack),
        };
    }

    private Type AttackEnumToClassRef(HeavyAttackType neutralAttackType)
    {
        return heavyAttackType switch
        {
            HeavyAttackType.Jab => typeof(JabHeavy),
            HeavyAttackType.Slash => typeof(SlashHeavy),
            HeavyAttackType.Projectile => typeof(ProjectileHeavy),
            HeavyAttackType.Boomerang => typeof(BoomerangHeavy),
            HeavyAttackType.Spray => typeof(SprayHeavy),
            HeavyAttackType.Beam => typeof(BeamHeavy),
            HeavyAttackType.Grapple => typeof(GrappleHeavy),
            HeavyAttackType.Reel => typeof(ReelHeavy),
            HeavyAttackType.Spinning => typeof(SpinningHeavy),
            _ => typeof(HeavyAttack),
        };
    }

    #endregion
}

