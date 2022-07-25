using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public class CameraManager : Singleton<CameraManager>
    {
        public const string MainCameraPrefabPath = "Camera/MainCamera";

        public Camera MainCamera => mainCamera;

        private Camera mainCamera;

        public override void Init()
        {
            base.Init();

            InitMainCamera();
        }

        public override void Release()
        {
            base.Release();

            ClearMainCamera();
        }

        private void InitMainCamera()
        {
            ClearMainCamera();

            GameObject obj = Object.Instantiate(Resources.Load<GameObject>(MainCameraPrefabPath));
            mainCamera = obj.GetComponent<Camera>();
        }

        private void ClearMainCamera()
        {
            if (mainCamera != null)
            {
                Object.Destroy(mainCamera.gameObject);
            }
        }
    }

}