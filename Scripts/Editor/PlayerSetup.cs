using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;              //for making this an editor
using System;                   //For exceptions
using CyberBullet.Controllers;
using CyberBullet.Cameras;

namespace CyberBullet.Editors {
    public class PlayerSetup : EditorWindow {

        #region Variables

        #region Inputs
        GameObject targetObject = null;
        #endregion

        #region VarHolders
        private bool btn_generated = false;
        string helpBox = "Click \"Generate\" to add all of the components and setup the hierarchy with default settings...";
        GameObject cameraHolder;
        GameObject camera;
        GameObject flashlight;
        GameObject weapons;
        GameObject weapon_camera;
        GameObject weapon_manager;
        GameObject equip_point;
        GameObject drop_point;
        GameObject throw_point;
        #endregion

        #region Helpers
        GenericEditor editor = new GenericEditor ();
        #endregion

        #endregion

        #region Visuals
        [MenuItem("Cyber Bullet/Player/Inital Setup")]
        private static void GDR_GeneratePlayer()
        {
            EditorWindow window = EditorWindow.GetWindow (typeof(PlayerSetup));//create an new windows
            window.minSize = new Vector2(400,500);
            window.Show ();
        }
        void OnEnable() 
        {
            editor.InitTextures ();
        }
        void OnGUI() 
        {
            editor.DrawBox ();
            editor.DrawTitle (" - GameDevRepo - ","Default Player Setup");
            if (targetObject == null) {
                editor.DrawHelpBox ("Add the object you want to make a player out of...");
            } else {
                editor.DrawHelpBox (helpBox);
            }
            ObjectInput ();
            Options ();
        }
        void ObjectInput() 
        {
            GUILayout.BeginArea(new Rect(20, 130, Screen.width-40, 100));
            targetObject = EditorGUILayout.ObjectField("Object:", targetObject, typeof(GameObject), true, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300)) as GameObject;
            GUILayout.EndArea ();
        }
        void Options() 
        {
            if (targetObject == null)
                return;
            GUILayout.BeginArea (new Rect (30, 200, Screen.width - 70, 30));
            helpBox = "Your done! If you added more points and want to update your connections grid, click refresh...";
            if (GUILayout.Button ("Generate Player", GUILayout.Height (30))) {
                GeneratePlayer();
            }
            GUILayout.EndArea ();
            if (btn_generated == true)
            {
                helpBox = "Your player is setup! Congratulations.\n You can always customize the script inputs to your needs...";
            }
        }
        #endregion

