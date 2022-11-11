using System.Collections.Generic;
using UnityEngine;
using ZeroUIFrame;
using Game.Client.Bussiness.EventCenter;
using UnityEngine.UI;

namespace Game.Client.Bussiness.UIBussiness.Panel
{


    public class Home_WorldRoomPanel : UIBehavior
    {

        InputField roomNameInput;

        struct WorldRoom
        {
            public string roomName;
            public int roomMemberNum;
            public int masterID;
            public string masterAccount;
            public string roomUIPath;
            public string host;
            public ushort port;
        }

        Dictionary<int, WorldRoom> worldRoomDic;

        void Awake()
        {
            roomNameInput = GetComponentFromChild<InputField>("CreateRoom/InputField");
            worldRoomDic = new Dictionary<int, WorldRoom>();

            NetworkEventCenter.Regist_WorldRoomCreate(OnCreateRoomSuccess);
            NetworkEventCenter.Regist_WorldRoomDismiss(OnWorldRoomDismiss);

            NetworkEventCenter.Regist_AllWorldRoomsBasicInfo(OnAllWorldRoomsBasicInfo);

            UIEventCenter.World_ReqAllRoomsBasicInfoAction.Invoke();
        }

        void OnEnable()
        {
            OnPointerDown("CreateRoom", ClickCreateRoomBtn);
        }

        string CreateRoom(string roomName, int roomMemberNum, string masterAccount, string host, ushort port)
        {
            var prefab = UIManager.GetUIAsset("RoomItem");
            var go = GameObject.Instantiate(prefab);

            string content = "RoomGroup/Viewport/Content";
            var parent = transform.Find(content);
            go.transform.SetParent(parent, true);

            var goName = $"{roomName}_{masterAccount}";
            go.transform.name = goName;

            string uiPath = $"{content}/{goName}";
            string info = $"房间名: {roomName} \n房主: {masterAccount} \n人数: {roomMemberNum} ";
            Text_SetText(uiPath + "/Info", info);

            OnPointerDown(uiPath, ClickRoom, host, port); // 一个人限制创建一个房间
            Debug.Log($"CreateRoom uiPath: {uiPath} masterAccount: {masterAccount} roomName: {roomName} host: {host} port: {port}");
            return uiPath;
        }

        void DismissRoom(int roomID)
        {
            if (worldRoomDic.TryGetValue(roomID, out var worldRoom))
            {
                var uiPath = worldRoom.roomUIPath;
                GameObject.Destroy(transform.Find(uiPath).gameObject);
                worldRoomDic.Remove(roomID);
                Debug.Log($"DismissRoom roomID {roomID}");
            }
        }

        void UpdateRoomInfo(WorldRoom worldRoom)
        {
            Debug.Log($"UpdateRoomInfo");
            string info = $"房间名: {worldRoom.roomName} \n房主: {worldRoom.masterAccount} \n人数: {worldRoom.roomMemberNum} ";
            Text_SetText($"{worldRoom.roomUIPath}/Info", info);
        }

        // == Network ==
        void OnCreateRoomSuccess(string roomName, string masterAccount, string host, ushort port)
        {
            CreateRoom(roomName, 0, masterAccount, host, port);
        }

        void OnWorldRoomDismiss(int roomID)
        {
            DismissRoom(roomID);
        }

        void OnAllWorldRoomsBasicInfo(int[] worldRoomIDs, string[] worldRoomNames, int[] worldRoomMemberNums, int[] masterIDs, string[] masterAccounts, string[] hosts, ushort[] ports)
        {
            Debug.Log($"OnAllWorldRoomsBasicInfo");
            var len = worldRoomIDs.Length;
            for (int i = 0; i < len; i++)
            {
                var roomID = worldRoomIDs[i];
                var roomName = worldRoomNames[i];
                var worldRoomMemberNum = worldRoomMemberNums[i];
                var masterID = masterIDs[i];
                var masterAccount = masterAccounts[i];
                var host = hosts[i];
                var port = ports[i];
                if (worldRoomDic.TryGetValue(roomID, out var worldRoom))
                {
                    worldRoom.roomName = roomName;
                    worldRoom.masterID = masterID;
                    worldRoom.masterAccount = masterAccount;
                    worldRoom.roomMemberNum = worldRoomMemberNum;
                    UpdateRoomInfo(worldRoom);
                }
                else
                {
                    worldRoom = new WorldRoom();
                    worldRoom.roomName = roomName;
                    worldRoom.masterID = masterID;
                    worldRoom.masterAccount = masterAccount;
                    worldRoom.roomMemberNum = worldRoomMemberNum;
                    worldRoom.roomUIPath = CreateRoom(roomName, worldRoomMemberNum, masterAccount, host, port);
                    worldRoomDic[roomID] = worldRoom;
                }
            }
        }

        // == UI Click ==
        void ClickCreateRoomBtn(params object[] args)
        {
            UIEventCenter.World_CreateRoomAction.Invoke(roomNameInput.text);
        }

        void ClickRoom(params object[] args)
        {
            var argsArray = args[0] as object[];
            //点击进入
            string host = (string)argsArray[0];
            ushort port = (ushort)argsArray[1];

            UIEventCenter.World_EnterRoom.Invoke(host, port);
            UIEventCenter.AddToTearDown("Home_WorldRoomPanel");
        }

    }

}