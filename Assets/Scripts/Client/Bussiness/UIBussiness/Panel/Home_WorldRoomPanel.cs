using UnityEngine;
using ZeroUIFrame;
using Game.Client.Bussiness.EventCenter;
using Game.Client.Bussiness.UIBussiness;
using UnityEngine.UI;

namespace Game.Bussiness.UIBussiness.Panel
{

    public class Home_WorldRoomPanel : UIBehavior
    {

        InputField roomNameInput;
        int roomIndex;

        void Awake()
        {
            NetworkEventCenter.Regist_WorldRoomCreate(CreateRoomSuccess);

            roomNameInput = GetComponentFromChild<InputField>("CreateRoom/InputField");
        }

        void OnEnable()
        {
            SetOnClick("CreateRoom", ClickCreateRoomBtn);
        }

        // == Network ==
        void CreateRoomSuccess(string masterAccount, string roomName)
        {
            var go = UIManager.GetUIAsset("RoomItem");
            go = GameObject.Instantiate(go);
            string content = "RoomGroup/Viewport/Content";
            var parent = transform.Find(content);
            Debug.Assert(parent != null);
            go.transform.SetParent(parent, true);
            go.transform.name += roomIndex++;
            string path = $"{content}/{go.transform.name}";
            string info = $"房间名:{roomName} \n 房主:{masterAccount} ";
            Text_SetText(path + "/Info", info);
        }

        // == UI Click ==
        void ClickCreateRoomBtn(params object[] args)
        {
            UIEventCenter.WorldRoomCreateAction.Invoke(roomNameInput.text);
        }

    }

}