        #region Logic
        void GeneratePlayer() {
            try {
                helpBox = "Generating Hierarchy...";
                GenerateHierarchy();
                helpBox = "Applying components...";
                ApplyComponents();
                helpBox = "Making component connections...";
                MakeComponentConnections();
                btn_generated = true;
            }
            catch (Exception e) {
                helpBox = "AN ERROR OCCURED:\n"+e.ToString();
            }
        }
        void GenerateHierarchy() {
            //CameraHolder
            cameraHolder = new GameObject("Camera Holder");
            cameraHolder.transform.parent = targetObject.transform;
            cameraHolder.transform.localPosition = new Vector3(0, 1.5f, 0);

            camera = new GameObject("Camera");
            flashlight = new GameObject("Flashlight");
            camera.transform.parent = cameraHolder.transform;
            flashlight.transform.parent = cameraHolder.transform;
            camera.transform.localPosition = Vector3.zero;
            flashlight.transform.localPosition = Vector3.zero;

            //Camera
            weapons = new GameObject("Weapons");
            weapons.transform.parent = camera.transform;
            weapons.transform.localPosition = Vector3.zero;

            //Weapons
            weapon_camera = new GameObject("Weapon Camera");
            weapon_camera.transform.parent = weapons.transform;
            weapon_camera.transform.localPosition = Vector3.zero;

            //weaponCamera
            weapon_manager = new GameObject("Weapon Manager");
            equip_point = new GameObject("Equip Point");
            drop_point = new GameObject("Drop Point");
            throw_point = new GameObject("Throw Point");
            weapon_manager.transform.parent = weapon_camera.transform;
            equip_point.transform.parent = weapon_camera.transform;
            drop_point.transform.parent = weapon_camera.transform;
            throw_point.transform.parent = weapon_camera.transform;
            weapon_manager.transform.localPosition = Vector3.zero;
            equip_point.transform.localPosition = Vector3.zero;
            drop_point.transform.localPosition = Vector3.zero;
            throw_point.transform.localPosition = Vector3.zero;
        }
        void ApplyComponents() {
            //root
            if (!targetObject.GetComponent<Animator>())
                targetObject.AddComponent<Animator>();
            if (!targetObject.GetComponent<BreathingController>())
                targetObject.AddComponent<BreathingController>();
            if (!targetObject.GetComponent<Health>())
                targetObject.AddComponent<Health>();
            if (!targetObject.GetComponent<MouseLook>())
                targetObject.AddComponent<MouseLook>();
            if (!targetObject.GetComponent<CharacterController>())
                targetObject.AddComponent<CharacterController>();
            if (!targetObject.GetComponent<MovementController>())
                targetObject.AddComponent<MovementController>();
            if (!targetObject.GetComponent<FootStepKeyFrame>())
                targetObject.AddComponent<FootStepKeyFrame>();
            if (!targetObject.GetComponent<AnimController>())
                targetObject.AddComponent<AnimController>();
            if (!targetObject.GetComponent<CapsuleCollider>())
                targetObject.AddComponent<CapsuleCollider>();
            if (!targetObject.GetComponent<ClimbController>())
                targetObject.AddComponent<ClimbController>();
            if (!targetObject.GetComponent<SwimController>())
                targetObject.AddComponent<SwimController>();
            AudioSource[] sources = targetObject.GetComponents<AudioSource>();
            for (int i = 0; i < 4 - sources.Length; i++)
            {
                targetObject.AddComponent<AudioSource>();
            }

            //camera holder
            if (!cameraHolder.GetComponent<HeadBobber>())
                cameraHolder.AddComponent<HeadBobber>();
            if (!cameraHolder.GetComponent<MouseLook>())
                cameraHolder.AddComponent<MouseLook>();

            //Camera
            if (!camera.GetComponent<Camera>())
                camera.AddComponent<Camera>();
            if (!camera.GetComponent<AudioListener>())
                camera.AddComponent<AudioListener>();
            if (!camera.GetComponent<CameraShake>())
                camera.AddComponent<CameraShake>();
            if (!camera.GetComponent<Animation>())
                camera.AddComponent<Animation>();
            if (!camera.GetComponent<UnityStandardAssets.ImageEffects.CameraMotionBlur>())
                camera.AddComponent<UnityStandardAssets.ImageEffects.CameraMotionBlur>();
            if (!camera.GetComponent<UnityStandardAssets.ImageEffects.Antialiasing>())
                camera.AddComponent<UnityStandardAssets.ImageEffects.Antialiasing>();
            if (!camera.GetComponent<UnityStandardAssets.ImageEffects.BloomOptimized>())
                camera.AddComponent<UnityStandardAssets.ImageEffects.BloomOptimized>();
            if (!camera.GetComponent<UnityStandardAssets.ImageEffects.DepthOfField>())
                camera.AddComponent<UnityStandardAssets.ImageEffects.DepthOfField>();
            if (!camera.GetComponent<UnityStandardAssets.ImageEffects.ScreenSpaceAmbientOcclusion>())
                camera.AddComponent<UnityStandardAssets.ImageEffects.ScreenSpaceAmbientOcclusion>();
            if (!camera.GetComponent<UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration>())
                camera.AddComponent<UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration>();

            //Weapons

            //WeaponCamera
            if (!weapon_camera.GetComponent<Camera>())
                weapon_camera.AddComponent<Camera>();

            //WeaponManager
            if (!weapon_manager.GetComponent<WeaponManagerNew>())
                weapon_manager.AddComponent<WeaponManagerNew>();
                
            //FlashLight
            if (!flashlight.GetComponent<Light>())
                flashlight.AddComponent<Light>();
            if (!flashlight.GetComponent<FlashLight>())
                flashlight.AddComponent<FlashLight>();
            if (!flashlight.GetComponent<AudioSource>())
                flashlight.AddComponent<AudioSource>();
        }
        void MakeComponentConnections() {

        }
        #endregion
    }

}
