﻿/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    /*
     * Global Asset references
     * Edit Asset references in the prefab CodeMonkey/Resources/CodeMonkeyAssets
     * */
    public class Assets : MonoBehaviour {

        // Internal instance reference
        private static Assets _i; 

        // Instance reference
        public static Assets i {
            get {
                if (_i == null) _i = Instantiate(Resources.Load<Assets>("Assets")); 
                return _i; 
            }
        }


        // All references
        
        public Sprite s_White;
        public Sprite s_Circle;

        public Material m_White;

        public Transform pfDamagePopup;
        public Transform pfCriticalPopup;
        public Transform pfEffectivePopup;
        public Transform pfUneffectivePopup;
        public Transform pfBleedingPopup;
        public Transform pfBloodFrenzyPopup;
        public Transform pfRalentizadoPopup;
        public Sprite Helah_neutral;
        public Sprite Hades_neutral;

    }


