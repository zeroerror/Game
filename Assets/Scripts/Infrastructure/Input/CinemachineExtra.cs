using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Game.Infrastructure.Input
{

    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CinemachineExtra : MonoBehaviour
    {

        // == SerializeField ==
        [SerializeField]
        CinemachineTargetGroup _FollowGroup;
        [SerializeField]
        CinemachineTargetGroup _LookAtGroup;

        CinemachineFramingTransposer framingTransposer;
        public CinemachineFramingTransposer FramingTransposer => framingTransposer;

        CinemachineVirtualCamera cinemachineVirtual;

        public int CurrentFollowCount { get; private set; }
        public int CurrentLookAtCount { get; private set; }

        const int MAX_TARGET_FOLLOW = 3;
        const int MAX_TARGET_LOOKAT = 3;

        void Awake()
        {
            _FollowGroup = GameObject.Find("FollowGroup").GetComponent<CinemachineTargetGroup>();
            _LookAtGroup = GameObject.Find("TargetGroup").GetComponent<CinemachineTargetGroup>();
            Debug.Assert(_FollowGroup != null, "_FollowGroup Is Null!");
            Debug.Assert(_LookAtGroup != null, "_TargetGroup Is Null!");
            _FollowGroup.m_Targets = new CinemachineTargetGroup.Target[MAX_TARGET_FOLLOW];
            _LookAtGroup.m_Targets = new CinemachineTargetGroup.Target[MAX_TARGET_LOOKAT];

            // Find
            cinemachineVirtual = GetComponent<CinemachineVirtualCamera>();
            cinemachineVirtual.m_Follow = _FollowGroup.transform;
            cinemachineVirtual.m_LookAt = _LookAtGroup.transform;
            framingTransposer = cinemachineVirtual.GetCinemachineComponent<CinemachineFramingTransposer>();
            Debug.Assert(framingTransposer != null, "framingTransposer Is Null!");
        }

        // == Follow ==
        public void FollowSolo(Transform trans, float weight = 1f, float radius = 0f)
        {
            ResetFollowGroup();
            var target = _FollowGroup.m_Targets[0];
            target.target = trans;
            target.weight = weight;
            target.radius = radius;
            _FollowGroup.m_Targets[0] = target;
            CurrentFollowCount = 1;

            cinemachineVirtual.m_Follow = trans;
            Debug.Log($"FollowSolo: target{target.target.name}");
        }

        public void AddFollowMember(Transform trans, float weight, float radius)
        {
            var target = _FollowGroup.m_Targets[CurrentFollowCount++];
            target.target = trans;
            target.weight = weight;
            target.radius = radius;
            _FollowGroup.m_Targets[CurrentFollowCount] = target;

            cinemachineVirtual.m_Follow = _FollowGroup.transform;
        }

        public void ResetFollowGroup()
        {
            for (int i = 0; i < CurrentFollowCount; i++)
            {
                var target = _FollowGroup.m_Targets[i];
                target.target = null;
                target.weight = 0;
                target.radius = 0;
                _FollowGroup.m_Targets[i] = target;
            }

            CurrentFollowCount = 0;
        }

        // == Look At ==
        public void LookAtSolo(Transform trans, float weight = 1f, float radius = 0f)
        {
            ResetFollowGroup();
            var target = _LookAtGroup.m_Targets[0];
            target.target = trans;
            target.weight = 1;
            target.radius = radius;
            _LookAtGroup.m_Targets[0] = target;
            CurrentLookAtCount = 1;

            cinemachineVirtual.m_LookAt = trans;
        }

        public void AddLookAtMember(Transform trans, float weight, float radius)
        {
            var target = _LookAtGroup.m_Targets[CurrentFollowCount++];
            target.target = trans;
            target.weight = weight;
            target.radius = radius;
            _LookAtGroup.m_Targets[CurrentFollowCount++] = target;

            cinemachineVirtual.m_LookAt = _LookAtGroup.transform;
        }

        public void ResetLookAtGroup()
        {
            for (int i = 0; i < CurrentFollowCount; i++)
            {
                var target = _LookAtGroup.m_Targets[i];
                target.target = null;
                target.weight = 0;
                target.radius = 0;
                _LookAtGroup.m_Targets[i] = target;
            }

            CurrentLookAtCount = 0;
        }

        // == Rotation ==
        public void AddEulerAngleX(float eulerAngleX)
        {
            var euler = transform.rotation.eulerAngles;
            euler.x += eulerAngleX;
            transform.rotation = Quaternion.Euler(euler);
        }

        public void SetEulerAngleX(float eulerAngleX)
        {
            var euler = transform.rotation.eulerAngles;
            euler.x = eulerAngleX;
            transform.rotation = Quaternion.Euler(euler);
        }

        public void AddEulerAngleY(float eulerAngleY)
        {
            var euler = transform.rotation.eulerAngles;
            euler.y += eulerAngleY;
            transform.rotation = Quaternion.Euler(euler);
        }

        public void SetEulerAngleY(float eulerAngleY)
        {
            var euler = transform.rotation.eulerAngles;
            euler.y = eulerAngleY;
            transform.rotation = Quaternion.Euler(euler);
        }


    }

}

